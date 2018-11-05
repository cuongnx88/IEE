using IEE.Infrastructure.DbContext;

namespace IEE.Infrastructure
{
    public class DatabaseFactory :  IDatabaseFactory
    {
        public SATEntities dataContext;
        public DatabaseFactory()
        {
            dataContext = new SATEntities();
        }
        public SATEntities Get()
        {
            return dataContext ?? (dataContext = new SATEntities());
        }
      
    }
}
