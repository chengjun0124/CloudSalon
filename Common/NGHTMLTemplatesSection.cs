using CloudSalon.Model.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace CloudSalon.Common
{
    public class NGHTMLTemplatesSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            List<NGHTMLTemplate> list = new List<NGHTMLTemplate>();

            foreach (XmlNode node in section.ChildNodes)
            {
                list.Add(new NGHTMLTemplate()
                {
                    State = node.SelectSingleNode("state").InnerText,
                    Url = node.SelectSingleNode("url").InnerText,
                    TemplateUrl = node.SelectSingleNode("templateUrl").InnerText,
                    Controller = node.SelectSingleNode("controller").InnerText,
                    Digest = node.SelectSingleNode("digest").InnerText
                });
            }

            return list;
        }
    }

    public class NGDirectiveInculdeHtmlTemplatesSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<NGDirectiveInculdeHtmlTemplate> list = new List<NGDirectiveInculdeHtmlTemplate>();

            foreach (XmlNode node in section.ChildNodes)
            {
                list.Add(new NGDirectiveInculdeHtmlTemplate()
                {
                    Name = node.SelectSingleNode("name").InnerText,
                    TemplateUrl = node.SelectSingleNode("templateUrl").InnerText,
                    Digest = node.SelectSingleNode("digest").InnerText
                });
            }

            return list;
        }
    }

    public class NGHTMLTemplate
    {
        public string State;
        public string Url;
        public string TemplateUrl;
        public string Controller;
        public string Digest;
    }

    public class NGDirectiveInculdeHtmlTemplate
    {
        public string Name;
        public string TemplateUrl;
        public string Digest;
    }
}
