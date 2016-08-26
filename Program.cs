using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyData
{
    class Program
    {
        private  static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs,DateTime creationDateTime)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var  dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                if(file.CreationTime.Date.ToUniversalTime()>=creationDateTime)
                    continue;
                CopyFileAsync(file, destDirName);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (!copySubDirs)
                return;
            
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, true, creationDateTime);
                }
        }

        private static async void CopyFileAsync(FileInfo file,string destDirName)
        {
            var temppath = Path.Combine(destDirName, file.Name);
            try
            {
                using (var sourceStream = File.Open(file.FullName, FileMode.Open))
                {
                    using (var destinationStream = File.Create(temppath))
                    {
                        try
                        {
                            Console.WriteLine($"Copying of {file.Name} is starting");
                            await sourceStream.CopyToAsync(destinationStream);
                            Console.WriteLine($"File {file.Name} was copied successfully");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"File {file.Name} was not copied :(");
                        }
                    }
                }
            }
            catch (Exception)
            {
                
                Console.WriteLine($"{file.Name}- can't be open");
            }
            
        }

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Not enough arguments for work");
                Console.ReadKey();
                return;
            }
            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("Can not copy. Folder is not exists");
                Console.ReadKey();
                return;
            }
            DirectoryCopy(args[0],args[1],true,DateTime.Parse(args[2]).Date.ToUniversalTime());
            Console.ReadKey();
        }
    }
}
