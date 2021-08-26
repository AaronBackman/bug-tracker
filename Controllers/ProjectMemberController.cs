using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;

namespace bug_tracker.Controllers
{
    [ApiController]
    [Route("api/projects/{project_guid}/members")]
    public class ProjectMemberController : ControllerBase
    {
        private readonly ProjectMemberRepository projectMemberRepository;
        private readonly ILogger<UserController> _logger;

        public ProjectMemberController(ILogger<UserController> logger)
        {
            _logger = logger;
            this.projectMemberRepository = new ProjectMemberRepository();
        }

        [HttpPost]
        public IActionResult Post(ProjectMember projectMember, Guid project_guid) {
            Console.WriteLine("post");
            Console.WriteLine(projectMember.ProjectRole);
            // later add checks to prevent duplicate id
            ProjectMember newProjectMember = projectMemberRepository.Add(projectMember, project_guid, HttpContext.Request.Query["email"].ToString(), false);
            return Ok(newProjectMember);
        }
        

        [HttpPut("{guid}")]
        public IActionResult Update(ProjectMember updatedProjectMember, Guid project_guid, Guid guid) {
            Console.WriteLine("put");

            if (updatedProjectMember.ProjectMemberGUID != guid) {
                return Unauthorized();
            }

            projectMemberRepository.Put(updatedProjectMember, project_guid, HttpContext.Request.Query["email"].ToString());

            return NoContent();
        }

        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid, Guid project_guid) {
            Console.WriteLine("delete");

            projectMemberRepository.Delete(guid, project_guid, HttpContext.Request.Query["email"].ToString());

            return NoContent();
        }

        [HttpGet]
        public IActionResult Get(Guid project_guid)
        {

            Console.WriteLine("get");
            
            List<ProjectMember> projectMembers = projectMemberRepository.GetAll(project_guid, HttpContext.Request.Query["email"].ToString());

            return Ok(projectMembers);
        }
    }
}
