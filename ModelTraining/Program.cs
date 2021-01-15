using Microsoft.ML;
using Microsoft.ML.Data;
using movie_recommendation.Entities;
using System;
using System.Data.SqlClient;
using Microsoft.ML.Trainers;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;

namespace ModelTraining
{
    class Program
    {
        static MLContext mlContext = new MLContext();
        static DataTable ratings = null;
        static DataTable users = null;
        static List<Rating> myRatings;
        static List<Rating> myUsers;
        static void Main(string[] args)
        {
            var data = LoadData();

            ITransformer model = BuildAndTrainModel(mlContext, data.training);
            EvaluateModel(mlContext, data.test, model);
            UseModelForSinglePrediction(mlContext, model, 6);
            SaveModel(mlContext, data.training.Schema, model);
            SavePredictionsToDb(mlContext, model);
        }

        private static SQLiteConnection OpenSqlConnection()
        {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection(GetConnectionString());
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }


        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code,
            // you can retrieve it from a configuration file.
            return @"Data Source='..\..\..\movierecommendationapp.db'";
        }

        public static (IDataView training, IDataView test) LoadData()
        {
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<Rating>();
            string connectionString = @"Data Source='..\..\..\movierecommendationapp.db'";
            IDataView data;
            myRatings = new List<Rating>();
            int offset = 0;

            do
            {
                string command = "SELECT * FROM Ratings LIMIT 100 OFFSET " + offset;
                offset += 100;
                DatabaseSource dbSource = new DatabaseSource(SqliteFactory.Instance, connectionString, command);
                data = loader.Load(dbSource);
                ratings = data.ToDataTable();
                if (ratings.Rows.Count == 0)
                    break;
                if (offset % 100000 == 0)
                    Console.Write(offset + " ");
                myRatings.AddRange(ratings.Rows.Cast<DataRow>()
                        .Select(r => new Rating()
                        {
                            userId = Int32.Parse(r.Field<String>("userId")),
                            movieId = Int32.Parse(r.Field<String>("movieId")),
                            rating = float.Parse(r.Field<String>("rating"))
                        }).ToList());
            } while (offset < 25000000);

            data = mlContext.Data.LoadFromEnumerable<Rating>(myRatings);

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
                NumberOfIterations = 20,
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

        public static List<int> UseModelForSinglePrediction(MLContext mlContext, ITransformer model, int id)
        {
            //Console.WriteLine("=============== Making a prediction ===============");
            var predictionEngine = mlContext.Model.CreatePredictionEngine<Rating, RatingPrediction>(model);

           // Console.WriteLine("Calculating the top 25 movies for user " + id.ToString());

            List<int> movieIds = (from m in myRatings select m.movieId).Distinct().ToList();
            var top = (from _movieId in movieIds
                        let p = predictionEngine.Predict(
                           new Rating()
                           {
                               userId = id,
                               movieId = _movieId
                           })
                        orderby p.score descending
                        select (MovieId: _movieId, Score: p.score)).Take(25);

            List<int> recommendedMovieIds = new List<int>();
            foreach (var t in top)
            {
                recommendedMovieIds.Add(t.MovieId);
                //Console.WriteLine(t.MovieId + " " + t.Score);
            }
            return recommendedMovieIds;
        }

        public static void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            System.IO.Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Data"));
            var modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "MovieRecommenderModel7.zip");

            Console.WriteLine("=============== Saving the model to a file ===============");
            mlContext.Model.Save(model, trainingDataViewSchema, modelPath);
            Console.WriteLine("Model saved to " + modelPath);
        }

        public static void SavePredictionsToDb(MLContext mlContext, ITransformer model)
        {
            //var users = (from m in myUsers select m);
            //int users = 500;
            //List<(int, int)> userRecommendations = new List<(int, int)>();
            SQLiteConnection connection = OpenSqlConnection();
            List<int> utilizatori = (from m in myRatings select m.userId).Distinct().ToList();
            /*SQLiteCommand myComm = connection.CreateCommand();
            myComm.CommandText = @"SELECT Id FROM Users";
            SQLiteDataReader r = myComm.ExecuteReader();
            while (r.Read())
            {
                utilizatori.Add(Convert.ToInt32(r["Id"]));
            }*/
            for (int i = 0; i < utilizatori.Count; i++)
            {
                List<int> recommendedMovieIds = UseModelForSinglePrediction(mlContext, model, utilizatori[i]);
                foreach (var recommendation in recommendedMovieIds)
                {
                    string query = "INSERT INTO Recommendations (userId, movieId)";
                    query += " VALUES (@u_id, @m_id)";

                    SQLiteCommand myCommand = new SQLiteCommand(query, connection);
                    myCommand.Parameters.AddWithValue("@u_id", utilizatori[i]);
                    myCommand.Parameters.AddWithValue("@m_id", recommendation);
                    myCommand.ExecuteNonQuery();
                    //Console.WriteLine(recommendation);
                }
            }

        }


    }
}