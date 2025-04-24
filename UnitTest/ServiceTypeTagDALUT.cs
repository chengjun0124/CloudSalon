using CloudSalon.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class ServiceTypeTagDALUT
    {
        static SalonContext context = new SalonContext();
        static ServiceTypeTagDAL serviceTypeTagDAL = new ServiceTypeTagDAL(context);
        public static void GetServiceTypeTags()
        {
            var a = serviceTypeTagDAL.GetServiceTypeTags(1);
        }
    }
}
