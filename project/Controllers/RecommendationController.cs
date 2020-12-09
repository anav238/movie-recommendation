using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using movie_recommendation.Data;
using movie_recommendation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movie_recommendation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly PredictionEnginePool<Rating, RatingPrediction> _model;

        private readonly IRatingRepository _ratingRepository;
        private readonly IRepository<Movie> _movieRepository;
        public RecommendationController(PredictionEnginePool<Rating, RatingPrediction> model, IRatingRepository ratingRepository, IRepository<Movie> movieRepository)
        {
            _model = model;
            _ratingRepository = ratingRepository;
            _movieRepository = movieRepository;
        }

        // GET: /api/recommendations/{userId}
        [HttpGet("recommendations/{userId}")]
        public ActionResult<List<(int movieId, float normalizedScore)>> GetRecommendedMovies(int userId)
        {
            MLContext mlContext = new MLContext();
            List<(int movieId, float normalizedScore)> ratings = new List<(int movieId, float normalizedScore)>();
            var MovieRatings = _ratingRepository.GetUserRatings(userId);
            List<Movie> WatchedMovies = new List<Movie>();

            foreach (Rating rating in MovieRatings)
                WatchedMovies.Add(_movieRepository.GetById(rating.movieId));

            RatingPrediction prediction = null;
            foreach (var movie in _movieRepository.GetAll())
            {
                // Call the Rating Prediction for each movie prediction
                prediction = _model.Predict(new Rating
                {
                    userId = userId,
                    movieId = movie.Id
                });

                // Normalize the prediction scores for the "ratings" b/w 0 - 100
                float normalizedscore = Sigmoid(prediction.Score);

                // Add the score for recommendation of each movie in the trending movie list
                ratings.Add((movie.Id, normalizedscore));
            }
           
            return ratings;
        }

        public float Sigmoid(float x)
        {
            return (float)(100 / (1 + Math.Exp(-x)));
        }

    }
}
