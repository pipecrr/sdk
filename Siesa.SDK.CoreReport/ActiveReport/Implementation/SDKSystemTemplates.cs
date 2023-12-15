using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Siesa.SDK.Frontend.Report.Services;
using GrapeCity.ActiveReports.Aspnetcore.Designer.Utilities;
using Siesa.SDK.Shared.Utilities;
using System.Text;

namespace Siesa.SDK.Report.Implementation
{
    public class SDKSystemTemplates : ITemplatesService
    {

        static readonly string[] TemplateExtensions = { ".rdlx", ".rdl" };

        public TemplateInfo[] GetTemplatesList()
        {
            var templatesList = Utilities.GetAssemblyResources(this.GetType().Assembly, $"ActiveReport.templates")
                .Where(x => string.Equals(Path.GetExtension(x), ".rdlx", StringComparison.InvariantCultureIgnoreCase))
                .Select(name => new TemplateInfo
                {
                    Id = name.Replace("Siesa.SDK.CoreReport.ActiveReport.templates.","", 
                    StringComparison.Ordinal),
                    Name = name.Replace("Siesa.SDK.CoreReport.ActiveReport.templates.","", 
                    StringComparison.Ordinal)
                }).ToArray();
            return templatesList;
        }


        const string templateThumbnailName = "template_thumbnail";
        // Gets template content with thumbnail cut out if it exists
        public byte[] GetTemplate(string id)
        {
            string templateContent = "";
            try
            {
                templateContent = Utilities.ReadAssemblyResource(this.GetType().Assembly, 
				$"Siesa.SDK.CoreReport.ActiveReport.templates.{id}");


            }
            catch (System.Exception)
            {

            }

            if (string.IsNullOrEmpty(templateContent)) throw new FileNotFoundException();

            var templateXml = Encoding.UTF8.GetBytes(templateContent);
            var template = ReportConverter.FromXML(templateXml);
            var thumbnail = template.EmbeddedImages.FirstOrDefault(i => i.Name == templateThumbnailName);
            if (thumbnail != null) template.EmbeddedImages.Remove(thumbnail);

            var templateJson = ReportConverter.ToJson(template);
            return templateJson;
        }


        // Gets a thumbnail with specific name from Embedded Images
        public TemplateThumbnail GetTemplateThumbnail(string id)
        {
            string templateContent = "";
            try
            {
                templateContent = Utilities.ReadAssemblyResource(this.GetType().Assembly, $"ActiveReport.templates.{id}");
            }
            catch (System.Exception)
            {

            }

            if (string.IsNullOrEmpty(templateContent)) throw new FileNotFoundException();

            var templateXml = Encoding.UTF8.GetBytes(templateContent);
            var xElement =  XElement.Load(templateContent);
            var thmumbnailElement = xElement.XPathSelectElement($"*[local-name() = 'EmbeddedImages']/*[local-name() = 'EmbeddedImage' and @Name='{templateThumbnailName}']");
            if (thmumbnailElement == null) throw new FileNotFoundException();
            var data = thmumbnailElement.XPathSelectElement("*[local-name() = 'ImageData']")?.Value ?? string.Empty;
            var mimeType = thmumbnailElement.XPathSelectElement("*[local-name() = 'MIMEType']")?.Value ?? string.Empty;

            return new TemplateThumbnail() { Data = data, MIMEType = mimeType };
        }
    }
}
