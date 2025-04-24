using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.DAL
{
    public class BaseDAL<TEntity> where TEntity : class
    {
        protected BaseContext dbContext { get; set; }

        public TEntity Get(params object[] keyValues)
        {
            return dbContext.Get<TEntity>(keyValues);
        }

        public void Update(TEntity entity)
        {
            dbContext.Update<TEntity>(entity);
        }

        public TEntity Insert(TEntity entity)
        {
            return dbContext.Insert<TEntity>(entity);
        }

        public void Delete(TEntity entity)
        {
            dbContext.Delete<TEntity>(entity);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            dbContext.Delete<TEntity>(entities);
        }
    }
}
