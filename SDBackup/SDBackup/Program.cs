using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace SDBackup
{
    class Program
    {
        static int Main(string[] args)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "config.xml");
            if(!File.Exists(path))
            {
                using (XmlWriter w = XmlWriter.Create(path))
                {
                    w.WriteStartDocument();
                    w.WriteStartElement("settings");
                    //
                    w.WriteStartElement("dirs");
                    w.WriteElementString("local", "d");
                    w.WriteElementString("server", "serv");
                    w.WriteEndElement();
                    //
                    w.WriteStartElement("exclude");
                    w.WriteElementString("_1", "C:\\Users\\!adminx");
                    w.WriteEndElement();
                    //
                    w.WriteEndElement();
                    w.WriteEndDocument();
                }
            }

            if (args.Length == 0)
            {
                string ID_Name, srcDrive;

                Console.Write("User's name and PC ID number: ");
                ID_Name = Console.ReadLine();
                Console.Write("Drive letter to backup: ");
                srcDrive = Console.ReadLine();

                if (string.IsNullOrEmpty(ID_Name) || string.IsNullOrEmpty(srcDrive))
                {
                    Console.WriteLine("Invalid arguments. Please use -h to see usage.");
                    return Constants.ERROR_ARGUMENTS;
                }

                // Perform Backup
                return Backup(ID_Name, srcDrive, path);

            }
            else if (args.Length == 1)
            {
                if (args[0] == "/h" || args[0] == "-h" || args[0] == "/?" || args[0] == "-?")
                {
                    Console.WriteLine("Displaying HELP");
                    return 0;
                }

                else
                {
                    Console.WriteLine("Invalid Command-Line Arguments");
                    return Constants.ERROR_ARGUMENTS;
                }
            }

            else if (args.Length == 2)
            {
                Console.WriteLine("User entered 10. and source");

                if (string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
                {
                    Console.WriteLine("Invalid arguments. Please use -h to see usage.");
                    return Constants.ERROR_ARGUMENTS;
                }

                return Backup(args[0], args[1], path);
            }

            else
            {
                Console.WriteLine("Invalid Command-Line Arguments");
                return Constants.ERROR_ARGUMENTS;
            }
        }

        static int Backup(string ID_name, string srcDrive, string xmlPath)
        {
            // Initial Error Checking
            if (!Directory.Exists(srcDrive + "://"))        // If the source drive does not exist
                return Constants.ERROR_MISSINGDIRECTORY;

            DriveInfo src = new DriveInfo(srcDrive);


            List<string> excludes = new List<string>();
            // Get XML backup information
            // local is the local destination
            // server is the server backup destination
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            string local, server;
            XmlNode node = doc.DocumentElement.SelectSingleNode("/settings/dirs/local");
            local = node.InnerText;
            node = doc.DocumentElement.SelectSingleNode("/settings/dirs/server");
            server = node.InnerText;

            node = doc.DocumentElement.SelectSingleNode("/settings/exclude");
            
            foreach(XmlNode child in node.ChildNodes)
            {
                excludes.Add(child.InnerText);
            }
            // END Get XML backup information

            // Copy Root Directory
            string[] files = Directory.GetFiles(srcDrive + ":\\");
            foreach (string file in files)
            {
                if(!file.EndsWith(".sys") && file.Contains("."))
                {
                   // File.Copy(file, local + file.Substring(1));
                    Console.WriteLine("File.Copy(" + file + ", " + local + "\\Root" + file.Substring(1) + ")");
                }
            }

            // Copy Users Directory
            int count = CopyDirectory("C:\\Users", local, true);
            Console.WriteLine("Copied " + count + " files");


            return 0;
        }

        static void DisplaySize(long bytes)
        {
            Console.WriteLine((int)(bytes / 1024f / 1024f / 1024f) + " GB");
        }

        static int CopyDirectory(string source, string dest, bool copySubDirs)
        {
            if (source == "C:\\Users\\!adminx")
                return 0;
            if (source.Contains("AppData"))
                return 0;
            int count = 0;
            string[] dirs;

            // Copy files in the source directory
            count += CopyFiles(source, dest);
            // END Copy Files

            // Copy the directories below this
            if(copySubDirs)
            {
                try
                {
                    dirs = Directory.GetDirectories(source);
                    foreach (string dir in dirs)
                    {
                        count += CopyDirectory(dir, dest, copySubDirs);
                    }
                }
                catch (Exception e) { }
            }
            // END Copy Directories
            
            return count;
        }
        static int CopyFiles(string source, string dest)
        {
            int count = 0;
            string[] files;
            try
            {
                files = Directory.GetFiles(source);
                if (files == null) return count;
                foreach (string file in files)
                {
                    if (!file.EndsWith(".sys") && file.Contains("."))
                    {
                        // File.Copy(file, local + file.Substring(1));
                        Console.WriteLine(file);
                        ++count;
                    }
                }
            }
            catch (Exception e) { }
            return count;
        }
    }
}
