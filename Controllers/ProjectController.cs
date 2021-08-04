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
        private ArrayList Projects = new ArrayList() {
            new Project {ProjectCreator = "foo", ProjectName = "qwerty", ProjectMembers = new List<ProjectMember>() {new ProjectMember {Username = "jellyfish", NickName = "jelly", Role = Role.Developer}}, ProjectId = 53}
        };

        private readonly ILogger<UserController> _logger;

        public ProjectController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post(Project project) {
            Random rng = new Random();
            Console.WriteLine("post");
            // later add checks to prevent duplicate id
            project.ProjectId = rng.Next(1, 1000000);
            Projects.Add(project);
            return Ok(project);
        }
        

        [HttpPut("{id}")]
        public IActionResult Update(Project updatedProject, int id) {
            Console.WriteLine("put");

            if (updatedProject.ProjectId != id) {
                return Unauthorized();
            }

            for (int i = 0; i < Projects.Count; i++) {
                Project project = (Project) Projects[i];

                if (string.Equals(project.ProjectId, id)) {
                    // update the resource
                    Projects[i] = updatedProject;
                    return Ok(updatedProject);
                }
            }

            // updated resource does not exist
            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            Console.WriteLine("get");
            Console.WriteLine(id);
            for (int i = 0; i < Projects.Count; i++) {
                Project oldProject = (Project) Projects[i];

                Console.WriteLine(oldProject.ProjectId);

                if (string.Equals(oldProject.ProjectId, id)) {
                    return Ok(oldProject);
                }
            }

            return NoContent();
        }
    }
}
