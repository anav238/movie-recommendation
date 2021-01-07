using Microsoft.AspNetCore.Mvc;
using Moq;
using movie_recommendation.Controllers;
using movie_recommendation.Data;
using movie_recommendation.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace movie_recommendation.Tests
{
    public class UsersControllerTests
    {
        List<User> users = new List<User>
                {
                    new User { Id = 1, Username="Testing", DateCreated=DateTime.Now, Password="testare"},
                    new User { Id = 2, Username="Testing2", DateCreated=DateTime.Now, Password="testare"},
                    new User { Id = 3, Username="Testing3", DateCreated=DateTime.Now, Password="testare"}

                };

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GetFriendMovies_ReturnsNotFoundWhenUsersFriendIsNone()
        {

            //Arrange
            Mock<IRepository<User>> mockIRepository = new Mock<IRepository<User>>();
            Mock<IUserRepository> mockUserRepository = new Mock<IUserRepository>();

            mockUserRepository.Setup(m => m.GetFriendship(It.IsAny<int>(), It.IsAny<int>())).Returns((Friendship)null);

            var controller = new UsersController(mockIRepository.Object, mockUserRepository.Object);

            //Act
            var result = controller.GetFriendMovies(1, 1).Result;

            //Assert

            Assert.IsInstanceOf<NotFoundResult>(result);


        }

        [Test]
        public void GetFriendsMovies_ReturnsNotFoundWhenUsersFriendsMoviesIsNone()
        {

            //Arrange
            Mock<IRepository<User>> mockIRepository = new Mock<IRepository<User>>();
            Mock<IUserRepository> mockUserRepository = new Mock<IUserRepository>();

            mockUserRepository.Setup(m => m.GetFriendsMovies(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Movie>());

            var controller = new UsersController(mockIRepository.Object, mockUserRepository.Object);

            //Act
            var result = controller.GetFriendsMovies(1, 1,100).Result;

            //Assert

            Assert.IsInstanceOf<NotFoundResult>(result);


        }

        [Test]
        public void GetById_ReturnsUserById()
        {

            //Arrange
            Mock<IRepository<User>> mockIRepository = new Mock<IRepository<User>>();
            Mock<IUserRepository> mockUserRepository = new Mock<IUserRepository>();

            mockIRepository.Setup(m => m.GetById(It.IsAny<int>())).Returns( (int id) => users.Where( x => x.Id == id ).SingleOrDefault());

            var controller = new UsersController(mockIRepository.Object, mockUserRepository.Object);

            //Act
            var user = controller.GetById(1).Value;

            //Assert

            Assert.IsInstanceOf<User>(user);
            Assert.AreEqual(users[0], user);

        }

        [Test]
        public void GetById_ReturnsNotFoundWhenUserIsNone()
        {

            //Arrange
            Mock<IRepository<User>> mockIRepository = new Mock<IRepository<User>>();
            Mock<IUserRepository> mockUserRepository = new Mock<IUserRepository>();

            mockIRepository.Setup(m => m.GetById(It.IsAny<int>())).Returns((int id) => users.Where(x => x.Id == id).SingleOrDefault());

            var controller = new UsersController(mockIRepository.Object, mockUserRepository.Object);

            //Act
            var result = controller.GetById(4).Result;

            //Assert

            Assert.IsInstanceOf<NotFoundResult>(result);


        }





    }
}