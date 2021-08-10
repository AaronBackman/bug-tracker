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
            Random rng = new Random();
            Console.WriteLine("post");
            // later add checks to prevent duplicate id
            project.ProjectId = rng.Next(1, 1000000);
            projectRepository.Add(project);
            return Ok(project);
        }
        

        [HttpPut("{id}")]
        public IActionResult Update(Project updatedProject, int id) {
            Console.WriteLine("put");

            if (updatedProject.ProjectId != id) {
                return Unauthorized();
            }

            projectRepository.Put(updatedProject);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Project deletedProject, int id) {
            Console.WriteLine("put");

            if (deletedProject.ProjectId != id) {
                return Unauthorized();
            }

            projectRepository.Delete(deletedProject);

            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            Console.WriteLine("get");
            
            Project project = projectRepository.GetById(id);

            if (project == null) {
                return NotFound();
            }

            return Ok(project);
        }
    }
}
