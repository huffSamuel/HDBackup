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
            XmlDocument config = new XmlDocument();
            config.Load(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "config.xml"));
            XmlNode node = config.DocumentElement.SelectSingleNode("/settings/destinations");
            string destLocal = node.Attributes["local"]?.InnerText;
            string destServer = node.Attributes["server"]?.InnerText;
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
                return Backup(ID_Name, srcDrive);

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

                return Backup(args[0], args[1]);
            }

            else
            {
                Console.WriteLine("Invalid Command-Line Arguments");
                return Constants.ERROR_ARGUMENTS;
            }

        }

        static int Backup(string ID_name, string srcDrive)
        {
            
            return 0;
        }
    }
}
