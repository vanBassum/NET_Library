using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoUpdaterTest
{
    public class Updator
    {

        public event Action CloseApplication;

        string GetNewestVersion(string path)
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

        //Returns wether this went sucsessfully.
        //If not, retry.
        public bool DoUpdate(string path)
        {
            string[] args = Environment.GetCommandLineArgs();

            //args = new string[] { @"C:\a\Updater\V1.0\AutoUpdaterTest.exe", @"C:\Workspace\NET_Library\MasterLibrary\AutoUpdaterTest\bin\Debug" };

            if (args.Length == 2)
            {
                //We are the updator

                try
                {
                    //Move running application to the path passed in the arg
                    //Directory.Delete(args[1], true);
                    CopyAll(Path.GetDirectoryName(args[0]), args[1]);

                    return true;
                }
                catch(Exception ex)
                {

                }
            }
            else
            {
                //Check for new version and update
                string newest = GetNewestVersion(path);
                if (newest == "")
                    return true;

                Version newVers = Version.Parse(Path.GetFileName(newest).ToUpper().Replace(" ", "").TrimStart('V'));
                if (newVers < Assembly.GetExecutingAssembly().GetName().Version)
                    return true;

                string newApplication = Path.Combine(newest, System.AppDomain.CurrentDomain.FriendlyName);

                if (File.Exists(newApplication))
                {
                    Process.Start(newApplication, Directory.GetCurrentDirectory());
                    CloseApplication?.Invoke();
                }
                return true;
            }
            return false;
        }


        void CopyAll(string source, string dest)
        {
            // Now Create all of the directories
            string[] dirs = Directory.GetDirectories(source, "*", SearchOption.AllDirectories);
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i].Replace(source, dest);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }

            //Copy all the files & Replaces any files with the same name
            string[] files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i], files[i].Replace(source, dest), true);
            }
        }
    }
}
