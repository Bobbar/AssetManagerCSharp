using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AssetManager
{
    internal class TestMessageFilter : IMessageFilter
    {
        private long lastMessage = 0;
        private long msgCount = 0;
        private List<double> lastXElap = new List<double>();
        private int avgCount = 30;

        public bool PreFilterMessage(ref Message m)
        {
            if (lastMessage == 0)
            {
                lastMessage = DateTime.Now.Ticks;
            }
            else
            {
                var now = DateTime.Now.Ticks;
                var elap = now - lastMessage;
                lastMessage = now;

                if (elap > 0)
                {
                    var ms = elap / 10000;

                    if (lastXElap.Count < avgCount)
                    {
                        lastXElap.Add(ms);
                    }
                    else if (lastXElap.Count >= avgCount)
                    {
                        lastXElap.RemoveAt(0);
                        lastXElap.Add(ms);
                    }

                    double tot = 0;
                    lastXElap.ForEach((v) => tot += v);

                    var avg = tot / (float)lastXElap.Count();
                    var msgPerSecond = 1000 / avg;

                    Console.WriteLine("Count: " + msgCount + "   Msgs Per Sec: " + msgPerSecond);// + "  Msg: " + m.ToString());
                }
                msgCount++;
            }

          
            return false;
        }
    }
}