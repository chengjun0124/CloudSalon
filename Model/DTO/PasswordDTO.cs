using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.DTO
{
    public class PasswordDTO : BaseDTO
    {
        public string OldPassword;
        public string NewPassword;
    }
}
