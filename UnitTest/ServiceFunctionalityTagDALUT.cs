using CloudSalon.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class ServiceFunctionalityTagDALUT
    {
        static SalonContext context = new SalonContext();
        static ServiceFunctionalityTagDAL serviceFunctionalityTagDAL = new ServiceFunctionalityTagDAL(context);

        public static void GetServiceFunctionalityTags()
        {
            var a = serviceFunctionalityTagDAL.GetServiceFunctionalityTags(5);
        }
    }
}
