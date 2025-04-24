using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.DTO
{
    public class DateAndCount : BaseDTO
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
