using CloudSalon.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.DAL
{
    public class ServiceEffectImageDAL : BaseDAL<ServiceEffectImage>
    {
        public ServiceEffectImageDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }
    }
}
