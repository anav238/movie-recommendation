﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movie_recommendation.Data;
using movie_recommendation.Entities;

namespace movie_recommendation.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FriendshipsController : ControllerBase
    {
        private readonly IFriendshipRepository _repository;

        public FriendshipsController(IFriendshipRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Friendships
        [HttpGet]
        public ActionResult<IEnumerable<Friendship>> GetFriendships(int page=1 , int pageSize=100)
        {
            return _repository.GetAll(page, pageSize).ToList();
        }

        // GET: api/Friendships/5
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Friendship>> GetFriends(int id, int page = 1, int pageSize=100)
        {
            var friendship = _repository.GetFriends(id,page, pageSize);

            if (friendship == null)
            {
                return NotFound();
            }

            return friendship.ToList();
        }

        // GET: api/Friendships/1/2
        [HttpGet("{id_1}/{id_2}")]
        public ActionResult<Friendship> GetFriendship(int id_1, int id_2)
        {
            var friendship = _repository.GetFriendship(id_1, id_2);

            if (friendship == null)
            {
                return NotFound();
            }

            return friendship;
        }



        // POST: api/Friendships
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<Friendship> Create([FromBody] Friendship friendship)
        {
            try
            {
                _repository.Create(friendship);
            }
            catch (Exception)
            {
                if (_repository.GetFriendship(friendship.UserId_1, friendship.UserId_2) != null)
                  return  Conflict();
                else
                    throw;
            }
            
           
            return CreatedAtAction("GetFriendship", new { id_1 = friendship.UserId_1, id_2 = friendship.UserId_2 }, friendship);
        }

        // DELETE: api/Friendships/5/5
        
        [HttpDelete("{id_1}/{id_2}")]
        public ActionResult<Friendship> DeleteFriendship(int id_1, int id_2)
        {
            var friendship = _repository.GetFriendship(id_1, id_2);
            if (friendship == null)
            {
                return NotFound();
            }

            _repository.Remove(friendship);
            return friendship;
        }

        
    }
}
