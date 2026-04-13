using Core.Abstracts.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Abstracts
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        ICustomerRepository Customers { get; }
        ILeadRepository Leads { get; }
        IActivityRepository Activities { get; }
        IContactRepository Contacts { get; }
        IOpportunityRepository Opportunities { get; }

        Task<bool> CommitAsync();
    }
}
