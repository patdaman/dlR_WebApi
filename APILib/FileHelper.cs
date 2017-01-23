using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalAPILib
{

    public class FileHelper
    {
        public enum FileHelperStatus
        {
            EmptyDownload,
            OK
        };

        public Dictionary<FileHelperStatus, string> FileHelperStatusItems = new Dictionary<FileHelperStatus, string>()
        {
            {FileHelperStatus.EmptyDownload, "Did Not Download" },
            {FileHelperStatus.OK, "OK" }
        };

        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public FileHelperStatus Status { get; set; } = FileHelper.FileHelperStatus.OK;

    }
}
