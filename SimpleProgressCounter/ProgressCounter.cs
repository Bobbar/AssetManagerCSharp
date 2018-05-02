using System;

namespace SimpleProgressCounter
{
    public class ProgressCounter
    {
        #region "Fields"

        private long startTick;
        private long currentTick;
        private long progBytesMoved;
        private long progTotalBytes;
        private long speedBytesMoved;
        private double speedThroughput;

        #endregion "Fields"

        #region "Constructors"

        public ProgressCounter()
        {
            progBytesMoved = 0;
            progTotalBytes = 0;
            speedBytesMoved = 0;
            currentTick = 0;
            startTick = 0;
            speedThroughput = 0;
        }

        #endregion "Constructors"

        #region "Properties"

        public long BytesMoved
        {
            get { return progBytesMoved; }
            set
            {
                speedBytesMoved += value;
                progBytesMoved += value;
            }
        }

        public long BytesToTransfer
        {
            get { return progTotalBytes; }
            set { progTotalBytes = value; }
        }

        public int Percent
        {
            get
            {
                if (progTotalBytes > 0)
                {
                    return (int)Math.Round(((progBytesMoved / (float)progTotalBytes) * 100), 0);
                }
                else
                {
                    return 0;
                }
            }
        }

        public double Throughput
        {
            get { return speedThroughput; }
        }

        #endregion "Properties"

        #region "Methods"

        public void ResetProgress()
        {
            progBytesMoved = 0;
        }

        public void Tick()
        {
            currentTick = DateTime.Now.Ticks;

            if (startTick > 0 & speedBytesMoved > 0)
            {
                double elapTimeMs = (currentTick - startTick) / 10000;
                speedThroughput = Math.Round((speedBytesMoved / elapTimeMs) / 1000, 2);
            }
            else
            {
                startTick = currentTick;
            }
        }

        #endregion "Methods"
    }
}
