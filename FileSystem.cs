using System;
using System.Linq;
using System.Threading.Tasks;
using static SomeGame.Program;
using Color = System.ConsoleColor;
using System.IO;
using System.Collections.Generic;

namespace SomeGame
{
    public abstract class FileSystemElement
    {
        public FileSystemElement()
        {
            if(Path != null)
                Path = System.IO.Path.Combine(Parent.Path, Path);
        }
        public string Name { get; protected set; }
        public string Path { get; protected set; }
        public Directory Parent { get; protected set; } = null;
        abstract public void Rename(string newName);
        abstract public void Move(string newPath);
    }
    public class Directory : FileSystemElement
    {
        public readonly List<FileSystemElement> Childs = new List<FileSystemElement>();

        public Directory(string name, Directory parent = null)
        {
            Name = name;
            Path = name;
        }

        public override void Move(string newPath)
        {
            if (!Uri.IsWellFormedUriString(newPath, UriKind.RelativeOrAbsolute))
                return;
            Path = newPath; 
        }

        public override void Rename(string newName)
        {
            Name = newName;
            Move(System.IO.Path.Combine(Parent.Path, Name));
        }
    }
    public class File : FileSystemElement
    {
        public byte[] Content = new byte[0];
        public File(string name, Directory parent = null)
        {
            Name = name;
            Path = name;
        }
        public string FullName { get; protected set; }
        public bool IsSystem = false;
        public bool ReadOnly = false;

        public override void Rename(string newName)
        {
            FullName = newName;
        }

        public override void Move(string newPath)
        {

        }
    }
    public class FileSystem : Component<FileSystem>
    {
        public Directory Drive = new Directory("/");
        protected override async Task Load(params object[] args)
        {
            await gpu.Out("Filesystem Initialization");
            await gpu.Dots(() => gpu.Out("...", delay: 0.25), times: 2);
            await gpu.Out("\n");
        }
    }
}
