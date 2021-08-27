using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;

namespace bug_tracker.Controllers
{
    [ApiController]
    [Route("api/projects/{project_guid}/tickets/{ticket_guid}/history")]
    public class TicketHistoryController : ControllerBase
    {
        private readonly TicketHistoryRepository ticketHistoryRepository;
        private readonly ILogger<UserController> _logger;

        public TicketHistoryController(ILogger<UserController> logger)
        {
            _logger = logger;
            this.ticketHistoryRepository = new TicketHistoryRepository();
        }

        [HttpGet]
        public IActionResult Get(Guid project_guid, Guid ticket_guid)
        {

            Console.WriteLine("get");
            
            List<TicketHistory> ticketHistories = ticketHistoryRepository.GetAll(project_guid, ticket_guid, HttpContext.Request.Query["email"].ToString());

            return Ok(ticketHistories);
        }
    }
}