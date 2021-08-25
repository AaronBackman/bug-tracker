using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;

namespace bug_tracker.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectRepository projectRepository;
        private readonly ILogger<UserController> _logger;

        public ProjectController(ILogger<UserController> logger)
        {
            _logger = logger;
            this.projectRepository = new ProjectRepository();
        }

        [HttpPost]
        public IActionResult Post(Project project) {
            Console.WriteLine("post");
            // later add checks to prevent duplicate id
            projectRepository.Add(project, HttpContext.Request.Query["email"].ToString());
            return Ok(project);
        }
        

        [HttpPut("{guid}")]
        public IActionResult Update(Project updatedProject, Guid guid) {
            Console.WriteLine("put");

            if (updatedProject.ProjectGUID != guid) {
                return Unauthorized();
            }

            projectRepository.Put(updatedProject, HttpContext.Request.Query["email"].ToString());

            return NoContent();
        }

        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid) {
            Console.WriteLine("delete");

            projectRepository.Delete(guid, HttpContext.Request.Query["email"].ToString());

            return NoContent();
        }

        [HttpGet]
        public IActionResult Get()
        {

            Console.WriteLine("get");
            
            List<Project> projects = projectRepository.GetAll(HttpContext.Request.Query["email"].ToString());

            return Ok(projects);
        }
    }
}
