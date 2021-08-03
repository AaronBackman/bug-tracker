using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;

namespace bug_tracker.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private ArrayList Users = new ArrayList() {
            new User {Username = "plane", Nickname = "pale"}
        };

        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post(User user) {
            Console.Write("post");
            Users.Add(user);
            return Ok(user);
        }
        

        [HttpPut("{username}")]
        public IActionResult Update(string username, User newUser) {
            Console.Write("put");

            for (int i = 0; i < Users.Count; i++) {
                User user = (User) Users[i];

                if (string.Equals(user.Username, username)) {
                    // update the resource
                    Users.Add(newUser);
                    return Ok(newUser);
                }
            }

            // updated resource does not exist
            return NoContent();
        }

        [HttpGet("{username}")]
        public IActionResult Get(string username)
        {

            Console.Write("get");
            Console.Write(username);
            for (int i = 0; i < Users.Count; i++) {
                User user = (User) Users[i];

                Console.Write(user.Username);

                if (string.Equals(user.Username, username)) {
                    return Ok(user);
                }
            }

            return NoContent();
        }
    }
}
