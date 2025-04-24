using CloudSalon.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CloudSalon.Model.Enum;

namespace CloudSalon.DAL
{
    public class PurchasedServiceDAL : BaseDAL<PurchasedService>
    {
        public PurchasedServiceDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public PurchasedService GetPurchasedService(int purchasedServiceId, int? salonId, int? userId, params ConsumedServiceStatusEnum[] status)
        {
            IQueryable<PurchasedService> list = dbContext.Query<PurchasedService>();
            list = list.Include(ps => ps.ServiceSnapShot);
            list = list.Include("ConsumedServiceDetails.ConsumedService.Employee");
            list = list.Where(ps => ps.PurchasedServiceId == purchasedServiceId);

            if (salonId.HasValue)
                list = list.Where(ps => ps.ServiceSnapShot.Service.SalonId == salonId);
            else
                list = list.Where(ps => ps.UserId == userId);

            return list.FirstOrDefault();
        }


        public List<PurchasedService> GetPurchasedServicesByUserId(int userId, int pageNumber, int pageSize, bool? isAvai)
        {
            IQueryable<PurchasedService> list = dbContext.Query<PurchasedService>();

            list = list.Include(ps => ps.ServiceSnapShot);
            list = list.Include("ConsumedServiceDetails.ConsumedService.Employee");

            list = list.Where(ps => ps.UserId == userId);


            //isAvai为空：取全部；为true：取可用的，即有剩余次数的；为false：取不可用的，即没有剩余次数的
            if (isAvai.HasValue)
            {
                if (isAvai.Value == true)
                    list = list.Where(ps => ps.Time.HasValue == false || ps.ConsumedServiceDetails.Count == 0 || ps.Time - ps.ConsumedServiceDetails.Sum(csd => csd.Time) > 0);
                else
                    list = list.Where(ps => ps.Time.HasValue == true && ps.Time - ps.ConsumedServiceDetails.Sum(csd => csd.Time) == 0);
            }

            return list.OrderByDescending(ps => ps.PurchasedServiceId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
        }

        public int GetPurchasedServiceCount(int userId)
        {
            return dbContext.Query<PurchasedService>().Where(ps => ps.UserId == userId).Count();
        }
    }
}
