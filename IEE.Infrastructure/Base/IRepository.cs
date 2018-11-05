using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using IEE.Infrastructure.Specification.Contract;
using System.Data.Entity;

namespace IEE.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        DbSet<T> CreateSet();
        void Insert(T entity);
        void InsertAndSubmit(T entity);
        void Update(T entity);
        void Update(T entity, bool updateRootEntity);
        void UpdateAndSubmit(T entity);
        void UpdateAndSubmit(T entity, bool updateRootEntity);
        void Delete(T entity);
        void Delete(object id);
        void Delete(Expression<Func<T, bool>> where);
        void DeleteAndSubmit(T entity);
        void SoftDeleteAndSubmit(T entity);
        T GetById(int id);
        T GetById(string id);
        T Get(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        IEnumerable<T> GetTop(int top);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> filter = null,
                                               Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                               string includeProperties = "",
                                               int? skip = null,
                                               int? take = null);
        IEnumerable<T> GetPage<KProperty>(int pageIndex, int pageCount, System.Linq.Expressions.Expression<Func<T, KProperty>> orderByExpression, bool ascending);
        int ExecuteCommand(string command, params object[] parameters);
        IEnumerable<T> ExecuteQuery(string sqlQuery, params object[] parameters);
        void SaveChanges();
        void CommitAndRefreshChanges();
        IEnumerable<T> AllMatching(ISpecification<T> specification);
        void SetModified(T item);
        void Merge(T original, T current);
        void RollbackChanges();
    }
}
