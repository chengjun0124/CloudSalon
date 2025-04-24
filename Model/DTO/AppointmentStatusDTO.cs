using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CloudSalon.Model.Enum;


namespace CloudSalon.Model.DTO
{
    
    public class AppointmentStatusDTO : BaseDTO
    {
        public AppointmentStatusEnum ApponintmentStatus { get; set; }
        public int AppointmentId { get; set; }
    }
}
