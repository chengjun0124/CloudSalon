using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalonAdmin
{
    public partial class _default : System.Web.UI.Page
    {
        protected string NGHtmlTemplatesJSON;
        protected string NGDirectiveInculdeHtmlTemplatesJSON;
        protected void Page_Load(object sender, EventArgs e)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            var templates = ConfigurationManager.GetSection("ngHtmlTemplates");
            NGHtmlTemplatesJSON = Newtonsoft.Json.JsonConvert.SerializeObject(templates, setting);


            templates = ConfigurationManager.GetSection("ngDirectiveInculdeHtmlTemplates");
            NGDirectiveInculdeHtmlTemplatesJSON = Newtonsoft.Json.JsonConvert.SerializeObject(templates, setting);
        }
    }
}