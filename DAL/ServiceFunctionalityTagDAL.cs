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
    public class ServiceFunctionalityTagDAL : BaseDAL<ServiceFunctionalityTag>
    {
        public ServiceFunctionalityTagDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public List<ServiceFunctionalityTag> GetServiceFunctionalityTags(int serviceId)
        {
            return this.dbContext.Query<ServiceFunctionalityTag>().Where(t => t.ServiceId == serviceId).ToList();
        }


        public void ServiceFunctionalityTags(IEnumerable<ServiceFunctionalityTag> entities)
        {
            this.dbContext.Delete(entities);
        }
    }
}
