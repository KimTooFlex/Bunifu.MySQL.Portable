using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Bunifu.MySQL.Portable
{
    public class MySQLService
    {

        public static int Port { get; set; }
        public static Process StartService(int port = 3306)
        {
            Port = port;
            Deploy();
            if (File.Exists("MySQL\\stop.exe"))
            {


                Process proc = new Process();
                proc.StartInfo.FileName = "MySQL\\start.exe";
                proc.Start();



                return proc;
            }

            return null;

        }
        public static void StopSerice()
        {
            if (File.Exists("MySQL\\stop.exe"))
            {
                var stopProc = new Process();
                stopProc.StartInfo.FileName = "MySQL\\stop.exe";
                stopProc.Start();
                stopProc.WaitForExit();
            }
        }

        static void Deploy(bool Ovewrite = false)
        {
            StopSerice();
            //if (Ovewrite || !Directory.Exists("MySQL"))
            {
                Directory.CreateDirectory("MySQL");


                //Level 1
                if (!Directory.Exists(@"MySQL\bin")) Directory.CreateDirectory(@"MySQL\bin");
                if (!Directory.Exists(@"MySQL\data")) Directory.CreateDirectory(@"MySQL\data");
                if (!Directory.Exists(@"MySQL\share")) Directory.CreateDirectory(@"MySQL\share");

                //Leve 2
                if (!Directory.Exists(@"MySQL\data\mysql")) Directory.CreateDirectory(@"MySQL\data\mysql");
                if (!Directory.Exists(@"MySQL\share\charsets")) Directory.CreateDirectory(@"MySQL\share\charsets");
                if (!Directory.Exists(@"MySQL\share\english")) Directory.CreateDirectory(@"MySQL\share\english");


                foreach (var item in GetResourceFiles(".MySQL."))
                {

                    string path = item.Substring(0, item.LastIndexOf("."));
                    path = path.Substring(0, path.LastIndexOf(".")).Replace("MySQL.Portable.", "");
                    string filename = item.Replace("MySQL.Portable." + path + ".", "");

                    ExtractResourceFile(item, path.Replace(".", "\\") + "\\" + filename);
                }

                File.WriteAllText(@"MySQL\my.ini", ReadResourceFile("my.ini")
                    .Replace("$port", Port.ToString())
                    .Replace("$path", Environment.CurrentDirectory.Replace("\\", "/"))
                    );

            }
        }
        public static string ReadResourceFile(string filename)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();

            foreach (var item in thisAssembly.GetManifestResourceNames())
            {
                //Console.WriteLine(item);

                if (item.EndsWith(filename))
                {
                    using (var stream = thisAssembly.GetManifestResourceStream(item))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }

            throw new Exception($"{filename} name not found");

        }
        internal static void ExtractResourceFile(string filename, string targetFile)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();

            foreach (var item in thisAssembly.GetManifestResourceNames())
            {

                if (item.EndsWith(filename))
                {
                    using (var stream = thisAssembly.GetManifestResourceStream(item))
                    {
                        using (var file = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
                        {
                            stream.CopyTo(file);
                        }
                    }
                }
            }



        }
        public static List<string> GetResourceFiles(string path = "")
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            List<string> ret = new List<string>();
            foreach (var item in thisAssembly.GetManifestResourceNames())
            {

                if (item.Contains(path))
                {
                    ret.Add(item);
                }
            }
            return ret;
        }
    }

}
