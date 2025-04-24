using CloudSalon.DAL;
using CloudSalon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class TagDALUT
    {
        static SalonContext content = new SalonContext();
        static CloudSalon.DAL.TagDAL dal = new CloudSalon.DAL.TagDAL(content);        

        public static void GetTags()
        {
            var a = dal.GetTags("Service[5]-Functionality");
        }

        public static void DeleteTagsByTarget()
        {
            dal.Insert(new Tag() { Target = "test", TagName = "tag1" });
            dal.Insert(new Tag() { Target = "test", TagName = "tag2" });

            dal.DeleteTagsByTarget("test");

            content.SaveChanges();
        }
    }
}
