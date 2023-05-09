using System.Threading.Tasks;
using static SomeGame.Program;

namespace SomeGame
{
    public struct Vector
    {
        public int x, y;
        public Vector(int x = 0, int y = 0)
        {
            GPU.CheckVector(ref x, ref y);
            this.x = x;
            this.y = y;
        }
        public Task<GPUOutput> Out(object obj, double delay = 0) => gpu.Out(obj, x, y, delay);
        public Task<GPUOutput> OutVertical(object obj, double delay = 0) => gpu.OutVertical(obj, x, y, delay);
    }
}
