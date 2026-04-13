using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Data.Contexts;
using Utilities.Generics;

namespace Data.Repositories
{
    public class LeadRepository(ApplicationContext context) : Repository<Lead>(context), ILeadRepository  {    }
}
