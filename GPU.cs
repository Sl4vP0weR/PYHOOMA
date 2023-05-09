using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WenceyWang.FIGlet;
using Color = System.ConsoleColor;

namespace SomeGame
{
    public class GPU : Component<GPU>
    {
        public static void CheckVector(ref int x, ref int y)
        {
            if (x < 0)
                x = Console.CursorLeft;
            if (y < 0)
                y = Console.CursorTop;
        }
        public static string ToString(string c, int times = 1)
        {
            string s = "";
            for (int i = 0; i < times; i++)
                s += c;
            return s;
        }
        public static string ToString(char c, int times = 1) => ToString(c.ToString(), times);
        public bool CheckStr(object obj) => obj != null && !string.IsNullOrEmpty(obj.ToString());
        public bool PrepareOut(object obj, int x = -1, int y = -1, double delay = 0, Color fg = Color.White, Color bg = Color.Black)
        {
            CheckVector(ref x, ref y);
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            return CheckStr(obj);
        }
        public async Task<GPUOutput> Out(object obj, int x = -1, int y = -1, double delay = 0, Color fg = Color.White, Color bg = Color.Black)
        {
            if (!PrepareOut(obj, x, y, delay, fg, bg))
                return null;
            if (delay == 0) Console.Write(obj);
            else
            {
                var str = obj.ToString();
                for (int i = 0; i < str.Length; i++)
                {
                    Console.Write(str[i]);
                    await OS.Delay(delay);
                }
            }
            return new GPUOutput { obj = obj, pos = new Vector(x, y) };
        }
        public async Task<GPUOutput> OutVertical(object obj, int x = -1, int y = -1, double delay = 0, Color fg = Color.White, Color bg = Color.Black)
        {
            if (!PrepareOut(obj, x, y, delay, fg, bg))
                return null;
            var str = obj.ToString();
            for (int i = 0; i < str.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(str[i]);
                await OS.Delay(delay);
            }
            return new GPUOutput { obj = obj, pos = new Vector(x, y), vertical = true };
        }
        public async Task<T> Question<T>(Task output, T def = default)
        {
            output.Wait();
            if (def is bool b)
            {
                object v = Console.ReadKey(true).Key == ConsoleKey.Y ? true : b;
                await Out(((bool)v ? 'Y' : 'N') + "\n");
                return (T)v;
            }
            else if (def is string)
            {
                object v = Console.ReadLine();
                if (string.IsNullOrEmpty(v.ToString())) return def;
                return (T)v;
            }
            else return (dynamic)Console.ReadLine();
        }

        public static Border Border1 = new Border('┌', '└', '┐', '┘', '─', '─', '|', '|');

        public async Task Borders(Func<string, int, Task<GPUOutput>> output, Border border, int w = 1, int h = 1)
        {
            await output(border.LT + ToString(border.T, w - 2) + border.RT, 0);
            for (int i = 1; i < h-1; i++) await output(border.L + ToString(" ", w-2)+ border.R, i);
            await output(border.LB + ToString(border.B, w - 2) + border.RB, h-1);
        }

        public async Task Dots(Func<Task<GPUOutput>> output, int times = 1, double delay = 0.1)
        {
            for (int i = 0; i < times; i++)
            {
                await OS.Delay(delay);
                var dots = await output();
                dots.pos.x -= dots.obj.ToString().Length;
                if (i != times - 1)
                {
                    await dots.Clear(delay);
                    await dots.pos.Out(null);
                }
            }
        }
        public async Task<AsciiArt> GetASCIIArt(string text, string fontname)
        {
            using (var font = System.IO.File.OpenRead($"fonts/{fontname}.flf"))
            {
                var art = new AsciiArt(
                    text,
                    new FIGletFont(font));
                return art;
            }
        }
        public async Task OutASCIIArt(Func<string, int, Task<GPUOutput>> output, AsciiArt art)
        {
            for (int i = 0; i < art.Result.Length; i++)
                await output(art.Result[i], i);
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        private static void Maximize() => ShowWindow(GetConsoleWindow(), 3); //SW_MAXIMIZE = 3

        protected override async Task Load(params object[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Title = "Pyhooma";
            Maximize();
        }
    }
}
