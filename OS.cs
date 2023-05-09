using System;
using System.Linq;
using System.Threading.Tasks;
using static SomeGame.Program;
using Color = System.ConsoleColor;

namespace SomeGame
{
    public class OS : Component<OS>
    {
        public static Task Delay(double seconds) => seconds > 0 ? Task.Delay(TimeSpan.FromSeconds(seconds)) : Task.CompletedTask;
        public static async Task Delay(Task task, double seconds) => await Delay(seconds).ContinueWith(x => task);
        public async Task Beep(int frequency = 0, int duration = 0, int times = 1, double delay = 0)
        {
            for (int i = 0; i < times; i++)
            {
                await Delay(delay);
                if (frequency == 0 || duration == 0)
                    Console.Beep();
                else
                    Console.Beep(frequency, duration);
            }
        }
        protected override async Task Load(params object[] args)
        {
            var logoPos = new Vector(Console.WindowWidth - 2, 1);
            var logo = await gpu.GetASCIIArt("PYHOOMA", "Colossal");
            await gpu.Borders((l, y) => gpu.Out(l, logoPos.x - l.Length + 1, logoPos.y + y - 1), GPU.Border1, logo.Width + 3, logo.Height - 1);
            await gpu.OutASCIIArt((l, y) => gpu.Out(l.All(x => x == ' ') ? "" : l, logoPos.x - l.Length, logoPos.y + y, fg: Color.Cyan), logo);

            await gpu.Out("Initialization of system", 0, 0);
            await gpu.Dots(() => gpu.Out("...", delay: 0.25), times: 3);
            await gpu.Out("\n");
            await Beep();
        }
    }
}
