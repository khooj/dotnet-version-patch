using System.Xml.Linq;
using System.Linq;

namespace dotnet_version_patch
{
    public class XmlHelpers
    {
        static (XElement assembly, XElement file, XDocument doc) GetVersionXmlFromCsproj(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            foreach (var el in doc.Root.Elements())
            {
                if (el.Name == "PropertyGroup")
                {
                    var assemblyVersion = el.Elements().FirstOrDefault(x => x.Name == "AssemblyVersion");
                    var fileVersion = el.Elements().FirstOrDefault(x => x.Name == "FileVersion");

                    if (assemblyVersion == null && fileVersion == null)
                        continue;

                    return (assemblyVersion, fileVersion, doc);
                }
            }

            return (null, null, doc);
        }

        static string WriteVersionToCsproj((XElement assembly, XElement file, XDocument doc) data, bool addMissing)
        {
            foreach (var el in data.doc.Root.Elements())
            {
                if (el.Name == "PropertyGroup")
                {
                    var assemblyXml = el.Elements().FirstOrDefault(x => x.Name == "AssemblyVersion");
                    var fileXml = el.Elements().FirstOrDefault(x => x.Name == "FileVersion");
                    if (assemblyXml == null && fileXml == null)
                        continue;

                    if (data.assembly != null)
                    {
                        if (assemblyXml == null)
                        {
                            if (addMissing)
                                el.Add(data.assembly);
                        }
                        else
                            assemblyXml.SetValue(data.assembly.Value);
                    }

                    if (data.file != null)
                    {
                        if (fileXml == null)
                        {
                            if (addMissing)
                                el.Add(data.file);
                        }
                        else
                            fileXml.SetValue(data.file.Value);
                    }

                    break;
                }
            }

            return data.doc.ToString();
        }
    }
}
