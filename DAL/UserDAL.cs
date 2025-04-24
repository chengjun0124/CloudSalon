using CloudSalon.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CloudSalon.DAL
{
    public class UserDAL : BaseDAL<User>
    {
        public UserDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public User GetUserByConsumeCode(string consumeCode, int salonId, bool isGetConsumedServiceDetails, bool isGetServiceSnapShot, bool isGetConsumedService,bool isGetEmployee)
        {
            IQueryable<User> list = dbContext.Query<User>();

            if (isGetConsumedServiceDetails)
                list = list.Include("PurchasedServices.ConsumedServiceDetails");

            if (isGetServiceSnapShot)
                list = list.Include("PurchasedServices.ServiceSnapShot");
            
            if(isGetConsumedService)
                list = list.Include(u => u.ConsumedServices);

            if(isGetEmployee)
                list = list.Include("ConsumedServices.Employee");

            return list.Where(u => u.ConsumeCode == consumeCode && u.SalonId == salonId).FirstOrDefault();
        }

        public List<User> GetUsers(int salonId, int pageNumber, int pageSize, string keyword, bool isGetPurcharsedService, bool isGetConsumedServiceDetails, bool isGetConsumedServices)
        {
            IQueryable<User> list = dbContext.Query<User>();

            if (isGetPurcharsedService)
                list = list.Include(u => u.PurchasedServices);

            if (isGetConsumedServiceDetails)
                list = list.Include("PurchasedServices.ConsumedServiceDetails");

            if (isGetConsumedServices)
                list = list.Include(u => u.ConsumedServices);

            return list
                .Where(u => u.SalonId == salonId
                    && 
                    (
                        keyword == null ? 1 == 1 : u.Mobile.StartsWith(keyword)
                        ||
                        keyword == null ? 1 == 1 : u.NickName.Contains(keyword)
                        ||
                        keyword == null ? 1 == 1 : u.Name.Contains(keyword)
                    )
                )
                .OrderByDescending(ee => ee.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
        }

        public User GetUser(int userId, int salonId, bool isGetConsumedService, bool isGetServiceSnapShot)
        {
            IQueryable<User> list = dbContext.Query<User>();

            if (isGetConsumedService)
                list = list.Include("PurchasedServices.ConsumedServiceDetails");

            if (isGetServiceSnapShot)
                list = list.Include("PurchasedServices.ServiceSnapShot");

            return list
                .Where(u => u.SalonId == salonId && u.UserId == userId).FirstOrDefault();
        }

        public User GetUser(string mobile, int salonId, bool isGetLoginValidCode)
        {
            IQueryable<User> list = dbContext.Query<User>();

            if (isGetLoginValidCode)
                list = list.Include(u => u.LoginValidCodes);

            return list
                .Where(u => u.SalonId == salonId && u.Mobile == mobile).FirstOrDefault();
        }

        public bool IsExistedUser(string mobile, int salonId)
        {
            return dbContext.Query<User>().Where(u => u.Mobile == mobile && u.SalonId == salonId).Count() > 0;
        }

        public int GetUserCount(int salonId)
        {
            return dbContext.Query<User>()
                           .Where(u => u.SalonId == salonId).Count();
        }
    }
}
