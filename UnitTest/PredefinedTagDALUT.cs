using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSalon.DAL;

namespace UnitTest
{
    public class PredefinedTagDALUT
    {
        static PredefinedTagDAL dal = new PredefinedTagDAL(new SalonContext());

        public static void GetPredefinedTags()
        {
            var a = dal.GetPredefinedTags("Service-ServiceType[2]");
        } 
    }
}
