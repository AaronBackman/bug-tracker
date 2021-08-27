using System;
using System.Collections.Generic;

namespace bug_tracker
{
    public interface ITicketRepository
    {
        public List<Ticket> GetAll(Guid projectGuid, string email);
        public Ticket Add(Ticket ticket, Guid projectGuid, string email);

        public void Put(Ticket ticket, Guid ticketGuid, Guid projectGuid, string email);
        public void Delete(Guid ticketGuid, Guid projectGuid, string email);
    }
}