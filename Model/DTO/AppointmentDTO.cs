using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace CloudSalon.Model.DTO
{
    
    public class AppointmentDTO : BaseDTO
    {
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int ServiceId{ get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? EmployeeId { get; set; }
        public string ServiceName { get; set; }
        public string NickName{ get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mobile { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int AppointmentStatusId { get; set; }
        public int AppointmentId { get; set; }
        public string SalonPhone { get; set; }
        public string UserNickName { get; set; }
        public string UserPicture { get; set; }
        public string EmployeePicture { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
