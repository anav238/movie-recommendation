﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_recommendation.Data;
using movie_recommendation.Entities;

namespace movie_recommendation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _repository;

        public UsersController(IRepository<User> repository)
        {
            _repository = repository;
        }

        // GET: api/Users
        [HttpGet]
        public  ActionResult<IEnumerable<User>> GetUsers()
        {
            return _repository.GetAll().ToList();
        }

        // GET: api/Users/5
        
        [HttpGet("{id}")]
        public  ActionResult<User> GetById(int id)
        {
            var user =  _repository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<User> PostUser(User user)
        {
            _repository.Create(user);
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public ActionResult<User> DeleteUser(int id)
        {
            var user = _repository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            _repository.Remove(user);     

            return user;
        }

        
    }
}