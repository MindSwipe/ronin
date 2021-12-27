using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Ronin.Updater.Update
{
    public class Md5Summer
    {
        public async Task<List<(string FileName, byte[] Md5Sum)>> SumFiles(IEnumerable<string> files)
        {
            var result = new List<(string FileName, byte[] Md5Sum)>();

            foreach (var file in files)
            {
                if (!File.Exists(file))
                    continue;

                result.Add(new (file, await SumFile(file)));
            }

            return result;
        }

        public async Task<byte[]> SumFile(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(null, file);

            using var md5 = MD5.Create();
            await using var stream = File.OpenRead(file);

            return await md5.ComputeHashAsync(stream);
        }
    }
}
