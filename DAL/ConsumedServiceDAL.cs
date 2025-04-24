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
    public class ConsumedServiceDAL : BaseDAL<ConsumedService>
    {
        public ConsumedServiceDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public ConsumedService GetConsumedService(int consumedServiceId, UserTypeEnum userType, int id, bool isGetConsumedServiceDetails, bool isGetEmployee, bool isGetService, bool isGetAppointment, bool isGetUser, bool isGetAppointmentFlows)
        {
            IQueryable<ConsumedService> list = dbContext.Query<ConsumedService>();

            if (isGetConsumedServiceDetails)
                list = list.Include(cs => cs.ConsumedServiceDetails);

            if (isGetEmployee)
                list = list.Include(cs => cs.Employee);

            if (isGetService)
                list = list.Include("ConsumedServiceDetails.PurchasedService.ServiceSnapShot.Service");

            if (isGetAppointment)
                list = list.Include(cs => cs.Appointment);

            if(isGetUser)
                list = list.Include(cs => cs.User);

            if(isGetAppointmentFlows)
                list = list.Include("Appointment.AppointmentFlows");

            if (userType == UserTypeEnum.User)
                return list.Where(cs => cs.ConsumedServiceId == consumedServiceId && cs.UserId == id).FirstOrDefault();
            else
                return list.Where(cs => cs.ConsumedServiceId == consumedServiceId && cs.Employee.SalonId == id).FirstOrDefault();
        }

        public List<ConsumedService> GetUnConfirmedConsumedServices(int userId, bool isGetConsumedServiceDetails, bool isGetEmployee, bool isGetService,bool isGetAppointment)
        {
            IQueryable<ConsumedService> list = dbContext.Query<ConsumedService>();

            if (isGetConsumedServiceDetails)
                list = list.Include(cs => cs.ConsumedServiceDetails);

            if(isGetEmployee)
                list = list.Include(cs => cs.Employee);

            if(isGetService)
                list = list.Include("ConsumedServiceDetails.PurchasedService.ServiceSnapShot.Service");

            if(isGetAppointment)
                list = list.Include(cs => cs.Appointment);

            return list.Where(cs => cs.UserId == userId && cs.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.NeedConfirm).ToList();
        }

        public List<ConsumedService> GetConsumedServices(int? salonId, int? employeeId, DateTime? date, int pageNumber, int pageSize)
        {
            IQueryable<ConsumedService> list = dbContext.Query<ConsumedService>();
            list = list.Include(cs => cs.Employee);
            list = list.Include("ConsumedServiceDetails.PurchasedService.ServiceSnapShot.Service");

            if (date.HasValue)
            {
                DateTime end = date.Value.AddDays(1);
                list = list.Where(cs => cs.CreatedDate >= date && cs.CreatedDate < end);
            }

            if (salonId.HasValue)
                return list.Where(cs => cs.Employee.SalonId == salonId)
                    .OrderByDescending(cs => cs.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).ToList();
            else
                return list.Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(cs => cs.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
        }

        public List<int> GetConsumedServiceCount(DateTime start, DateTime end, int? salonId, int? employeeId)
        {
            end = end.AddDays(1);

            IQueryable<ConsumedService> list = this.dbContext.Query<ConsumedService>()
                .Where(cs => cs.CreatedDate >= start && cs.CreatedDate < end);

            if (salonId.HasValue)
                list = list.Where(a => a.Employee.SalonId == salonId);
            else
                list = list.Where(a => a.EmployeeId == employeeId);


            List<IGrouping<DateTime, ConsumedService>> group = list.ToList().GroupBy(cs => cs.CreatedDate.Date).ToList();

            DateTime date = start;
            IGrouping<DateTime, ConsumedService> g = null;
            List<int> count = new List<int>();
            while (date < end)
            {
                g = group.Find(cs => cs.Key == date);
                count.Add(g == null ? 0 : g.Count());

                date = date.AddDays(1);
            }

            return count;
        }

        public void Delete(ConsumedService cs)
        {
            this.dbContext.Delete<ConsumedServiceDetail>(cs.ConsumedServiceDetails);

            base.Delete(cs);
        }

        public void Delete(IEnumerable<ConsumedServiceDetail> entities)
        {
            this.dbContext.Delete<ConsumedServiceDetail>(entities);
        }

    }
}
