using Microsoft.AspNetCore.Mvc;
using Moq;
using movie_recommendation.Controllers;
using movie_recommendation.Data;
using movie_recommendation.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace movie_recommendation.Tests
{
    class RatingsControllerTests
    {

        List<Rating> ratings = new List<Rating>
        {
            new Rating { movieId = 1, rating=4.0f, timestamp=DateTime.Now, userId = 1},
            new Rating { movieId = 2, rating=4.0f, timestamp=DateTime.Now, userId = 2},
            new Rating { movieId = 1, rating=4.0f, timestamp=DateTime.Now, userId = 3},
            new Rating { movieId = 1, rating=4.0f, timestamp=DateTime.Now, userId = 4},
            new Rating { movieId = 3, rating=4.0f, timestamp=DateTime.Now, userId = 1}
        };

        [SetUp]
        public void Setup()
        {

        }


        [Test]
        public void GetRatings_GetRatingsByMovieId()
        {
            //Arrange
            Mock<IRepository<Movie>> mockIRepository = new Mock<IRepository<Movie>>();
            Mock<IRatingRepository> mockRatingRepository = new Mock<IRatingRepository>();

            mockRatingRepository.Setup(r => r.GetRatings(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int id, int page, int pageSize) => ratings.Where(x => x.movieId == id));

            var controller = new RatingsController( mockRatingRepository.Object, mockIRepository.Object);

            //Act
            List<Rating> allRatings = (List<Rating>)controller.GetRatings(1, 1, 1).Value;

            //Assert
            Assert.AreEqual(3, allRatings.Count());
            Assert.AreEqual(ratings.Where(rat => rat.movieId == 1), allRatings);
            
        }

        [Test]
        public void GetRatings_ReturnNotFoundWhenMovieHasNoRatings()
        {
            //Arrange
            Mock<IRepository<Movie>> mockIRepository = new Mock<IRepository<Movie>>();
            Mock<IRatingRepository> mockRatingRepository = new Mock<IRatingRepository>();

            mockRatingRepository.Setup(r => r.GetRatings(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int id, int page, int pageSize) => ratings.Where(x => x.movieId == id));

            var controller = new RatingsController(mockRatingRepository.Object, mockIRepository.Object);

            //Act
            var result = controller.GetRatings(5,1,1).Result;

            //Assert

            Assert.IsInstanceOf<NotFoundResult>(result);

        }

        [Test]
        public void GetRating_GetRatingByUserIdAndMovieId()
        {
            //Arrange
            Mock<IRepository<Movie>> mockIRepository = new Mock<IRepository<Movie>>();
            Mock<IRatingRepository> mockRatingRepository = new Mock<IRatingRepository>();

            mockRatingRepository.Setup(r => r.GetRating(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int userId, int movieId) => ratings.Where(x => x.movieId == movieId && x.userId == userId).SingleOrDefault());

            var controller = new RatingsController(mockRatingRepository.Object, mockIRepository.Object);

            //Act
            Rating rating = (Rating)controller.GetRating(1, 1).Value;

            //Assert
            Assert.AreEqual(ratings[0], rating);

        }

        [Test]
        public void GetRating_ReturnNotFoundIfRatingIsNone()
        {
            //Arrange
            Mock<IRepository<Movie>> mockIRepository = new Mock<IRepository<Movie>>();
            Mock<IRatingRepository> mockRatingRepository = new Mock<IRatingRepository>();

            mockRatingRepository.Setup(r => r.GetRating(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int userId, int movieId) => ratings.Where(x => x.movieId == movieId && x.userId == userId).SingleOrDefault());

            var controller = new RatingsController(mockRatingRepository.Object, mockIRepository.Object);

            //Act
            var result = controller.GetRating(23, 1).Result;

            //Assert
            Assert.IsInstanceOf<NotFoundResult>(result);

        }


    }
}
