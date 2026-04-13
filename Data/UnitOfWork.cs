using Core.Abstracts;
using Core.Abstracts.IRepositories;
using Data.Contexts;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UnitOfWork(ApplicationContext context) : IUnitOfWork
    {
        private ICustomerRepository? customers;
        public ICustomerRepository Customers => customers ??= new CustomerRepository(context);

        private ILeadRepository? leads;
        public ILeadRepository Leads => leads ??= new LeadRepository(context);

        private IActivityRepository? activities;
        public IActivityRepository Activities => activities ??= new ActivityRepository(context);

        private IContactRepository? contacts;
        public IContactRepository Contacts => contacts ??= new ContactRepository(context);

        private IOpportunityRepository? opportunities;
        public IOpportunityRepository Opportunities => opportunities ??= new OpportunityRepository(context);

        public async Task<bool> CommitAsync()
        {
            try
            {
                return (await context.SaveChangesAsync()) > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await context.DisposeAsync();
        }
    }
}
