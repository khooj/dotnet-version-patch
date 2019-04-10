using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_version_patch
{
    public class VersionStruct
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        private string _build { get; set; }

        public string BuildString
        {
            get { return _build; }
            set { _build = value; }
        }

        public int BuildInt
        {
            get
            {
                if (int.TryParse(_build, out int result))
                    return result;
                throw new Exception("Cannot parse build");
            }

            set { _build = value.ToString(); }
        }

        public int? Revision { get; set; }

        public VersionStruct()
        {
            Major = 0;
            Minor = 0;
            _build = string.Empty;
            Revision = null;
        }

        public VersionStruct(VersionStruct v)
        {
            Major = v.Major;
            Minor = v.Minor;
            BuildString = v.BuildString;
            Revision = v.Revision;
        }

        public static VersionStruct Parse(string content)
        {
            var result = new VersionStruct();
            var tokens = content.Split('.');
            if (tokens.Length < 2)
                throw new Exception("Unsupported version struct format");
            int tmp = 0;
            if (!int.TryParse(tokens[0], out tmp))
                throw new Exception($"Not valid major format: {tokens[0]}");
            result.Major = tmp;

            if (!int.TryParse(tokens[1], out tmp))
                throw new Exception($"Not valid minor format: {tokens[1]}");
            result.Minor = tmp;

            if (tokens.Length == 3)
            {
                if (tokens[2] == "*")
                {
                    result.BuildString = tokens[2];
                    return result;
                }
                
                if (!int.TryParse(tokens[2], out tmp))
                    throw new Exception($"Not valid build format: {tokens[2]}");

                result.BuildInt = tmp;
            }
            else if (tokens.Length == 4)
            {
                if (tokens[2] == "*")
                    throw new Exception("Version format cannot contain revision when build is \"*\"");

                if (!int.TryParse(tokens[2], out tmp))
                    throw new Exception($"Not valid build format: {tokens[2]}");
                result.BuildInt = tmp;

                if (!int.TryParse(tokens[3], out tmp))
                    throw new Exception($"Not valid revision format: {tokens[3]}");
                result.Revision = tmp;
            }

            return result;
        }

        public override string ToString()
        {
            string result = $"{Major}.{Minor}";
            if (!string.IsNullOrEmpty(BuildString))
                result = $"{result}.{BuildString}";
            if (Revision.HasValue)
                result = $"{result}.{Revision.Value}";
            return result;
        }
    }
}
