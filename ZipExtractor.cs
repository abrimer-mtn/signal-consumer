using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Ionic.Zip;

namespace FtpClient
{
    public class ZipExtractor
    {
        public ZipExtractor()
        {
        }

        public static ZipFile Factory(string fileName)
        {
            return ZipFile.Read(FtpSignalGateway.FullyQualifiedLocalFileName(fileName));
        }

        public static IList<XDocument> GetEntries(string fileName)
        {
            return null; //Factory(file
        }

        public static bool Extract(string fileName, bool createFile, ref XDocument result)
        {
            bool retVal = true;

            string zipToUnpack = FtpSignalGateway.FullyQualifiedLocalFileName(fileName);

            MemoryStream s;

            XDocument rootDoc = new XDocument();
            rootDoc.Add(new XElement("root"));

            try
            {
                using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
                {
                    #region Zip Entry Processing

                    // here, we extract every entry, but we could extract conditionally
                    // based on entry name, size, date, checkbox status, etc.  
                    foreach (ZipEntry e in zip1)
                    {
                        try
                        {
                            s = new MemoryStream();

                            e.Extract(s);

                            using (StreamReader reader = new StreamReader(s))
                            {
                                if (object.ReferenceEquals(s, null))
                                    continue;

                                s.Position = 0;

                                using (System.Xml.XmlReader xreader = System.Xml.XmlReader.Create(s))
                                {
                                    xreader.MoveToContent();

                                    XElement element = XElement.Load(xreader, LoadOptions.PreserveWhitespace);

                                    rootDoc.Element(XName.Get("root")).Add(element);
                                }

                                s.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            string problem = ex.Message;
                        }
                        finally
                        {
                            s = null;
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                retVal = false;
            }
            finally
            {
                if (createFile)
                    rootDoc.Save(FtpSignalGateway.FullyQualifiedLocalCacheFileName(fileName, true));
                else
                    result = rootDoc;

                File.Delete(FtpSignalGateway.FullyQualifiedLocalFileName(fileName));
            }

            return retVal;
        }

    }
}
