using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;

namespace bug_tracker.Controllers
{
    [ApiController]
    [Route("api/projects/{project_id}/tickets")]
    public class TicketController : ControllerBase
    {
        private ArrayList Tickets = new ArrayList() {
            new Ticket {Creator = "foo", Name = "qwerty", DateCreated = new DateTime(2008), ProjectId = 53, TicketId = 12}
        };

        private readonly ILogger<UserController> _logger;

        public TicketController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post(Ticket ticket, int project_id) {
            int projectId = project_id;

            Random rng = new Random();
            Console.WriteLine("post");
            // later add checks to prevent duplicate id
            ticket.TicketId = rng.Next(1, 1000000);
            Tickets.Add(ticket);
            return Ok(ticket);
        }
        

        [HttpPut("{id}")]
        public IActionResult Update(Ticket updatedTicket, int id) {
            Console.WriteLine("put");

            if (updatedTicket.ProjectId != id) {
                return Unauthorized();
            }

            for (int i = 0; i < Tickets.Count; i++) {
                Ticket ticket = (Ticket) Tickets[i];

                if (string.Equals(ticket.TicketId, id)) {
                    // update the resource
                    Tickets[i] = updatedTicket;
                    return Ok(updatedTicket);
                }
            }

            // updated resource does not exist
            return NoContent();
        }

        [HttpGet]
        public IActionResult Get(int project_id)
        {
            int projectId = project_id;

            Console.WriteLine("get");
            Console.WriteLine(projectId);
            return Ok(Tickets);
        }
    }
}