using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.NMS;
using Apache.NMS.Util;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Net.Sockets;

namespace FtpClient
{
    public class QueueProxy
    {
        public static readonly string GROUP_ID = "GroupID";
        public static readonly string SIGNAL_SET_NAME = "SignalSetName";

        public event EventHandler<QueueConnectedEventArgs> QueueConnected;
        public event EventHandler<QueueDisconnectedEventArgs> QueueDisconnected;

        System.Timers.Timer busWatchdog;

        Uri connecturi;
        IConnection connection;
        ISession session;

        IMessageProducer producer;
        IDestination destination;

        string serviceBusFqdn = string.Empty;
        int serviceBusPort = 61616;
        string userName = string.Empty;
        string userPwd = string.Empty;
        string queueName = string.Empty;

        bool connected = false;

        public bool Connected
        {
            get { return this.connected; }
        }

        bool ServiceBusConnected
        {
            get { return Connected; }
            set 
            {
                connected = value;

                if (value)
                    OnQueueConnected(queueName);
                else
                    OnQueueDisconnected(queueName);
            }
        }

        private QueueProxy(Uri serviceBusURI)
        {
            try
            {
                connecturi = serviceBusURI;

                busWatchdog = new System.Timers.Timer(5000);
            }
            catch
            {
            }
        }

        //public static QueueProxy Factory(Uri serviceBusURI, string queueName, string queueUser, string queueUserPwd)
        //{
        //    return new QueueProxy(serviceBusURI, queueName, queueUser, queueUserPwd);
        //}

        public static QueueProxy Factory(string serverFqdn, int port, string queueName, string queueUser, string queueUserPwd)
        {
            QueueProxy proxy = new QueueProxy(ServiceBusUriFactory(serverFqdn, port));

            try
            {
                proxy.queueName = queueName;
                proxy.userName = queueUser;
                proxy.userPwd = queueUserPwd;

                proxy.serviceBusFqdn = serverFqdn;
                proxy.serviceBusPort = port;

                proxy.AttachServiceBus();
            }
            catch (Exception ex)
            {
            }

            return proxy;
        }

        public static Uri ServiceBusUriFactory(string serverFqdn, int port)
        {
            string address = string.Format("activemq:tcp://{0}:{1}", serverFqdn, port);

            return new Uri(address);
        }

        protected virtual void OnQueueConnected(string queueName)
        {
            try
            {
                EventHandler<QueueConnectedEventArgs> handler = QueueConnected;

                if (handler != null)
                {
                    handler(this, QueueConnectedEventArgs.Factory(queueName));
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        protected virtual void OnQueueDisconnected(string queueName)
        {
            try
            {
                EventHandler<QueueDisconnectedEventArgs> handler = QueueDisconnected;

                if (handler != null)
                {
                    handler(this, QueueDisconnectedEventArgs.Factory(queueName));
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public bool SendMessage(string fileName, XDocument doc)
        {
            bool retVal = true;

            try
            {
                ITextMessage signalSet = session.CreateTextMessage(doc.ToString());

                signalSet.NMSCorrelationID = "MTNSignal.SCC";
                signalSet.Properties[QueueProxy.GROUP_ID] = "OSI";
                signalSet.Properties[QueueProxy.SIGNAL_SET_NAME] = (string.Empty.Equals(fileName)) ? Guid.NewGuid().ToString() : fileName;

                producer.Send(signalSet);
            }
            catch (Exception ex)
            {
                retVal = false;
            }
            finally
            {
            }

            return retVal;
        }

        void CheckServiceBusStatus(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ServiceBusConnected)
                return;

            if (BusIsAvailable() == false)
                return;

            (sender as System.Timers.Timer).Stop();

            if (AttachServiceBus(false))
            {
                

                

            }

        }

        void OnServiceBusException(Exception exception)
        {
            DetachServiceBus();
        }

        private void ActivateConnectionListeners(ref IMessageConsumer msgConsumer, ref MessageListener listener)
        {
            connection.ExceptionListener += new ExceptionListener(OnServiceBusException);
            msgConsumer.Listener += listener;
        }

        private void DeactivateConnectionListeners(ref IMessageConsumer msgConsumer, ref MessageListener listener)
        {
            connection.ExceptionListener -= OnServiceBusException;
            msgConsumer.Listener -= listener;
        }

        bool AttachServiceBus(bool addListener = false)
        {
            string andy = string.Empty;

            bool retVal = true;

            try
            {
                if (ServiceBusConnected || BusIsAvailable() == false)
                    return false;

                connection = NMSConnectionFactory.CreateConnectionFactory(connecturi).CreateConnection(userName, userPwd);

                session = connection.CreateSession();

                connection.Start();

                destination = SessionUtil.GetDestination(session, queueName, DestinationType.Queue);

                producer = session.CreateProducer(destination);

                ServiceBusConnected = true;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }

            return retVal;
        }

        bool BusIsAvailable()
        {
            bool retVal = true;

            using (TcpClient bus = new TcpClient())
            {

                try
                {
                    bus.Connect(serviceBusFqdn, serviceBusPort);

                    bus.Close();
                }
                catch
                {
                    retVal = false;

                    DetachServiceBus();
                }
            }

            return retVal;

        }

        private void DetachServiceBus()
        {
            if (ServiceBusConnected == false)
                return;

            ServiceBusConnected = false;

            producer.Dispose();
            session.Dispose();
            connection.Dispose();

            producer = null;
            session = null;
            connection = null;
        }
    }

     
    [Serializable]
    public class ServiceBusNotAvailableException : Exception
    {
        public ServiceBusNotAvailableException() { }
        public ServiceBusNotAvailableException( string message ) : base( message ) { }
        public ServiceBusNotAvailableException( string message, Exception inner ) : base( message, inner ) { }
        protected ServiceBusNotAvailableException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }
    }
}
