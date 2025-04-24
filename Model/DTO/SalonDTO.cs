using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.DTO
{
    public class SalonDTO : BaseDTO
    {
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public string SalonName { get; set; }
        public string SalonAddress { get; set; }
        public string Phone { get; set; }
        public int BeauticianCount { get; set; }
        public string Contact { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public string SmallPicture { get; set; }
        public string QrCodePicture { get; set; }

        public List<SalonCloseDTO> SalonCloses { get; set; }
    }
}
