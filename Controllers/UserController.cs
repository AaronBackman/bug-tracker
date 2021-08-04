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
            new User {Username = "plane", Nickname = "pale", Projects = new List<int>(){23, 56}}
        };

        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post(User user) {
            Console.WriteLine("post");
            user.Projects = new List<int>();
            Users.Add(user);
            return Ok(user);
        }
        

        [HttpPut]
        public IActionResult Update(User newUser) {
            Console.Write("put");

            for (int i = 0; i < Users.Count; i++) {
                User user = (User) Users[i];

                if (string.Equals(user.Username, newUser.Username)) {
                    // update the resource
                    Users[i] = newUser;
                    return Ok(newUser);
                }
            }

            // updated resource does not exist
            return NoContent();
        }

        [HttpGet]
        public IActionResult Get(User user)
        {

            Console.Write("get");
            Console.Write(user.Username);
            for (int i = 0; i < Users.Count; i++) {
                User oldUser = (User) Users[i];

                Console.Write(oldUser.Username);

                if (string.Equals(oldUser.Username, user.Username)) {
                    return Ok(oldUser);
                }
            }

            return NoContent();
        }
    }
}
