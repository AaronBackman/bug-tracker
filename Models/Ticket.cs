using System;
using System.Collections.Generic;

namespace bug_tracker
{
    public class Ticket
    {
        public string TicketName {get; set;}
        public Guid TicketGUID {get; set;}
        public string TicketDescription {get; set;}
        public DateTime DateCreated {get; set;}
        public string CreatorNickname {get; set;}
        public int TicketPriority {get; set;}
        public bool TicketCompleted {get; set;}
    }
}
