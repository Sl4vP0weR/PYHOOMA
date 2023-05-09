namespace SomeGame
{
    public class EthernetConnection
    {
        public EthernetConnection(EthernetNode net, string mac, string ip, string lanip)
        {
            MAC = mac;
            LanIP = lanip;
            PublicIP = ip;
            Parent = net;
        }
        public readonly string MAC;
        public readonly string PublicIP;
        public readonly string LanIP;
        public readonly EthernetNode Parent;
        public override string ToString() => $"{MAC} - {PublicIP}[{LanIP}]";
    }
}
