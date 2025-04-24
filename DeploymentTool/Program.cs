using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using CloudSalon.Common;
using System.Xml;
using System.Configuration;

namespace DeploymentTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectFolder = ConfigurationManager.AppSettings["ProjectFolder"];
            string projectHomePage = ConfigurationManager.AppSettings["ProjectHomePage"];
            string allScriptPath = ConfigurationManager.AppSettings["AllScriptPath"];
            string allCSSPath = ConfigurationManager.AppSettings["allCSSPath"];

            Console.WriteLine("input 1 for deployment js, 2 for css, 3 for html, 123 for all");

            string str = Console.ReadLine();

            if (str.IndexOf("1") > -1)
            {
                //打包JS
                Pack(projectFolder, projectHomePage, allScriptPath, "js", "/configuration/appSettings/add[@key='AllScriptDigest']");
            }

            if (str.IndexOf("2") > -1)
            {
                //打包CSS
                Pack(projectFolder, projectHomePage, allCSSPath, "css", "/configuration/appSettings/add[@key='AllCSSDigest']");
            }

            if (str.IndexOf("3") > -1)
            {
                //创建所有angular HTML模板的摘要
                CreateHTMLTemplateDigest(projectFolder);
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void CreateHTMLTemplateDigest(string projectFolder)
        {
            XmlDocument xml = new System.Xml.XmlDocument();
            xml.Load(projectFolder + "Web.config");

            XmlNodeList nodes = xml.SelectNodes("/configuration/ngHtmlTemplates/template");
            foreach (XmlNode node in nodes)
            {
                string content = File.ReadAllText(projectFolder + node.SelectSingleNode("templateUrl").InnerText);
                string MD5 = EncodingHelper.MD5(content);
                node.SelectSingleNode("digest").InnerText = MD5;
            }

            nodes = xml.SelectNodes("/configuration/ngDirectiveInculdeHtmlTemplates/template");
            foreach (XmlNode node in nodes)
            {
                string content = File.ReadAllText(projectFolder + node.SelectSingleNode("templateUrl").InnerText);
                string MD5 = EncodingHelper.MD5(content);
                node.SelectSingleNode("digest").InnerText = MD5;
            }


            xml.Save(projectFolder + "Web.config");
            xml = null;
        }

        private static void Pack(string projectFolder, string projectHomePage, string packedPath, string cssOrJs,string xPath)
        {
            //1.读default.aspx所有的js/css
            List<string> files = ReadAllJSCSSPath(projectFolder, projectHomePage, packedPath, cssOrJs);

            //2. 读取所有文件的内容，生成摘要
            StringBuilder stringbuilder = new StringBuilder();
            files.ForEach(js =>
            {
                stringbuilder.Append(File.ReadAllText(js, UTF8Encoding.UTF8));
            });
            string MD5 = EncodingHelper.MD5(stringbuilder.ToString());


            //3.把合并后的javascript或css写入文件
            File.WriteAllText(projectFolder + packedPath, stringbuilder.ToString(), UTF8Encoding.UTF8);


            //4. 把生成的摘要,写入Web.config
            WriteAllScriptDigest(projectFolder, MD5, xPath);
        }


        private static void WriteAllScriptDigest(string folder, string digest, string xPath)
        {
            XmlDocument xml = new System.Xml.XmlDocument();
            xml.Load(folder + "Web.config");

            XmlNode node = xml.SelectSingleNode(xPath);
            node.Attributes["value"].Value = digest;

            xml.Save(folder + "Web.config");
            xml = null;
        }


        private static List<string> ReadAllJSCSSPath(string projectPath, string projectHomePage, string packedPath,string cssOrJs)
        {
            List<string> js=new List<string>();
            Match matchedJs = null;
            MatchCollection matchedScripts = null;
            string[] allLines = System.IO.File.ReadLines(projectPath + projectHomePage).ToArray();
            bool isInMulComment = false;
            
            for (int i = 0; i < allLines.Length; i++)
            {
                string line = allLines[i].Trim();
                if (!isInMulComment)
                {
                    //跳过<!--xxxxx-->
                    if (line.IndexOf("<!--") == 0 && line.EndsWith("-->"))
                        continue;

                    //跳过<%--xxxxx--%>
                    if (line.IndexOf("<%--") == 0 && line.EndsWith("--%>"))
                        continue;

                    //跳过<!-- 中间有多行  -->
                    if (line.IndexOf("<!--") == 0)
                    {
                        isInMulComment = true;
                        continue;
                    }

                    //跳过<%-- 中间有多行 --%>
                    if (line.IndexOf("<%--") == 0)
                    {
                        isInMulComment = true;
                        continue;
                    }

                    //跳过allscript.js或allstyle.css
                    if (line.IndexOf(packedPath,StringComparison.CurrentCultureIgnoreCase) > -1)
                        continue;


                    if (cssOrJs == "js")
                        matchedScripts = Regex.Matches(line, @"<script[^>]+></script>");
                    else
                        matchedScripts = Regex.Matches(line, @"<link[^>]+/>");
                    foreach (Match m in matchedScripts)
                    {
                        matchedJs = Regex.Match(m.Value, (cssOrJs == "css" ? "href" : "src") + "=\"([^\"]+)\"");
                        if (matchedJs != null)
                            js.Add(projectPath + matchedJs.Groups[1].Value.Insert(matchedJs.Groups[1].Value.IndexOf("." + cssOrJs), ".min"));
                    }

                }
                else
                {
                    if (line.EndsWith("-->") || line.EndsWith("--%>"))
                        isInMulComment = false;
                }
            }

            return js;
        }
    }
}
