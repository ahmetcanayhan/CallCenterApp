using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Data.Contexts;
using Utilities.Generics;

namespace Data.Repositories
{
    public class OpportunityRepository(ApplicationContext context) : Repository<Opportunity>(context), IOpportunityRepository { }
}
