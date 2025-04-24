using CloudSalon.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Collections;

namespace CloudSalon.DAL
{
    public class PredefinedTagDAL : BaseDAL<PredefinedTag>
    {
        public PredefinedTagDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public List<string> GetPredefinedTags(string target)
        {
            return this.dbContext.Query<PredefinedTag>().Where(t => t.Target.StartsWith(target)).Select(t => t.TagName).Distinct().ToList();
        }
    }
}
