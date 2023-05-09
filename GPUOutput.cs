using System;
using System.Threading.Tasks;

namespace SomeGame
{
    public class GPUOutput
    {
        public Vector pos;
        public object obj;
        public bool vertical = false;
        public async Task<GPUOutput> Clear(double delay = 0, double wdelay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            var s = GPU.ToString(' ', obj.ToString().Length);
            return await (vertical ? pos.OutVertical(s, wdelay) : pos.Out(s, wdelay));
        }
    }
}
