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
        

        [HttpPut("{email}")]
        public IActionResult Update(User user, string email) {
            Console.WriteLine("put");

            userTestRepository.Put(user);
            return NoContent();

            // updated resource does not exist
            // return BadRequest("Updated resource does not exist");
        }

        [HttpDelete("{email}")]
        public IActionResult Delete(string email) {
            Console.WriteLine("delete");

            userTestRepository.Delete(email);
            return NoContent();
        }

        [HttpGet("{email}")]
        public IActionResult Get(string email)
        {

            Console.WriteLine("get");
            
            User QueriedUser = userTestRepository.GetByEmail(email);

            if (QueriedUser == null) {
                return NotFound();
            }

            return Ok(QueriedUser);
        }
    }
}
