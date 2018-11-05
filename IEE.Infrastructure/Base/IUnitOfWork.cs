using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace IEE.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        void Commit();
        DbSet<T> CreateSet<T>() where T : class;
        IEnumerable<T> ExecuteQuery<T>(string sqlQuery, params object[] parameters);
        int ExecuteCommand(string sqlCommand, params object[] parameters);
    }
}
