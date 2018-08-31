using System;
using System.Diagnostics;
using System.IO;

namespace PepLib
{
    public static class Util
    {
        public static string GetNestFileFormat(string filename)
        {
            try
            {
                var name = Path.GetFileName(filename);
                var ext = Path.GetExtension(name);

                if (name.LastIndexOf(ext) > 5 && !name.Contains("-"))
                    name = name.Insert(5, "-");

                if (name.LastIndexOf(ext) > 8 && char.IsLetter(name[8]))
                    name = name.Remove(8, 1);

                return Path.Combine(Path.GetDirectoryName(filename), name);
            }
            catch (SystemException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return string.Empty;
        }
    }
}
