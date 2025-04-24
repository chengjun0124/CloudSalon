using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class PredefinedTag : BaseModel
    {
        public string TagName { get; set; }
        public string Target { get; set; }
    }

    public class PredefinedTagConfiguration : EntityTypeConfiguration<PredefinedTag>
    {
        public PredefinedTagConfiguration()
        {
            this.HasKey(t => new { t.TagName, t.Target });
        }
    }
}
