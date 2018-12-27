using System;

namespace Library1
{
    public class DataDvcStatus
    {
        public DataDvcStatus()
        {
            ErrorNo = 0;
            TimeStamp = new DateTime();
            Ok = false;
        }

        public DataDvcStatus(int errorNo, DateTime timeStamp)
        {
            ErrorNo = errorNo;
            TimeStamp = timeStamp;
            Ok = ErrorNo == 0;
        }

        public bool Ok { get; private set; }

        public int ErrorNo { get; set; }

        public DateTime TimeStamp { get; set; }

        public override string ToString()
        {
            return $"Ok={Ok}, ErrorNo={ErrorNo}, Timestamp={TimeStamp}";
        }
    }
}