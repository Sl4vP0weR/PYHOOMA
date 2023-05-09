using System;
using System.Threading.Tasks;

namespace SomeGame
{
    public abstract class Component<T> where T : Component<T>
    {
        protected abstract Task Load(params object[] args);
        public static T Initialize(params object[] args)
        {
            var t = Activator.CreateInstance<T>();
            t.Load(args).Wait();
            return t;
        }
    }
}
