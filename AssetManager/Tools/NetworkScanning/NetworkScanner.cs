using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AssetManager.Tools.NetworkScanning
{
    class NetworkScanner
    {
        private System.Timers.Timer threadTimer;
        private List<string> pingResults = new List<string>();

        private const int maxThreads = 60;

        private int currentThreads = 0;

        private string subnet = "10.10.83.";
        private int currentOctet = 0;
        private int lastOctet = 255;

        private object thislock = new object();

        public NetworkScanner()
        {
            InitTimer();
        }

        public void StartScan()
        {

            //foreach (string subnet in NetworkInfo.SubnetLocations.Keys)
            //{
            //}

            //var subnet = "10.10.80.";

            //for (int i = 0; i < 255; i++)
            //{
            //    var currentIp = subnet + i.ToString();
            //    Console.WriteLine(currentIp);
            //    Pinger pinger = new Pinger(currentIp);
            //    Thread t = new Thread(pinger.StartPing);
            //    pinger.PingComplete += PingComplete;
            //    t.Start();

            //}




            //int currentOctet = 0;
            // int lastOctet = 255;

            //do
            //{
            //    var currentIp = subnet + currentOctet.ToString();


            //} while (currentOctet < lastOctet);


            threadTimer.Start();

        }
        private void InitTimer()
        {
            threadTimer = new System.Timers.Timer();
            threadTimer.Interval = 50;
            threadTimer.Enabled = true;
            threadTimer.Elapsed += TimerTick;
            //  threadTimer.Start();


        }

        private void TimerTick(object sender, EventArgs e)
        {

            if (currentThreads < maxThreads && currentOctet < (lastOctet + 1))
            {
                var currentIp = subnet + currentOctet.ToString();
                StartPingThread(currentIp);
                currentOctet++;
            }


        }


        private void StartPingThread(string ip)
        {

            Console.WriteLine("Starting: " + ip);
            Pinger pinger = new Pinger(ip);
            Thread t = new Thread(pinger.StartPing);
            pinger.PingComplete += PingComplete;
            t.Start();

            currentThreads++;

        }

        private void PingComplete(object sender, EventArgs e)
        {
            var pingEvent = (Pinger.PingerCompleteEventArgs)e;

            if (!string.IsNullOrEmpty(pingEvent.Hostname) || pingEvent.Success)
            {
                var pingResult = pingEvent.PingIP + " - " + pingEvent.Hostname + " - " + pingEvent.Success.ToString();
                pingResults.Add(pingResult);
                Console.WriteLine(pingResult);
            }

            lock (thislock)
            {

                currentThreads--;
            }

            if (ScanComplete())
            {
                Console.WriteLine("Scan Complete: " + pingResults.Count.ToString());
                threadTimer.Stop();
            }

        }

        private bool ScanComplete()
        {
            if (currentOctet >= lastOctet && currentThreads == 0)
            {
                return true;
            }
            return false;
        }


    }

}
