using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DriveThru.Integration.Core.Helpers
{
    public static class TemplateHelper
    {
        public static string ParseTemplate(string template, IDictionary<string, string> tags)
        {
            foreach (string key in tags.Keys)
            {
                string replacement = string.IsNullOrEmpty(tags[key]) ? "" : tags[key];
                string pattern = "{{" + key + "}}";
                template = Regex.Replace(template, pattern, replacement, RegexOptions.IgnoreCase);
            }

            return template;
        }

        public static string ParseTemplate(string template, string prefix, string postfix, IDictionary<string, string> tags)
        {
            foreach (string key in tags.Keys)
            {
                string replacement = string.IsNullOrEmpty(tags[key]) ? "" : tags[key];
                string pattern = prefix + key + postfix;
                template = Regex.Replace(template, pattern, replacement, RegexOptions.IgnoreCase);
            }

            return template;
        }

        public static string ParseQueryTemplate(string template, IDictionary<string, object> parameters)
        {
            template = template.Replace("{{", "").Replace("}}", "");

            foreach (string key in parameters.Keys)
            {
                string replacement = string.Empty;
                if (parameters[key] is string || parameters[key] is DateTime)
                {
                    replacement = "'" + parameters[key].ToString() + "'";
                }
                else
                {
                    replacement = parameters[key].ToString();
                }

                string pattern = "@" + key + "";
                template = Regex.Replace(template, pattern, replacement, RegexOptions.IgnoreCase);
            }

            return template;
        }
    }
}
