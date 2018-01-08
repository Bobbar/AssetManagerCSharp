using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace AssetManager.Tools.NetworkScanning
{
    public class Pinger
    {

        private Ping myPinger;
        private string pingIp;
        private PingReply pingReply;
        private string hostName;

        private const int timeOut = 1000;

        public event EventHandler PingComplete;

        protected virtual void OnPingComplete(PingerCompleteEventArgs e)
        {
            PingComplete(this, e);
            myPinger.Dispose();
        }

        public string PingIP
        {
            get
            {
                return pingIp;
            }
        }


        public Pinger(string pingIp)
        {
            this.pingIp = pingIp;
            myPinger = new Ping();

        }

        public async void StartPing()
        {
            hostName = ResolveHostname(pingIp);

            pingReply = await GetPingReply(pingIp);

            bool success;
            if (pingReply.Status == IPStatus.Success)
            {
                success = true;
            }
            else
            {
                success = false;
            }

            OnPingComplete(new PingerCompleteEventArgs(success, hostName, pingIp));

        }

        private async Task<PingReply> GetPingReply(string ip)
        {
            var pingOptions = new PingOptions();
            pingOptions.DontFragment = true;

            byte[] pingBuffer = Encoding.ASCII.GetBytes("ping");

            return await myPinger.SendPingAsync(pingIp, timeOut, pingBuffer, pingOptions);

        }

        private string ResolveHostname(string ip)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ip);
                if (entry != null)
                {
                    return entry.HostName;
                }

            }
            catch (SocketException ex)
            {
                // Console.WriteLine(ex.Message);

            }
            return string.Empty;
        }

        public class PingerCompleteEventArgs : EventArgs
        {
            public bool Success { get; }
            public string Hostname { get; }
            public string PingIP { get; }

            public PingerCompleteEventArgs(bool success, string hostname, string pingIp)
            {
                Success = success;
                Hostname = hostname;
                PingIP = pingIp;
            }


        }


    }
}
