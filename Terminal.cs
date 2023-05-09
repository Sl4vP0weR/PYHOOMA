using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static SomeGame.Program;
using Color = System.ConsoleColor;
using System.Linq.Expressions;

namespace SomeGame
{
    class WrongArguments : Exception
    {
        public readonly string
            Command,
            Syntax;

        public new string Message { get; set; } = null;

        public WrongArguments(string command = null, string syntax = null, string message = null)
        {
            Command = command;
            Syntax = syntax;
            Message = message;
            gpu.Out(this).Wait();
        }

        public override string ToString() => Message ?? $"Wrong arguments for command '{Command}'. Use: {Syntax}\n";
    }
    public class TerminalCommand
    {
        public readonly
        string
            Name,
            Help;
        public readonly Delegate Execute;
        public readonly List<string> Aliases = new List<string>();

        public TerminalCommand(string name, Delegate exe, string help, List<string> aliases = null)
        {
            Name = name;
            Help = help;
            Execute = exe;
            if(aliases != null)
                Aliases.AddRange(aliases);
        }
    }
    public class Terminal : Component<Terminal>
    {
        static readonly Dictionary<string, TerminalCommand> Cmds = new Dictionary<string, TerminalCommand>();
        static string Register(string cmd, Func<string, List<string>, Task> exe, string help, params string[] alias)
        {
            cmd = cmd.ToLower();
            Cmds[cmd] = new TerminalCommand(cmd, exe, help, alias == null ? null : alias.ToList());
            return cmd;
        }
        static Terminal()
        {
            Register("connect", async (cmd, args) =>
            {
                var ssid = args.ElementAtOrDefault(0);
                var pass = args.ElementAtOrDefault(1);
                if (ssid == null)
                    throw new WrongArguments(cmd, $"{cmd} ssid [password]");

                await gpu.Out("Connection establishing");
                await gpu.Dots(() => gpu.Out("...", delay: 0.25), times: 2);
                switch (wifi.Connect(ssid, pass))
                {
                    case EthernetConnectionResult.Connected:
                        await gpu.Out($"\nConnected: {wifi.CurrentConnection}", fg: Color.Green);
                        break;
                    case EthernetConnectionResult.Failed:
                        await gpu.Out("\nFailed to connect.", fg: Color.Red);
                        break;
                    case EthernetConnectionResult.IncorrectPassword:
                        await gpu.Out("\nIncorrect password!", fg: Color.Red);
                        break;
                    case EthernetConnectionResult.PasswordRequired:
                        await gpu.Out("\nPassword is required!", fg: Color.Yellow);
                        break;
                }
            }, "Connects to some ethernet node");
            Register("nets", async (cmd, args) =>
            {
                if (wifi.AvailableNodes.Count == 0)
                    throw new WrongArguments(message: "No networks available.");
                await gpu.Out($"Available networks:");
                foreach (var net in wifi.AvailableNodes)
                {
                    await gpu.Out($"\n{net.SSID} ", fg: net.SSID == wifi.CurrentConnection?.Parent.SSID ? Color.DarkGreen : Color.White);
                    var sec = net.CorrectPassword(null) == EthernetConnectionResult.PasswordRequired;
                    await gpu.Out(sec ? "[Secured]" : "[Not secured]", fg: sec ? Color.Red : Color.Yellow);
                }
            },
            "Scans for available networks", "netscan");
            Register("help", async (cmd, args) =>
            {
                var p = string.Join(" ", args).Trim();
                if (!int.TryParse(p, out var page))
                {
                    page = 1;
                    cmd = FindByAlias(p);
                    if (Cmds.ContainsKey(cmd))
                    {
                        await gpu.Out($"{p} - {Cmds[cmd].Help}");
                        return;
                    }
                }
                var lines = 3;
                var pages = (int)Math.Ceiling((decimal)Cmds.Count/lines);
                if (page > pages)
                    page = 1;
                page--;
                var cmds = Cmds.Select(x => x.Value).ToList();
                await gpu.Out($"Page {page + 1}/{pages}\n");
                for(int i = 0; i<lines; i++)
                {
                    var v = cmds[i+lines*page];
                    var a = string.Join("|", v.Aliases);
                    await gpu.Out($"{v.Name} {(a.Length > 0 ? "or " + a : "")} - {v.Help}\n");
                }
            }, "Shows help for all commands");
        }
        static string FindByAlias(string str)
        {
            var a = Cmds.Where(x => x.Value.Aliases.Contains(str));
            if (a.Count() > 0) return a.First().Key;
            else return str;
        }
        public async Task Execute(string cmd)
        {
            var args = (from Match m in Regex.Matches(cmd, "[\\\"](.+?)[\\\"]|([^ ]+)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled)
                        select m.Value.Trim(new char[]
                        {
                            '"'
                        }).Trim()).ToList();
            if (!args.Any())
                return;
            cmd = args[0].ToLower();
            args.RemoveAt(0);
            try
            {
                cmd = FindByAlias(cmd);
                if (Cmds.TryGetValue(cmd, out var f))
                    await (Task)f.Execute.DynamicInvoke(cmd, args);
                else throw new WrongArguments(message: "Unknown command.");
            }
            catch { }
        }
        protected override async Task Load(params object[] args)
        {
            while (true)
            {
                await gpu.Out("root@pyhooma", fg: Color.Green);
                await gpu.Out(":");
                await gpu.Out("~", fg: Color.DarkCyan);
                await gpu.Out("$ ");
                var cmd = Console.ReadLine();
                await Execute(cmd);
                await gpu.Out("\n");
            }
        }
    }
}
