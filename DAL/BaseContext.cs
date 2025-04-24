using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.DAL
{
    public class BaseContext : DbContext
    {
        public BaseContext(string str) : base(str) { }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class
        {
            var returnEntity = this.Set<TEntity>().Add(entity);
            this.SaveChanges();
            return returnEntity;
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return this.Set<TEntity>();
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            this.SaveChanges();
        }

        public TEntity Get<TEntity>(params object[] keyValues) where TEntity : class
        {
            return this.Set<TEntity>().Find(keyValues);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            this.Set<TEntity>().Remove(entity);
            this.SaveChanges();
        }

        public void Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            this.Set<TEntity>().RemoveRange(entities);
        }
    }
}
