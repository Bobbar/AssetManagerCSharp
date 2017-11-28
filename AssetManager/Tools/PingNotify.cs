using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace AssetManager.Tools
{

    class PingNotify
    {
        private Ping Pinger = new Ping();
        private System.Timers.Timer PingTimer = new System.Timers.Timer();
        private List<MonitorStats> MonitorList = new List<MonitorStats>();

        public PingNotify()
        {
            //PingTimer.Interval = 1;
            //PingTimer.Start();

        }

        public void AddDeviceToMonitor(string deviceGUID)
        {
            MonitorList.Add(new MonitorStats(deviceGUID));



        }

        private async void GetPingReplies()
        {
            var monitorListArr = MonitorList.ToArray();
            //foreach (MonitorStats dev in MonitorList)
            for (int i = 0; i < monitorListArr.Length; i++)
            {

                monitorListArr[i].LastReply = await Pinger.SendPingAsync(monitorListArr[i].MonitorDevice.HostName);


            }

            MonitorList = monitorListArr.ToList();



        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

            }
        }


        struct MonitorStats
        {
            //public string DeviceGUID { get; set; }
            public DeviceObject MonitorDevice { get; set; }
            public bool IsPinging { get; set; }
            public PingReply LastReply { get; set; }

            public MonitorStats(string deviceGUID)
            {
                // DeviceGUID = deviceGUID;
                MonitorDevice = new DeviceObject(deviceGUID);
                IsPinging = false;
                LastReply = null;
            }


        }

    }
}
