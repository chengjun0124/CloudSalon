using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.DTO
{
    public class AvaiAppointmentDTO : BaseDTO
    {
        public AvaiAppointmentDTO()
        {
            this.Beauticians = new List<Beautician>();
        }

        //public TimeSpan OpenTime;
        //public TimeSpan CloseTime;
        //public TimeSpan Interval;

        public List<Beautician> Beauticians;
    }

    public class Beautician
    {
        public Beautician()
        {
            this.AvaiDates = new List<AvaiDate>();
        }
        public int EmployeeId;
        public string NickName;
        public string Picture;
        public int ServedCount;

        public List<AvaiDate> AvaiDates;
    }

    public class AvaiDate
    {
        public AvaiDate()
        {
            this.AvaiTimes = new List<AvaiTime>();
        }
        public DateTime Date;
        public List<AvaiTime> AvaiTimes;
        public bool IsDayoff;
        public bool IsSalonClose;
    }

    public class AvaiTime
    {
        public TimeSpan Time;
        public bool IsAvailable;
    }
}
