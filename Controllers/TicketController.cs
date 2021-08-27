using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;

namespace bug_tracker.Controllers
{
    [ApiController]
    [Route("api/projects/{project_guid}/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly TicketRepository ticketRepository;
        private readonly TicketHistoryRepository ticketHistoryRepository;
        private readonly ILogger<UserController> _logger;

        public TicketController(ILogger<UserController> logger)
        {
            _logger = logger;
            this.ticketRepository = new TicketRepository();
            this.ticketHistoryRepository = new TicketHistoryRepository();
        }

        [HttpPost]
        public IActionResult Post(Ticket ticket, Guid project_guid) {
            Console.WriteLine("post");
            // later add checks to prevent duplicate id
            Ticket newTicket = ticketRepository.Add(ticket, project_guid, HttpContext.Request.Query["email"].ToString());
            return Ok(newTicket);
        }
        

        [HttpPut("{guid}")]
        public IActionResult Update(Ticket updatedTicket, Guid project_guid, Guid ticketGuid) {
            Console.WriteLine("put");

            if (updatedTicket.TicketGUID != ticketGuid) {
                return Unauthorized();
            }

            ticketRepository.Put(updatedTicket, ticketGuid, project_guid, HttpContext.Request.Query["email"].ToString());

            return NoContent();
        }

        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid ticketGuid, Guid project_guid) {
            Console.WriteLine("delete");

            ticketRepository.Delete(ticketGuid, project_guid, HttpContext.Request.Query["email"].ToString());

            return NoContent();
        }

        [HttpGet]
        public IActionResult Get(Guid project_guid)
        {

            Console.WriteLine("get");
            
            List<Ticket> tickets = ticketRepository.GetAll(project_guid, HttpContext.Request.Query["email"].ToString());

            return Ok(tickets);
        }
    }
}
