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
    public class ServiceDAL : BaseDAL<Service>
    {
        public ServiceDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public Service GetService(int serviceId, int salonId, bool isGetTags, bool isGetServiceType, bool isGetSalon)
        {
            IQueryable<Service> list = dbContext.Query<Service>();

            list = list.Include(s => s.EffectImages);
            if (isGetTags)
                list = list.Include(s => s.FunctionalityTags);

            if (isGetServiceType)
                list = list.Include(s => s.ServiceType);

            if (isGetSalon)
                list = list.Include(s => s.Salon);

            return list
                .Where(s => s.ServiceId == serviceId && s.SalonId == salonId && s.IsDeleted == false).FirstOrDefault();
        }

        public List<ServiceType> GetServiceTypes()
        {
            return dbContext.Query<ServiceType>().ToList();
        }

        public List<ServiceType> GetServiceTypesHasService(int salinId)
        {
            return dbContext.Query<ServiceType>().Where(st => st.Services.Where(s => s.IsDeleted == false && s.SalonId == salinId).Count() > 0).ToList();
        }

        public List<Service> GetServices(int salonId, int pageNumber, int pageSize, int? serviceTypeId, bool isGetServiceType, bool isGetEffectImage, bool isGetSalon, bool isGetTags)
        {
            IQueryable<Service> list = dbContext.Query<Service>();

            if(isGetServiceType)
                list = list.Include(s => s.ServiceType);

            if (isGetSalon)
                list = list.Include(s => s.Salon);

            if (isGetTags)
                list = list.Include(s => s.FunctionalityTags);

            if (isGetEffectImage)
                list = list.Include(s => s.EffectImages);

            if (serviceTypeId != null)
                list = list.Where(s => s.ServiceTypeId == serviceTypeId.Value);

            

            return list
                .Where(s => s.SalonId == salonId && s.IsDeleted == false)
                .OrderByDescending(s => s.Seq)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
        }

        public List<Service> GetHotServices(int top,int salonId) 
        {

            IQueryable<Service> list = dbContext.Query<Service>();

            list = list.Include(s => s.ServiceType);

            return list
                .Where(s => s.IsDeleted == false && s.SalonId == salonId)
                //.OrderByDescending(s => s.Appointments.Count())
                .OrderByDescending(s => s.ServiceSnapShots.Sum(sss=>sss.Appointments.Count()))
                .Take(top).ToList();

        }

        public int GetServiceCount(int salonId)
        {
            return dbContext.Query<Service>()
                           .Where(s => s.SalonId == salonId && s.IsDeleted == false).Count();
        }
    }
}
