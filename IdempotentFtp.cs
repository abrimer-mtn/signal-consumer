using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FtpClient
{
    public class IdempotentFtp
    {
        public class FtpEntry
        {
            public static FtpEntry Factory(string filename)
            {
                return new FtpEntry() { FileName = filename, FSTimeStamp = DateTime.Now };
            }

            public string FileName { get; set; }
            public DateTime FSTimeStamp { get; set; }
        }

        private List<FtpEntry> entries;

        public static IdempotentFtp Factory()
        {
            return new IdempotentFtp();
        }

        public IdempotentFtp()
        {
            this.entries = new List<FtpEntry>();
        }

        public void Add(string fileName)
        {
            this.entries.Add(FtpEntry.Factory(fileName));
        }

        public bool AlreadyAdded(string fileName)
        {
            return this.entries.Count(p => p.FileName == fileName) > 0;
        }

        public void Remove(string filename)
        {
            FtpEntry entry = this.entries.Find(p => p.FileName == filename);

            if (object.ReferenceEquals(entry, null))
                return;

            this.Remove(entry);
        }

        public void Remove(FtpEntry entry)
        {
            this.entries.Remove(entry);
        }

        public void Clear()
        {
            this.entries.Clear();
        }

        public void RemoveOld(DateTime fromTime)
        {
            foreach (FtpEntry item in this.entries.Where(p => p.FSTimeStamp < fromTime))
            {
                this.Remove(item);
            }
        }

    }
}
