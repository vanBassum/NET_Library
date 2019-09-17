using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Threading;
using MasterLibrary.Misc;
using MasterLibrary.Extentions;

namespace MasterLibrary.Misc
{
    public class Copier
    {
        public event Action CopyFirstDone;
        private static int tot = 0;

        void counter(object parameters)
        {
            IEnumerable<string> files = parameters as IEnumerable<string>;
            tot = 0;
            foreach (string source in files)
            {
                tot++;
            }
        }

        public void CopyAll(string sourceDir, string destDir, Action<int> ReportProgress, string[] CopyFirst)
        {
            IEnumerable<string> files = from a in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories)
                                        orderby a.ToLower().ContainsAny(CopyFirst)?0:1
                                        select a;


            Thread t = new Thread(counter)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal,
                Name = "file enum"
            };
            t.Start(files);

            Thread.Sleep(100);
            int i = 0;
            bool report = true;
            foreach (string source in files)
            {
                int progress = i * 100 / Math.Max(tot, 1);
                progress = Math.Min(Math.Max(progress, 1), 100);
                ReportProgress(progress);
                i++;
                string dest = source.Replace(sourceDir, destDir);

                string dir = Path.GetDirectoryName(dest);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.Copy(source, dest, true);

                if (report)
                {
                    if (!source.ToLower().ContainsAny(CopyFirst))
                    {
                        CopyFirstDone?.Invoke();
                        report = false;
                    }
                }
            }


            ReportProgress(0);
        }
    }


}
