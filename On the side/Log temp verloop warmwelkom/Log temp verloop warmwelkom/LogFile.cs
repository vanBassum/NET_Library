using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Log_temp_verloop_warmwelkom
{
    public class LogFile<T> where T : LogItemBase
    {
        public List<LogItemBase> Items { get; set; } = new List<LogItemBase>();


        public static LogFile<T> Open(string file)
        {
            LogFile<T> logFile = new LogFile<T>();

            using (StreamReader rdr = new StreamReader(file))
            {
                while(!rdr.EndOfStream)
                {
                    string line = rdr.ReadLine();


                    var listOfBs = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                    from assemblyType in domainAssembly.GetTypes()
                                    where typeof(T).IsAssignableFrom(assemblyType)
                                    select assemblyType).ToArray();

                    foreach (Type t in listOfBs)
                    {
                        if (t.IsAbstract)
                            continue;
                        T logItem = (T)Activator.CreateInstance(t);

                        if (logItem.TryPopulate(line))
                        {
                            logFile.Items.Add(logItem);
                        }

                    }




                    /*

                    MethodInfo info = typeof(T).GetMethod("TryParse", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                    if (info != null)
                    {
                        MethodInfo genericMethod = info.MakeGenericMethod(new[] { typeof(T) });

                        if(genericMethod != null)
                        {
                            T logItem;

                            if ((bool)genericMethod.Invoke(null, new object[] { logItem }))
                            {
                                logItem = (LogItemDoor)Activator.CreateInstance(t);
                                logItem.Code = code;
                                logItem.Timestamp = timestamp;
                            }
                        }
                    }

                    if (T.TryParse<T>(line, out logItem))

                            */
                        
                }
            }
            return logFile;
        }
    }
}
