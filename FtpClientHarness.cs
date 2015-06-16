using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Ionic.Zip;
using System.Xml.Linq;
using Apache.NMS;

namespace FtpClient
{
    public partial class FtpClientHarness : Form
    {
        List<string> fileList;

        FtpSignalGateway gateway;

        QueueProxy qProxy;

        public FtpClientHarness()
        {
            InitializeComponent();

            this.fileList = new List<string>();

            this.gateway = FtpSignalGateway.Factory();

            this.gateway.FtpFileReceived += new EventHandler<FtpFileReceivedEventArgs>(gateway_FtpFileReceived);

            this.qProxy = QueueProxy.Factory(QueueProxy.ServiceBusUriFactory("devadmin.pearlnet.net", 61616), "osi.zipsignals", "mroot", "password");

        }

        void gateway_FtpFileReceived(object sender, FtpFileReceivedEventArgs e)
        {
            string andy = string.Empty;

            this.fileList.Add(e.FileName);

            XDocument doc = new XDocument();

            ZipExtractor.Extract(e.FileName, false, ref doc);

            string fname = e.FileName;
        }

        private void btnGetFiles_Click(object sender, EventArgs e)
        {
            
        }

        private void btnCheckFiles_Click(object sender, EventArgs e)
        {

            string andy = string.Empty;
        }

        private void SendFileToBus()
        {

        }
    }


}
