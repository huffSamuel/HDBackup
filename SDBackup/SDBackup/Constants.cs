using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBackup
{
    class Constants
    {
        public const int ERROR_ARGUMENTS = 1;
        public const int ERROR_MISSINGDIRECTORY = 2;
        public const string HELP_MESSAGE = 
            @"-----------------------------------------------------------------
   SDBackup   ::  Service Desk Backup                               
-----------------------------------------------------------------
 Usage :: SDBackup pcinfo source                                  
              
 pcinfo :: ITS ID number and User's name (Huff1234 or Huff 1234). 
 source :: Source Drive (F or D).                                 
                                                                               
                                                                               
::                                                               
:: Options :                                                     
::                                                               
             /h :: displays help menu.                           
             SDBackup [ENTER] :: Prompts for pcinfo + source.    

";
    }
}
