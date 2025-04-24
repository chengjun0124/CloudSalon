using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.DTO
{
    public class JWT
    {
        public int UserId;
        public int UserTypeId;
        public int SalonId;
        public bool? IsBeautician;
        public string IdentityCode;
        public long Expire;
    }
}
