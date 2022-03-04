using System;

namespace AppModel
{
    public class UploadFilesResult
    {
        public string Name { get; set; }
        public float Length { get; set; }
        public string Type { get; set; }
        public Boolean Status { get; set; }
        public string StatusMessage { get; set; }

    }
}