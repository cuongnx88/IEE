using IEE.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace IEE.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly IDatabaseFactory databaseFactory;
        public SATEntities dataContext;
        private bool disposed;
        public UnitOfWork(IDatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
            dataContext = this.databaseFactory.Get();
        }

        public UnitOfWork()
        {
            this.databaseFactory = new DatabaseFactory();
            dataContext = this.databaseFactory.Get();
        }
        public SATEntities DataContext
        {
            get { return dataContext ?? (dataContext = databaseFactory.Get()); }
        }

        public void Commit()
        {
            DataContext.SaveChanges();
        }

        public DbSet<T> CreateSet<T>()
           where T : class
        {
            return DataContext.Set<T>();
        }

        public IEnumerable<T> ExecuteQuery<T>(string sqlQuery, params object[] parameters)
        {
            return DataContext.Database.SqlQuery<T>(sqlQuery, parameters);
        }

        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return DataContext.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }
       
        public IRepository<T> GetRepository<T>() where T : class
        {
            return new IEE.Infrastructure.Base.Repository<T>(dataContext);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dataContext.Dispose();
                    disposed = true;
                }
            }

            disposed = false;
        }
    }
}
