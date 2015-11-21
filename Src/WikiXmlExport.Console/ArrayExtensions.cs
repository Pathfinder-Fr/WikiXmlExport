using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiXmlExport.Console
{
    public static class ArrayExtensions
    {
        public static string ArgNamed(this string[] args, string name, string defaultValue = default(string))
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                var argFormat = string.Format("/{0}", name);
                if (arg.StartsWith(argFormat, StringComparison.OrdinalIgnoreCase))
                {
                    if (argFormat.Length == arg.Length)
                    {
                        // Juste le paramètre. Exemple /toto, on renvoie "true"
                        return bool.TrueString;
                    }
                    else if (arg.StartsWith(argFormat + ":", StringComparison.OrdinalIgnoreCase))
                    {
                        // Paramètre + valeur (ex: /toto:machin)
                        return arg.Substring(argFormat.Length + 1);
                    }
                }
            }

            return null;
        }
    }
}
