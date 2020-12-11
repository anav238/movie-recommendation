using Microsoft.ML;
using Microsoft.ML.Data;
using movie_recommendation.Entities;
using System;
using System.Data.SqlClient;
using Microsoft.ML.Trainers;
using System.IO;
using Microsoft.Data.Sqlite;

namespace ModelTraining
{
    class Program
    {
        static MLContext mlContext = new MLContext();
        static void Main(string[] args)
        {
            var data = LoadData();

            ITransformer model = BuildAndTrainModel(mlContext, data.training);
            EvaluateModel(mlContext, data.test, model);
            UseModelForSinglePrediction(mlContext, model);
            SaveModel(mlContext, data.training.Schema, model);
        }

        public static (IDataView training, IDataView test) LoadData()
        {
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<Rating>();
            string connectionString = @"Data Source='..\..\..\movierecommendationapp.db'";
            string command = "SELECT * FROM Ratings";
            DatabaseSource dbSource = new DatabaseSource(SqliteFactory.Instance, connectionString, command);
            IDataView data = loader.Load(dbSource);
            DataOperationsCatalog.TrainTestData dataSplit = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            return (dataSplit.TrainSet, dataSplit.TestSet);
        }

        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
        {
            IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "movieIdEncoded", inputColumnName: "movieId"));

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "movieIdEncoded",
                LabelColumnName = "rating",
                NumberOfIterations = 5000,
                ApproximationRank = 100
            };

            var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

            Console.WriteLine("=============== Training the model ===============");
            ITransformer model = trainerEstimator.Fit(trainingDataView);

            return model;
        }

        public static void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model)
        {
            Console.WriteLine("=============== Evaluating the model ===============");
            var prediction = model.Transform(testDataView);

            var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "rating", scoreColumnName: "Score");
            Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
            Console.WriteLine("RSquared: " + metrics.RSquared.ToString());
        }

        public static void UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
        {
            Console.WriteLine("=============== Making a prediction ===============");
            var predictionEngine = mlContext.Model.CreatePredictionEngine<Rating, RatingPrediction>(model);
            var testInput = new Rating { userId = 6, movieId = 10 };

            var movieRatingPrediction = predictionEngine.Predict(testInput);
            if (Math.Round(movieRatingPrediction.score, 1) > 7)
            {
                Console.WriteLine("Movie " + testInput.movieId + " is recommended for user " + testInput.userId);
            }
            else
            {
                Console.WriteLine("Movie " + testInput.movieId + " is not recommended for user " + testInput.userId);
            }
        }

        public static void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            System.IO.Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Data"));
            var modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "MovieRecommenderModel.zip");

            Console.WriteLine("=============== Saving the model to a file ===============");
            mlContext.Model.Save(model, trainingDataViewSchema, modelPath);
            Console.WriteLine("Model saved to " + modelPath);
        }

      
    }
}
