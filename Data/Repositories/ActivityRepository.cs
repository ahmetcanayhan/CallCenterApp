using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Data.Contexts;
using Utilities.Generics;

namespace Data.Repositories
{
    public class ActivityRepository(ApplicationContext context) : Repository<Activity>(context), IActivityRepository  {    }
}
