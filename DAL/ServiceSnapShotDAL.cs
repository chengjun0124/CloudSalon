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
    public class ServiceSnapShotDAL : BaseDAL<ServiceSnapShot>
    {
        public ServiceSnapShotDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }       
    }
}
