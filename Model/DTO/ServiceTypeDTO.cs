using CloudSalon.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.DTO
{
    public class ServiceTypeDTO : BaseDTO
    {
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }
    }
}
