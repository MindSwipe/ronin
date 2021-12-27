using System.IO;
using System.IO.Compression;

namespace Ronin.Updater
{
    public static class Unpacker
    {
        public static void UnpackIntoSameFolderZip(string zip)
        {
            var dirName = Path.GetDirectoryName(zip);
            if (!File.Exists(zip) || !Directory.Exists(dirName))
                return;

            ZipFile.ExtractToDirectory(zip, dirName!);
        }
    }
}
