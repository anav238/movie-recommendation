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
    class TagsControllerTests
    {

        List<Tag> tags = new List<Tag>
                {
                    new Tag { id = 1, movieId = 1, tag="Amazing", timestamp = DateTime.Now, userId = 1},
                    new Tag { id = 2, movieId = 1, tag="Amazing", timestamp = DateTime.Now, userId = 2},
                    new Tag { id = 3, movieId = 1, tag="Amazing", timestamp = DateTime.Now, userId = 3},
                    new Tag { id = 4, movieId = 1, tag="Amazing", timestamp = DateTime.Now, userId = 4},

                };

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GetTag_GetTagByUserAndMovie()
        {
            //Arrange
            Mock<ITagRepository> mockTagRepository = new Mock<ITagRepository>();

            mockTagRepository.Setup(tag => tag.GetTag(It.IsAny<int>(), It.IsAny<int>())).Returns((int userId, int movieId) => 
            tags.Where(x => x.userId == userId && x.movieId == movieId).SingleOrDefault());

            var controller = new TagsController(mockTagRepository.Object);

            //Act
            Tag tag = controller.GetTag(1, 1).Value;

            //Assert
            Assert.IsNotNull(tag);
            Assert.AreEqual(tag, tags[0]);
        }

        [Test]
        public void GetTag_GetNotFoundIfTagIsNone()
        {
            //Arrange
            Mock<ITagRepository> mockTagRepository = new Mock<ITagRepository>();

            mockTagRepository.Setup(tag => tag.GetTag(It.IsAny<int>(), It.IsAny<int>())).Returns((int userId, int movieId) =>
            tags.Where(x => x.userId == userId && x.movieId == movieId).SingleOrDefault());

            var controller = new TagsController(mockTagRepository.Object);

            //Act
            var result = controller.GetTag(1,1324).Result;

            //Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            
        }

        [Test]
        public void GetTags_GetTagsByMovieId()
        {
            //Arrange
            Mock<ITagRepository> mockTagRepository = new Mock<ITagRepository>();

            mockTagRepository.Setup(tag => tag.GetTags(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns( ( int movieId , int page, int pageSize) =>
            tags.Where(x => x.movieId == movieId));

            var controller = new TagsController(mockTagRepository.Object);

            //Act
            List<Tag> allTags = (List<Tag>)controller.GetTags(1, 1, 100).Value;

            //Assert
            Assert.IsNotEmpty(allTags);
            Assert.AreEqual(allTags, tags.Where(tag => tag.movieId == 1).ToList());

        }

        [Test]
        public void GetTags_ReturnNotFoundIfMovieHasNoTags()
        {
            //Arrange
            Mock<ITagRepository> mockTagRepository = new Mock<ITagRepository>();

            mockTagRepository.Setup(tag => tag.GetTags(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tag>());

            var controller = new TagsController(mockTagRepository.Object);

            //Act
            var result = controller.GetTags(213, 1, 100).Result;

            //Assert
            Assert.IsInstanceOf<NotFoundResult>(result);

        }

    }
}
