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
    public class TagDAL : BaseDAL<Tag>
    {
        public TagDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public List<string> GetTags(string target)
        {
            return this.dbContext.Query<Tag>().Where(t => t.Target.StartsWith(target)).Select(t => t.TagName).Distinct().ToList();
        }

        public void DeleteTagsByTarget(string target)
        {
            var entities = this.dbContext.Query<Tag>().Where(t => t.Target == target).ToList();
            this.dbContext.Delete<Tag>(entities);
        }
        
    }
}
