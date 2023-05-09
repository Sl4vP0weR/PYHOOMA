using System.Collections.Generic;
using System.Threading.Tasks;
using static SomeGame.Program;

namespace SomeGame
{
    public class Ethernet : Component<Ethernet>
    {
        public EthernetConnection CurrentConnection { get; internal set; }
        public readonly List<EthernetNode> AvailableNodes = new List<EthernetNode>();
        public readonly List<ushort> OpenedPorts = new List<ushort>();
        /*public bool SendPort(ushort port, byte[] data)
        {

        }*/
        public virtual bool OpenPort(Ethernet from, ushort port)
        {
            if (from == this)
            {
                if (!OpenedPorts.Contains(port))
                    OpenedPorts.Add(port);
                return true;
            }
            return false;
        }
        public virtual bool ClosePort(Ethernet from, ushort port)
        {
            if (from == this)
                return OpenedPorts.Remove(port);
            return false;
        }
        public EthernetConnectionResult Connect(string ssid, string pass)
        {
            var idx = AvailableNodes.FindIndex(x => x.SSID.ToLowerInvariant() == ssid.ToLowerInvariant());
            if (idx != -1)
                return AvailableNodes[idx].Connect(this, pass);
            return EthernetConnectionResult.Failed;
        }
        public bool Disconnect()
        {
            if (CurrentConnection == null)
                return false;
            return CurrentConnection.Parent.Disconnect(this);
        }
        protected override async Task Load(params object[] args)
        {
            AvailableNodes.Add(new EthernetNode("ceq", "123"));
        }
    }
}
