using CloudSalon.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.DTO
{
    public class EmployeeDTO : BaseDTO
    {
        public int EmployeeId;
        //public string UserName;
        public string PassWord;
        public string NickName;
        //public int UserTypeId;
        public UserTypeEnum UserTypeId;
        public string Picture;
        public string SmallPicture;
        public bool IsDayoffMon;
        public bool IsDayoffTue;
        public bool IsDayoffWeb;
        public bool IsDayoffThu;
        public bool IsDayoffFri;
        public bool IsDayoffSat;
        public bool IsDayoffSun;
        public string Mobile;
        public int TodayAppointmentCount;
        public int MonthCompletedAppointmentCount;
        public DateTime CreatedDate;
        public string Name;
        public string Description;
        public bool? IsBeautician;
    }
}
