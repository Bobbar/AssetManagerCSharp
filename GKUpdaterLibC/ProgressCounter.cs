using System;

namespace RemoteFileTransferTool
{
    public class ProgressCounter
    {
        #region "Fields"

        private long _currentTick;
        private long _progBytesMoved;
        private long _progTotalBytes;
        private long _speedBytesMoved;
        private double _speedThroughput;

        private long _startTick;

        #endregion "Fields"

        #region "Constructors"

        public ProgressCounter()
        {
            _progBytesMoved = 0;
            _progTotalBytes = 0;
            _speedBytesMoved = 0;
            _currentTick = 0;
            _startTick = 0;
            _speedThroughput = 0;
        }

        #endregion "Constructors"

        #region "Properties"

        public long BytesMoved
        {
            get { return _progBytesMoved; }
            set
            {
                _speedBytesMoved += value;
                _progBytesMoved += value;
            }
        }

        public long BytesToTransfer
        {
            get { return _progTotalBytes; }
            set { _progTotalBytes = value; }
        }

        public int Percent
        {
            get
            {
                if (_progTotalBytes > 0)
                {
                    return (int)Math.Round((((float)_progBytesMoved / (float)_progTotalBytes) * 100), 0);
                }
                else
                {
                    return 0;
                }
            }
        }

        public double Throughput
        {
            get { return _speedThroughput; }
        }

        #endregion "Properties"

        #region "Methods"

        public void ResetProgress()
        {
            _progBytesMoved = 0;
        }

        public void Tick()
        {
            _currentTick = DateTime.Now.Ticks;
            if (_startTick > 0)
            {
                if (_speedBytesMoved > 0)
                {
                    double elapTime = _currentTick - _startTick;
                    _speedThroughput = Math.Round((_speedBytesMoved / (elapTime / 10000)) / 1000, 2);
                }
            }
            else
            {
                _startTick = _currentTick;
            }
        }

        #endregion "Methods"
    }
}