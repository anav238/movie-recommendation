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
    public class FriendshipsControllerTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        List<Friendship> friendships = new List<Friendship>
                {
                    new Friendship { UserId_1 = 1, UserId_2 = 2},
                    new Friendship { UserId_1 = 1, UserId_2 = 3},
                    new Friendship { UserId_1 = 2, UserId_2 = 3},
                    new Friendship { UserId_1 = 2, UserId_2 = 4},
                    new Friendship { UserId_1 = 2, UserId_2 = 5},
                    new Friendship { UserId_1 = 3, UserId_2 = 1},
                    new Friendship { UserId_1 = 3, UserId_2 = 2},
                    new Friendship { UserId_1 = 3, UserId_2 = 4},
                    new Friendship { UserId_1 = 5, UserId_2 = 6},
                    new Friendship { UserId_1 = 6, UserId_2 = 5},
                    new Friendship { UserId_1 = 7, UserId_2 = 2},
                    new Friendship { UserId_1 = 7, UserId_2 = 3}
                };

        [Test]
        public void GetFriendship_ValidFriendshipsAreNotNull()
        {
           
            //Arrange
            Mock<IFriendshipRepository> mockFrienshipRepository = new Mock<IFriendshipRepository>(); 

            mockFrienshipRepository.Setup(mr => mr.GetFriendship(
                It.IsAny<int>(), It.IsAny<int>())).Returns((int i, int j) => friendships.Where(
                (x, y) => x.UserId_1 == i && x.UserId_2 == j).SingleOrDefault());

            var controller = new FriendshipsController(mockFrienshipRepository.Object);

            //Act
            Friendship friend = (Friendship)controller.GetFriendship(2, 3).Value;
            Console.WriteLine(friend.UserId_1);

            //Assert
            Assert.IsNotNull(friend);
            Assert.AreEqual(friend, friendships[2]);


        }

        [Test]
        public void GetFriendship_InvalidFriendshipNotFound()
        {

            //Arrange
            Mock<IFriendshipRepository> mockFriendshipRepository = new Mock<IFriendshipRepository>();

            mockFriendshipRepository.Setup(mr => mr.GetFriendship(
                It.IsAny<int>(), It.IsAny<int>())).Returns((int i, int j) => friendships.Where(
                (x, y) => x.UserId_1 == i && x.UserId_2 == j).SingleOrDefault());

            var controller = new FriendshipsController(mockFriendshipRepository.Object);

            //Act
            Friendship friend = (Friendship)controller.GetFriendship(1, 1).Value;
            

            //Assert
            Assert.Null(friend);


        }

        [Test]
        public void GetFriendships_ReturnsAll()
        {

            //Arrange
            Mock<IFriendshipRepository> mockFrienshipRepository = new Mock<IFriendshipRepository>();

            mockFrienshipRepository.Setup(fr => fr.GetAll(1,12)).Returns(friendships);

            var controller = new FriendshipsController(mockFrienshipRepository.Object);

            //Act
            List<Friendship> allFriendships = (List<Friendship>)controller.GetFriendships(1,12).Value;
            Console.WriteLine(allFriendships);

            //Assert
            Assert.AreEqual(12, allFriendships.Count());
            Assert.AreEqual(allFriendships, friendships);
            

        }

        [Test]
        public void GetFriends_ReturnsNotFoundWhenFriendsAreNone()
        {

            //Arrange
            Mock<IFriendshipRepository> mockFrienshipRepository = new Mock<IFriendshipRepository>();

            mockFrienshipRepository.Setup(fr => fr.GetFriends(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns((IEnumerable<Friendship>)null);

            var controller = new FriendshipsController(mockFrienshipRepository.Object);

            //Act
            var result = controller.GetFriends(1,1,5).Result;


            //Assert
            
            Assert.IsInstanceOf<NotFoundResult>(result);



        }

    }
}