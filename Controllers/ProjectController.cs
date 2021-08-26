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
        private readonly ProjectMemberRepository projectMemberRepository;
        private readonly ILogger<UserController> _logger;

        public ProjectController(ILogger<UserController> logger)
        {
            _logger = logger;
            this.projectRepository = new ProjectRepository();
            this.projectMemberRepository = new ProjectMemberRepository();
        }

        [HttpPost]
        public IActionResult Post(Project project) {
            Console.WriteLine("post");
            string email = HttpContext.Request.Query["email"].ToString();

            Project newProject = projectRepository.Add(project, email);

            ProjectMember projectMember = new ProjectMember();
            projectMember.Email = email;
            projectMember.ProjectRole = Role.Owner;
            projectMemberRepository.Add(projectMember, newProject.ProjectGUID, email, true);
            return Ok(newProject);
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
