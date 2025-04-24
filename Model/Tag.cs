using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class Tag : BaseModel
    {
        public string TagName { get; set; }
        public string Target { get; set; }
    }

    public class TagConfiguration : EntityTypeConfiguration<Tag>
    {
        public TagConfiguration()
        {
            this.HasKey(t => new { t.TagName, t.Target });
        }
    }
}
