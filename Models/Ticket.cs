using System;
using System.Collections.Generic;

namespace bug_tracker
{
    public class Ticket
    {
        public string Name {get; set;}
        public int TicketId {get; set;}
        public int ProjectId {get; set;}
        public string Description {get; set;}
        public DateTime DateCreated {get; set;}
        public string Creator {get; set;}
        public int priority {get; set;}
        public bool IsCompleted {get; set;}
        public List<TicketHistory> EditHistory {get; set;}
    }
}
