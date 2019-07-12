using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary
{
    public class AutoUpdate
    {
        static string GetNewestVersion(string path)
        {
            string newestVersion = "";

            foreach (string versionFolder in Directory.EnumerateDirectories(path))
            {
                Version version;
                string vString = Path.GetFileName(versionFolder).ToUpper().Replace(" ", "").TrimStart('V');
                string vNewest = Path.GetFileName(newestVersion).ToUpper().Replace(" ", "").TrimStart('V');
                if (Version.TryParse(vString, out version))
                {
                    if (newestVersion == "")
                        newestVersion = versionFolder;
                    else
                    {
                        if (version > Version.Parse(vNewest))
                            newestVersion = versionFolder;
                    }
                }
            }

            return newestVersion;
        }


        public static bool CheckForUpdate(string path, out Version newVersion)
        {
            Version thisVers = Assembly.GetEntryAssembly().GetName().Version;
            string newest = GetNewestVersion(path);
            newVersion = new Version(0, 0, 0, 0);

            if (newest == "")
                return false;

            newVersion = Version.Parse(Path.GetFileName(newest).ToUpper().Replace(" ", "").TrimStart('V'));
            
            if (newVersion > thisVers)
                return true;

            return false;
        }


    }
}
