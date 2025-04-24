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
    public class SalonDAL : BaseDAL<Salon>
    {
        public SalonDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public Salon GetSalonByIdentityCode(string identityCode)
        {
            return dbContext.Query<Salon>().Where(s => s.IdentityCode == identityCode).FirstOrDefault();
        }

        public Salon GetSalonIdentityCodeBySalonId(int salonId)
        {
            return dbContext.Query<Salon>().Where(s => s.SalonId == salonId).FirstOrDefault();
        }

        public Salon Get(int salonId, bool isGetSalonCloses, bool isGetBeautician)
        {
            IQueryable<Salon> list = dbContext.Query<Salon>();

            if (isGetSalonCloses)
                list = list.Include(s => s.SalonCloses);

            if (isGetBeautician)
                list = list.Include(s => s.Employees);

            var salon = list.Where(s => s.SalonId == salonId).First();
            salon.SalonCloses = salon.SalonCloses.OrderBy(s => s.StartDate).ToList();
            return salon;
        }

        public void CreateSalonClose(SalonClose s)
        {
            dbContext.Insert<SalonClose>(s);
        }

        public void DeleteSalonClose(SalonClose s)
        {
            dbContext.Delete<SalonClose>(s);
        }

        public SalonClose GetSalonClose(int id,int salonId)
        {
            return dbContext.Query<SalonClose>().Where(s => s.Id == id && s.SalonId == salonId).FirstOrDefault();
        }
    }
}
