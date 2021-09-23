using System;
using System.Collections.Generic;

// unused at the moment

namespace bug_tracker
{
    public interface ITicketHistoryRepository
    {
        public List<TicketHistory> GetAll(Guid projectGuid, Guid ticketGuid, string email);
        public void Add(TicketHistory ticketHistory, Guid projectGuid, Guid ticketGuid, string email);
        public void DeleteAll(Guid ticketGuid, Guid projectGuid, string email);
    }
}