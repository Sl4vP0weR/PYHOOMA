using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SomeGame
{
    public static class Program
    {
        public static GPU gpu;
        public static Ethernet wifi;
        public static Terminal term;
        public static OS os;
        public static FileSystem io;
        static void Main(string[] args)
        {
            AsyncMain(args).Wait();
        }
        static async Task AsyncMain(string[] args)
        {
            gpu = GPU.Initialize();
            os = OS.Initialize();
            io = FileSystem.Initialize();
            wifi = Ethernet.Initialize();
            //
            term = Terminal.Initialize();
        }
    }
}
