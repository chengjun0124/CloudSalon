using CloudSalon.API.Controllers;
using CloudSalon.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceTypeTagDALUT.GetServiceTypeTags();
            ServiceFunctionalityTagDALUT.GetServiceFunctionalityTags();

            
            
        }
    }
}
