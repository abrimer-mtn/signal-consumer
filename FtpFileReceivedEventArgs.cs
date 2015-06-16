using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FtpClient
{
    public class FtpFileReceivedEventArgs : EventArgs
    {
        public DateTime ReceivedTimeStamp { get; set; }
        public string FileName { get; set; }

        public static FtpFileReceivedEventArgs Factory(string fileName)
        {
            return new FtpFileReceivedEventArgs() { FileName = fileName, ReceivedTimeStamp = DateTime.Now };
        }
    }
}
