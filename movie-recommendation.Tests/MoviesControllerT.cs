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
    class MoviesControllerTests
    {

        List<Movie> movies = new List<Movie>
                {
                    new Movie { Id = 1, Genres="Comedy", ImdbId = 12334, NumberOfRatings= 1234, Rating = 3.5f, Title = "Testing", TmdbId = 1212344},
                    new Movie { Id = 2, Genres="Comedy", ImdbId = 123443, NumberOfRatings= 1234, Rating = 3.5f, Title = "Testing", TmdbId = 14},
                    new Movie { Id = 3, Genres="Comedy", ImdbId = 122, NumberOfRatings= 1234, Rating = 3.5f, Title = "Testing", TmdbId = 124},
                    new Movie { Id = 4, Genres="Comedy", ImdbId = 1244, NumberOfRatings= 1234, Rating = 3.5f, Title = "Testing", TmdbId = 14},
                    new Movie { Id = 5, Genres="Comedy", ImdbId = 1214, NumberOfRatings= 1234, Rating = 3.5f, Title = "Testing", TmdbId = 4}

                };

        [SetUp]
        public void Setup()
        {

        }


        [Test]
        public void GetById_GetMovieById()
        {
            //Arrange
            Mock<IRepository<Movie>> mockIRepository = new Mock<IRepository<Movie>>();
            Mock<IMovieRepository> mockMovieRepository = new Mock<IMovieRepository>();

            mockIRepository.Setup(m => m.GetById(It.IsAny<int>())).Returns((int id) => movies.Where(x => x.Id == id).SingleOrDefault());

            var controller = new MoviesController(mockIRepository.Object, mockMovieRepository.Object);

            //Act
            var movie = controller.GetById(1).Value;

            //Assert

            Assert.IsInstanceOf<Movie>(movie);
            Assert.AreEqual(movies[0], movie);
        }

        [Test]
        public void GetById_ReturnNotFoundIfMovieIsNone()
        {
            //Arrange
            Mock<IRepository<Movie>> mockIRepository = new Mock<IRepository<Movie>>();
            Mock<IMovieRepository> mockMovieRepository = new Mock<IMovieRepository>();

            mockIRepository.Setup(m => m.GetById(It.IsAny<int>())).Returns((int id) => movies.Where(x => x.Id == id).SingleOrDefault());

            var controller = new MoviesController(mockIRepository.Object, mockMovieRepository.Object);

            //Act
            var result = controller.GetById(111).Result;

            //Assert

            Assert.IsInstanceOf<NotFoundResult>(result);

        }


    }
}
