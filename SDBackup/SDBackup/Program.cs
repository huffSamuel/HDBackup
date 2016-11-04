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
                Console.Write("User's name and PC ID number: ");
                bi.ID = Console.ReadLine();
                Console.Write("Drive letter to backup: ");
                bi.Source = Console.ReadLine();

                if (string.IsNullOrEmpty(bi.ID) || string.IsNullOrEmpty(bi.Source))
                {
                    Console.WriteLine("Invalid arguments. Please use -h to see usage.");
                    return Constants.ERROR_ARGUMENTS;
                }

                // Perform Backup
                return Backup(path);

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
                bi.ID = args[0];
                bi.Source = args[1];
                return Backup(path);
            }

            else
            {
                Console.WriteLine("Invalid Command-Line Arguments");
                return Constants.ERROR_ARGUMENTS;
            }
        }

        static int Backup(string xmlPath)
        {
            // Initial Error Checking
            if (!Directory.Exists(bi.Source + "://"))
            {
                Console.WriteLine("Error: Missing source directory");
                return Constants.ERROR_MISSINGDIRECTORY;
            }

            // Get XML backup information 
            int error = ReadXML(xmlPath);
            if (error > 0) return error;

            // Begin recursive copy
            CopyDirectory(bi.Source + ":\\", bi.Local, true);

            // Display backup info
            Console.WriteLine("Copied " + bi.Count() + " files");
            DisplaySize(bi.Size());
            Console.Read();
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
            DirectoryInfo root = new DirectoryInfo(source);
            string[] dirs;

            // Check for path excludes
            if (bi.Paths().Contains(source, StringComparer.OrdinalIgnoreCase))
                return 0;
            // Check for folder excludes (AppData)
            if (bi.Dirs().Any(s => source.Contains(s)))
                return 0;

            // Copy files in "source"
            CopyFiles(source, dest);

            // Copy the directories below this
            if(copySubDirs)
            {
                try
                {
                    dirs = Directory.GetDirectories(source);
                    foreach (string dir in dirs)
                    {
                        CopyDirectory(dir, bi.Local, copySubDirs);
                    }
                }
                catch (Exception e) { bi.AddMessage(e.Message); }
            }
            // END Copy Directories
            
            return 0;
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
            string[] files;
            try // to copy files. Catches if unable to access
            {
                files = Directory.GetFiles(source);
                if (files == null) return 0;
                foreach (string file in files)
                {
                    if (!bi.Extensions().Contains(new FileInfo(file).Extension) && file.Contains("."))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(bi.Local + file.Substring(bi.Local.Length))))
                            Directory.CreateDirectory(Path.GetDirectoryName(bi.Local + file.Substring(bi.Local.Length)));
                        File.Copy(file, bi.Local + file.Substring(bi.Local.Length));
                        Console.WriteLine(file);
                        bi.AddBytes(new FileInfo(file).Length);
                    }
                }
            }
            catch (Exception e) { bi.AddMessage(e.Message); }
            return 0;
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
                // Write excluded file extensions
                w.WriteStartElement("extensions");
                w.WriteElementString("ex", ".sys");
                w.WriteEndElement();


                w.WriteEndElement();
                //
                w.WriteEndElement();
                w.WriteEndDocument();
            }
        }

        static int ReadXML(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode node = doc.DocumentElement.SelectSingleNode("/settings/dirs/local");
            bi.Local = node.InnerText;
            if (!Directory.Exists(bi.Local + "://"))
            {
                Console.WriteLine("Error: Missing local destination");
                return Constants.ERROR_MISSINGDIRECTORY;
            }

            node = doc.DocumentElement.SelectSingleNode("/settings/dirs/server");
            bi.Server = node.InnerText;
            // Add sanity checks for server location


            // Read excluded paths
            node = doc.DocumentElement.SelectSingleNode("/settings/exclude/paths");
            foreach (XmlNode child in node.ChildNodes)
            {
                bi.AddPath(child.InnerText);
            }

            // Read excluded directories
            node = doc.DocumentElement.SelectSingleNode("/settings/exclude/directories");
            foreach (XmlNode child in node.ChildNodes)
            {
                bi.AddDir(child.InnerText);
            }

            // Read excluded file extensions
            node = doc.DocumentElement.SelectSingleNode("/settings/exclude/extensions");
            foreach (XmlNode child in node.ChildNodes)
            {
                bi.AddExt(child.InnerText);
            }

            return 0;
        }
        
        // Global list of excluded users and directories
        // Ugly, I know, but functional.
        static BackupInfo bi = new BackupInfo();
    }
}
