using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.DTO
{
    public class UserDTO : BaseDTO
    {
        public int UserId { get; set; }
        public string Mobile { get; set; }
        public string NickName { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Memo { get; set; }
        public int PurchasedServiceCount { get; set; }
        public int RemainPurchasedServiceCount { get; set; }
        public int RemainTime { get; set; } 
        public DateTime? LastConsumedDate { get; set; }
    }
}
