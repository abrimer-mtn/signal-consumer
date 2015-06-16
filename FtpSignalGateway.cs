using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace FtpClient
{
    public class FtpSignalGateway
    {
        public event EventHandler<FtpFileReceivedEventArgs> FtpFileReceived;

        private static double SYSTEM_CHECK_INTERVAL = 10000;

        IdempotentFtp idempotent;

        List<string> fileList;

        System.Timers.Timer cronFtp;


        static string serverip = "ftp.realtraffictech.com";
        static string remoteDir = "DATA";
        static string localDir = "C:\\home\\zip";
        static string localCacheDir = "C:\\home\\zip\\extracted";
        static string userid = "SCC";
        static string userpwd = "scc_1";

        public static string FullyQualifiedLocalFileName(string localFile)
        {
            return string.Format("{0}\\{1}", localDir, localFile);
        }

        public static string FullyQualifiedLocalCacheFileName(string cachedFile, bool asXmlFile)
        {
            string fname = asXmlFile ? cachedFile.Replace(".zip", ".xml") : cachedFile;

            return string.Format("{0}\\{1}", localCacheDir, fname);
        }

        public static string LocalCacheDir()
        {
            return localCacheDir;
        }

        public static FtpSignalGateway Factory()
        {
            return new FtpSignalGateway();

        }

        public static FtpSignalGateway Factory(bool startCron)
        {
            return new FtpSignalGateway(startCron);
        }

        public static FtpWebRequest FtpRequestFactory(string fileName)
        {
            FtpWebRequest request = null;

            try
            {
                string uri = "ftp://" + serverip + "/" + remoteDir + "/" + fileName;
                Uri serverUri = new Uri(uri);
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return request;
                }

                request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + serverip + "/" + remoteDir + "/" + fileName));

                request.Credentials = new NetworkCredential(userid, userpwd);
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.UseBinary = true;
                request.Proxy = null;
                request.UsePassive = false;
            }
            catch (WebException wEx)
            {
                request = null;
            }
            catch (Exception ex)
            {
                request = null;
            }

            return request;
        }

        public static bool GetFtpFromServer(string fileName)
        {
            bool retVal = true;

            string streamDir = string.Format("{0}\\{1}", localDir, fileName);

            try
            {
                FtpWebRequest reqFtp = FtpRequestFactory(fileName);

                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    using (FileStream writeStream = new FileStream(streamDir, FileMode.Create))
                    {

                        int Length = 2048;
                        Byte[] buffer = new Byte[Length];
                        int bytesRead = responseStream.Read(buffer, 0, Length);

                        while (bytesRead > 0)
                        {
                            writeStream.Write(buffer, 0, bytesRead);
                            bytesRead = responseStream.Read(buffer, 0, Length);
                        }

                        writeStream.Close();
                    }

                    response.Close();
                }
            }
            catch (WebException wEx)
            {
                retVal = false;
            }
            catch (Exception ex)
            {
                retVal = false;
            }

            return retVal;
        }

        private FtpSignalGateway(bool startCron = true)
        {
            this.idempotent = IdempotentFtp.Factory();

            fileList = new List<string>();

            this.cronFtp = new System.Timers.Timer(SYSTEM_CHECK_INTERVAL);

            this.cronFtp.Elapsed += new System.Timers.ElapsedEventHandler(OnCheckFtpServer);

            this.cronFtp.Enabled = startCron;
        }

        public bool GatewayActive
        {
            get
            {
                if (object.ReferenceEquals(this.cronFtp, null))
                    return false;

                return this.cronFtp.Enabled;
            }

            set
            {
                if (object.ReferenceEquals(this.cronFtp, null))
                    return;

                this.cronFtp.Enabled = value;
            }
        }

        void OnCheckFtpServer(object sender, System.Timers.ElapsedEventArgs e)
        {
            string andy = string.Empty;

            List<string> newFiles = new List<string>();

            newFiles = this.GetLatestFiles().Where(p => p.EndsWith(".lock") == false).ToList();

            foreach (string file in newFiles)
            {
                if (this.idempotent.AlreadyAdded(file))
                    continue;

                idempotent.Add(file);

                this.OnFtpFileReceived(file);
            }

            string filedone = string.Empty;
        }

        protected virtual void OnFtpFileReceived(string fileName)
        {
            try
            {
                if (GetFtpFromServer(fileName) == false)
                    return;

                EventHandler<FtpFileReceivedEventArgs> handler = FtpFileReceived;

                if (handler != null)
                {
                    handler(this, FtpFileReceivedEventArgs.Factory(fileName));
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private string[] GetLatestFiles()
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + serverip + "/DATA/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(userid, userpwd);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                // to remove the trailing '\n'
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                downloadFiles = null;
                return downloadFiles;
            }
        }
    }
}
