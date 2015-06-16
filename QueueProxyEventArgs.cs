using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FtpClient
{
    
    public class QueueConnectedEventArgs : EventArgs
    {
        public DateTime ConnectedTimeStamp { get; set; }
        public string QueueName { get; set; }

        public static QueueConnectedEventArgs Factory(string queueName)
        {
            return new QueueConnectedEventArgs() { QueueName = queueName, ConnectedTimeStamp = DateTime.Now };
        }
    }

    public class QueueDisconnectedEventArgs : EventArgs
    {
        public DateTime ConnectedTimeStamp { get; set; }
        public string QueueName { get; set; }

        public static QueueDisconnectedEventArgs Factory(string queueName)
        {
            return new QueueDisconnectedEventArgs() { QueueName = queueName, ConnectedTimeStamp = DateTime.Now };
        }
    }
    
}
