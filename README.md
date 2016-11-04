[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
# Service Desk Backup
This is a backup utility that will copy files to a local and network backup location. Destinations and excluded directories are configured through an XML file. 

## Install
Download and run INSTALL\Install_SDBackup.msi. This will install the application in C:\Program Files\SDBackup and create Start Menu and Desktop shortcuts for the application. It also configures the default XML for locations and excludes.

## Usage
SDBackup allows for various command-line usages. 
#### With default arguments:
`cmd > SDBackup.exe [info] [source]`  
[info] is the identifier of the machine to backup  
[source] is the source drive to backup  

#### Without default arguments:
`cmd > SDBackup.exe`  
This option will prompt the user to enter [info] and [source]

#### Switches
| Option | Switch | Description
|:--------|:--------|:------------|
| Help| -h or -? | Displays the help menu. |
| * Full | -f | Ignores XML excluded directories and attempts to back up full drive.|
| * Image | -i | Attempts to create a Windows Imaging Format (WIM) file of the source drive. |
| * Local | -l | Ignores XML's remote node and only backs up to local directory. |
| * Network | -n | Ignores XML's local node and only backs up to network location. |

Items marked with * are planned or in-progress but not currently working.
