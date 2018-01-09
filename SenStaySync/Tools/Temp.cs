using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SenStaySync
{
    public static class Temp
    {
        public static void EmptyFolder(string Path = null)
        {
            try
            {
                if (Path == null)
                {
                    Path = Config.I.TempDirectory;
                }

                if (!Directory.Exists(Path)) return;

                DirectoryInfo DirInfo = new DirectoryInfo(Path);

                foreach (FileInfo file in DirInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in DirInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch
            {

            }
        }

        public static FileInfo GetFirstFile(string Pattern = "*.csv", string Path = null)
        {
            if (Path == null)
            {
                Path = Config.I.TempDirectory;
            }

            DirectoryInfo DirInfo = new DirectoryInfo(Path);
            foreach (var File in DirInfo.GetFiles(Pattern))
            {
                return File;
            }

            return null;
        }

        public static List<FileInfo> GetFileList(string Pattern = "*.csv", string Path = null)
        {
            if (Path == null)
            {
                Path = Config.I.TempDirectory;
            }

            var data = new List<FileInfo>();
            DirectoryInfo DirInfo = new DirectoryInfo(Path);
            foreach (var File in DirInfo.GetFiles(Pattern))
            {
                data.Add(File);
            }

            return data;
        }



        public static FileInfo WaitFotTheFirstFile(string Pattern = "*.csv", string Path = null, int MillisecondsExpire = 1000 * 60 * 2)
        {
            var ms = 0;
            while (true)
            {
                var fi = GetFirstFile(Pattern, Path);
                if (fi == null || fi.Length > 0)
                {
                    return fi;
                }
                else
                {
                    Thread.Sleep(1000);
                    ms += 1000;
                }

                if (ms > MillisecondsExpire)
                {
                    return null;
                }
            }
            
        }


        public static void RenameFile(FileInfo File, string newName)
        {
            File.MoveTo(File.Directory.FullName + "\\" + newName);
        }

        public static void MoveToDirectory(FileInfo File, string newDirectoryPath)
        {
            Move(File, newDirectoryPath + "\\" + File.Name);
        }

        public static void Move(FileInfo FileI, string newFullPath)
        {
            if (File.Exists(newFullPath))
            {
                File.Delete(newFullPath);
            }
            FileI.MoveTo(newFullPath);
        }

        public static void TouchDirectory(string Path)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }

        public static void Lock(string Hash)
        {
            try
            {
                var filePath = Config.I.LockDirectory + @"\" + Hash;
                if (File.Exists(filePath))
                {
                    return;
                }
                var fi = new FileInfo(filePath);
                TouchDirectory(fi.DirectoryName);
                FileUtils.SaveTextToFile(filePath, "");
            }
            catch { }
        }

        public static void Unlock(string Hash)
        {
            try
            {
                var filePath = Config.I.LockDirectory + @"\" + Hash;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch { }
        }

        public static void UnlockFolder(string HashFolder)
        {
            try
            {
                var filePath = Config.I.LockDirectory + @"\" + HashFolder;
                var fi = new FileInfo(filePath);
                EmptyFolder(fi.DirectoryName);
            }
            catch { }
        }

        public static bool IsLocked(string Hash)
        {
            try {
                var filePath = Config.I.LockDirectory + @"\" + Hash;
                return File.Exists(filePath);
            } catch
            {
                return false;
            }
        }

        public static bool Exists(string FilePath)
        {
            return File.Exists(FilePath);
        }

    }
}
