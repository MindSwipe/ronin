using System;
using System.IO;

namespace Ronin.Updater
{
    internal static class AppDataCreator
    {
        internal static string ClearAndGetDownloadFolder()
        {
            var targetFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ronin", "Downloads");
            if (Directory.Exists(targetFolder))
                Directory.Delete(targetFolder, true);

            Directory.CreateDirectory(targetFolder);
            return targetFolder;
        }
    }
}
