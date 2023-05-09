using System;
using System.Collections.Generic;
using System.Text;

namespace SomeGame
{
    public enum EthernetConnectionResult
    {
        IncorrectPassword,
        PasswordRequired,
        Connected,
        Failed
    }
    public class EthernetNode
    {
        public EthernetNode(string ssid, string pass)
        {
            this.ssid = ssid;
            password = pass;
        }
        public string SSID => ssid;
        protected string ssid;
        protected string password;
        public readonly List<EthernetConnection> Connections = new List<EthernetConnection>();
        public EthernetConnectionResult Connect(Ethernet eth, string pass = null)
        {
            if (eth == null)
                return EthernetConnectionResult.Failed;
            var pr = CorrectPassword(pass);
            if (pr == EthernetConnectionResult.Connected)
            {
                eth.CurrentConnection?.Parent.Disconnect(eth);
                var connection = new EthernetConnection(this, GenerateMAC(), GeneratePublic(), GenerateLan());
                Connections.Add(connection);
                eth.CurrentConnection = connection;
            }
            return pr;
        }
        public bool Disconnect(Ethernet eth)
        {
            if (eth == null || eth.CurrentConnection == null)
                return true;
            var idx = Connections.FindIndex(x => x.MAC == eth.CurrentConnection.MAC);
            if (idx != -1)
            {
                Connections.RemoveAt(idx);
                eth.CurrentConnection = null;
                return true;
            }
            return false;
        }
        protected virtual string GenerateMAC()
        {
            byte setBit(byte bs, int BitNumber, bool set)
            {
                if (BitNumber < 8 && BitNumber > -1)
                    return (byte)(bs | (byte)((set ? 0x01 : 0x00) << BitNumber));
                return 0;
            }
            var sBuilder = new StringBuilder();
            var r = new Random();
            int number;
            byte b;
            for (int i = 0; i < 6; i++)
            {
                number = r.Next(0, 255);
                b = Convert.ToByte(number);
                if (i == 0)
                {
                    b = setBit(b, 6, true);
                    b = setBit(b, 7, false);
                }
                sBuilder.Append(number.ToString("X2") + (i == 5 ? "" : ":"));
            }
            return sBuilder.ToString().ToUpper();
        }

        protected virtual string GenerateLan()
        {
            byte last = 1;
            foreach (var c in Connections)
            {
                var id = Convert.ToByte(c.LanIP.Split('.')[3]);
                if (id > last)
                    last = id;
            }
            return $"192.168.0.{last+1}";
        }
        protected virtual string GeneratePublic()
        {
            var random = new Random();
            return $"{random.Next(1, 254)}.{random.Next(0, 254)}.{random.Next(0, 254)}.{random.Next(0, 254)}";
        }
        public virtual EthernetConnectionResult CorrectPassword(string pass)
        {
            if (string.IsNullOrEmpty(password))
                return EthernetConnectionResult.Connected;
            if (string.IsNullOrEmpty(pass))
                return EthernetConnectionResult.PasswordRequired;
            if (password != pass)
                return EthernetConnectionResult.IncorrectPassword;
            return EthernetConnectionResult.Failed;
        }
    }
}
