using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;

namespace bug_tracker.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private ArrayList Users = new ArrayList() {
            new User {Username = "plane", Nickname = "pale", Projects = new List<int?>(){23, 56}}
        };

        private readonly ILogger<UserController> _logger;
        private readonly UserRepository userTestRepository;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
            this.userTestRepository = new UserRepository();
        }

        [HttpPost]
        public IActionResult Post(User user) {
            userTestRepository.Add(user);
            return Ok(user);
        }
        

        [HttpPut]
        public IActionResult Update(User user) {
            Console.WriteLine("put");

            userTestRepository.Put(user);
            return NoContent();

            // updated resource does not exist
            // return BadRequest("Updated resource does not exist");
        }

        [HttpDelete]
        public IActionResult Delete(User user) {
            Console.WriteLine("delete");

            userTestRepository.Delete(user);
            return NoContent();
        }

        [HttpGet]
        public IActionResult Get(User user)
        {

            Console.WriteLine("get");
            
            User QueriedUser = userTestRepository.GetByUsername(user.Username);

            if (QueriedUser == null) {
                return NotFound();
            }

            return Ok(QueriedUser);
        }
    }
}
