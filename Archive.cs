using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
namespace k3rn3lpanicTools
{
    public class Archive
    {
        /// <summary>
        /// Makes a Zip file out of a Folder
        /// </summary>
        /// <param name="FolderPath"></param>
        /// <param name="ArchiveName"></param>
        public static void ZipFolder(string FolderPath,string ArchiveName) {
            ZipFile.CreateFromDirectory(FolderPath,ArchiveName);
        }
        /// <summary>
        /// Extracts the zip file into a folder
        /// </summary>
        /// <param name="ArchiveName"></param>
        /// <param name="Decrypted_FolderName"></param>
        public static void UnZipFolder(string ArchiveName,string Decrypted_FolderName)
        {
            ZipFile.ExtractToDirectory(ArchiveName,Decrypted_FolderName);
        }
    }
}
