using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ronin.Updater.Update
{
    public class FileCopier
    {
        public static async Task<List<(string SourceFile, string TargetFile)>> GetCollidingFiles(IEnumerable<(string Source, string Target)> files)
        {
            var result = new List<(string SourceFile, string TargetFile)>();
            foreach (var (source, target) in files)
            {
                if (await IsCollidingFile(source, target))
                    result.Add(new (source, target));
            }

            return result;
        }

        public static async Task<bool> IsCollidingFile(string source, string target)
        {
            var summer = new Md5Summer();
            if (!File.Exists(source) || !File.Exists(target))
                throw new Exception("Source or Target doesn't exist");

            var md5Source = await summer.SumFile(source);
            var md5Target = await summer.SumFile(target);

            if (md5Source.SequenceEqual(md5Target))
                return false;

            return true;
        }

        public void CopyFiles(IEnumerable<string> files, string destination)
        {

        }
    }
}
