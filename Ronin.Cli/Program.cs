using System.Threading.Tasks;
using CliFx;

namespace Ronin.Cli;

public class Program
{
    public static async Task<int> Main()
    {
        return await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .Build()
            .RunAsync();
    }
}