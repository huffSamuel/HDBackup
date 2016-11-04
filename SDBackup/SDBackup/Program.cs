using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using CommandLine;

/* ToDo : + Informative errors
 *        + /Full or /F option to back up program files
 *        + /IMG for .wim image (Long time out from this)
 *        + Implement good command line parsing
 *        + Implement switches for altering XML
 */


namespace SDBackup
{
    class Program
    {
        class Options
        {
            [HelpOption(HelpText="Display this help screen.")]
            public string GetHelp()
            {
                return Constants.HELP_MESSAGE;
            }
        }
        static int Main(string[] args)
        {
            string path = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "config.xml");
            if(!File.Exists(path))
                CreateXML(path);

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
                    Console.Clear();
                    Console.Write(Constants.HELP_MESSAGE);
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

            // Get XML backup information **************************************************************************************
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            string local, server;
            XmlNode node = doc.DocumentElement.SelectSingleNode("/settings/dirs/local");
            local = node.InnerText;
            //if (!Directory.Exists(local + "://")) return Constants.ERROR_MISSINGDIRECTORY;

            node = doc.DocumentElement.SelectSingleNode("/settings/dirs/server");
            server = node.InnerText;
// Add sanity checks for server location
            

            // Read exclude paths
            node = doc.DocumentElement.SelectSingleNode("/settings/exclude/paths");
            foreach(XmlNode child in node.ChildNodes)
            {
                excludes.Add(child.InnerText);
                GlobalExcludes.Add(child.InnerText);
            }

            // Read exclude directories
            node = doc.DocumentElement.SelectSingleNode("/settings/exclude/directories");
            foreach(XmlNode child in node.ChildNodes)
            {
                GlobalSubExcludes.Add(child.InnerText);
            }
            // END Get XML backup information **********************************************************************************

            // Begin recursive copy
            int count = 0;

            count += CopyDirectory("C:\\", local, true);

            Console.WriteLine("Copied " + count + " files");
            DisplaySize(bytes);

            return 0;
        }

        static void DisplaySize(long bytes)
        {
            Console.WriteLine((UInt64)(bytes / 1024f / 1024f / 1024f) + " GB");
        }

        /***********************************************************************
         * int CopyFiles(string source, string dest)
         * 
         * Input
         *      string source - Path to source directory
         *      string dest - destination drive letter
         *      
         * Output
         *      TESTING - Prints name of file to copy on screen
         *      RELEASE - Copies source to directory
         ***********************************************************************/
        static int CopyDirectory(string source, string dest, bool copySubDirs)
        {
            int count = 0;
            string[] dirs;

            // Check for path excludes
            if (GlobalExcludes.Contains(source))
                return 0;
            // Check for folder excludes (AppData)
            if (GlobalSubExcludes.Any(s => source.Contains(s)))
                return 0;

            // Copy files in "source"
            count += CopyFiles(source, dest);

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
                catch (Exception e) { Messages.Add(e.Message); }
            }
            // END Copy Directories
            
            return count;
        }

        /***********************************************************************
         * int CopyFiles(string source, string dest)
         * 
         * Input
         *      string source - Path to source directory
         *      string dest - destination drive letter
         *      
         * Output
         *      TESTING - Prints name of file to copy on screen
         *      RELEASE - Copies source to directory
         ***********************************************************************/
        static int CopyFiles(string source, string dest)
        {
            FileInfo fi;
            int count = 0;
            string[] files;
            try // to copy files. Catches if unable to access
            {
                files = Directory.GetFiles(source);
                if (files == null) return count;
                foreach (string file in files)
                {
                    if (!file.EndsWith(".sys") && file.Contains("."))
                    {
                        // File.Copy(file, dest + file.Substring(dest.length));
                        
                        Console.WriteLine(file);
                        ++count;
                        fi = new FileInfo(file);
                        bytes += (long)fi.Length;
                    }
                }
            }
            catch (Exception e) { Messages.Add(e.Message); }
            return count;
        }

        static void CreateXML(string path)
        {
            using (XmlWriter w = XmlWriter.Create(path))
            {
                w.WriteStartDocument();
                w.WriteStartElement("settings");
                // Local and remote backup destination
                w.WriteStartElement("dirs");
                w.WriteElementString("local", "d");
                w.WriteElementString("server", "serv");
                w.WriteEndElement();
                // Write excluded paths
                w.WriteStartElement("exclude");
                w.WriteStartElement("paths");
                w.WriteElementString("ex", "C:\\Users\\!adminx");
                w.WriteElementString("ex", "C:\\Users\\All Users");
                w.WriteElementString("ex", "C:\\Windows");
                w.WriteElementString("ex", "C:\\ProgramData");
                w.WriteElementString("ex", "C:\\Program Files");
                w.WriteElementString("ex", "C:\\Program Files (x86)");
                w.WriteElementString("ex", "C:\\$Recycle.Bin");
                w.WriteElementString("ex", "C:\\Qt");
                w.WriteElementString("ex", "C:\\Python34");
                w.WriteEndElement();
                // Write excluded directories 
                w.WriteStartElement("directories");
                w.WriteElementString("ex", "AppData");
                w.WriteEndElement();

                w.WriteEndElement();
                //
                w.WriteEndElement();
                w.WriteEndDocument();
            }
        }
        
        // Global list of excluded users and directories
        // Ugly, I know, but functional.
        static List<string> GlobalExcludes = new List<string>();
        static List<string> GlobalSubExcludes = new List<string>();
        static long bytes;
        static List<string> Messages = new List<string>();

    }
}
