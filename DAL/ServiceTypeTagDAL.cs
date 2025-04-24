using CloudSalon.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CloudSalon.DAL
{
    public class ServiceTypeTagDAL : BaseDAL<ServiceTypeTag>
    {
        public ServiceTypeTagDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public List<ServiceTypeTag> GetServiceTypeTags(int serviceTypeId)
        {
            return this.dbContext.Query<ServiceTypeTag>().Where(t => t.ServiceTypeId == serviceTypeId).ToList();
        }
    }
}
