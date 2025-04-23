using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class FileResponseModel
    {
        public byte[] FileByteArray { get; set; }
        public string FileType { get; set; }

        public string FileName { get; set; }
    }
}
