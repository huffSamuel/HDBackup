using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBackup
{
    class BackupInfo
    {
        List<string> ExcludePaths;
        List<string> ExcludeDirs;
        List<string> ExcludeExt;
        long _bytes;
        long _files;
        List<string> ErrorMessages;

        public BackupInfo()
        {
            ExcludeDirs = new List<string>();
            ExcludePaths = new List<string>();
            ExcludeExt = new List<string>();
            _bytes = 0;
            _files = 0;
            ErrorMessages = new List<string>();
        }

        public void AddPath(string path){ ExcludePaths.Add(path); }
        public void AddDir(string dir) { ExcludeDirs.Add(dir); }
        public void AddExt(string ext) { ExcludeExt.Add(ext); }
        public void AddMessage(string msg) { ErrorMessages.Add(msg); }
        public void AddBytes(long bytes) { _bytes += bytes; ++_files; }


        public List<string> Paths() { return ExcludePaths; }
        public List<string> Dirs() { return ExcludeDirs; }
        public List<string> Extensions() { return ExcludeExt; }
        public long Size() { return _bytes; }
        public long Count() { return _files; }
        public string Local { get; set; }
        public string Server { get; set; }
        public string ID { get; set; }
        public string Source { get; set; }
        
    }
}
