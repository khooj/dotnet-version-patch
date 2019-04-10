using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace dotnet_version_patch
{
    class Program
    {
        static string FindFileRecursive(string directory, string mask)
        {
            var files = Directory.GetFiles(directory, mask, SearchOption.AllDirectories);
            if (files.Length == 0)
                throw new Exception($"Cannot find {mask} file");
            if (files.Length > 1)
                throw new Exception($"Found several {mask} files");
            return files[0];
        }

        static string ReadFile(string path)
        {
            using (var fs = File.Open(path, FileMode.Open))
            using (var sr = new StreamReader(fs))
                return sr.ReadToEnd();
        }

        static void WriteFile(string path, string content)
        {
            using (var fs = File.Open(path, FileMode.Truncate))
            using (var sw = new StreamWriter(fs))
                sw.Write(content);
        }

        static string SubstringMajorMinor(string version)
        {
            if (string.IsNullOrEmpty(version))
                return string.Empty;

            var v = Version.Parse(version);
            return $"{v.Major}.{v.Minor}";
        }

        static Version SubstringFullVersion(string version)
        {
            var v = Version.Parse(version);
            return v;
        }

        static (VersionStruct assembly, VersionStruct file) GetVersionFromAssemblyInfo(string content)
        {
            VersionStruct assembly = null;
            VersionStruct file = null;
            Regex reg = new Regex("\".*\"");

            foreach (var line in content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains("AssemblyVersion"))
                {
                    string c = reg.Match(line).Value.Trim('"');
                    assembly = VersionStruct.Parse(c);
                }

                if (line.Contains("AssemblyFileVersion"))
                {
                    string c = reg.Match(line).Value.Trim('"');
                    file = VersionStruct.Parse(c);
                }
            }

            return (assembly, file);
        }

        static string WriteVersionToAssemblyInfo((VersionStruct assembly, VersionStruct file) data, string content, bool addMissing)
        {
            var result = new StringBuilder();
            Regex reg = new Regex("\".*\"");

            foreach (var line in content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                string resultLine = line;
                if (line.Contains("AssemblyVersion"))
                {
                    resultLine = reg.Replace(line, $"\"{data.assembly}\"");
                }

                if (line.Contains("AssemblyFileVersion"))
                {
                    resultLine = reg.Replace(line, $"\"{data.file}\"");
                }

                result.AppendLine(resultLine);
            }

            return result.ToString();
        }

        struct Args
        {
            public bool IncrementMajor { get; set; }
            public bool IncrementMinor { get; set; }

            public static Args Parse(string[] args)
            {
                Args result = new Args
                {
                    IncrementMajor = false,
                    IncrementMinor = false
                };

                for (int i = 0; i < args.Length; ++i)
                {
                    switch (args[i])
                    {
                        case "--major":
                            result.IncrementMajor = true;
                            break;
                        case "--minor":
                            result.IncrementMinor = true;
                            break;
                        default:
                            break;
                    }
                }

                return result;
            }
        }

        static void Main(string[] args)
        {
            var arguments = Args.Parse(args);

            string path = Path.Combine(Environment.CurrentDirectory, "Properties");
            string project = FindFileRecursive(path, "AssemblyInfo*.cs");
            string fileContent = ReadFile(project);
            var versions = GetVersionFromAssemblyInfo(fileContent);
            VersionStruct v;
            if (versions.assembly != null)
                v = versions.assembly;
            else if (versions.file != null)
                v = versions.file;
            else
                throw new Exception("Not found version");

            if (arguments.IncrementMajor)
                v.Major = v.Major + 1;
            if (arguments.IncrementMinor)
                v.Minor = v.Minor + 1;

            versions.assembly = new VersionStruct(v);
            versions.file = new VersionStruct(v);
            versions.file.BuildString = string.Empty;
            versions.file.Revision = null;

            fileContent = WriteVersionToAssemblyInfo(versions, fileContent, false);
            WriteFile(project, fileContent);
        }
    }
}
