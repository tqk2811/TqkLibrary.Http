using System.Collections.Generic;

namespace TqkLibrary.Http.Api.Other.ImagesHostApi.ImagesHackCom.DataClass
{
    public class ImagesHackUploadResult
    {
        public long? max_filesize { get; set; }
        public long? space_limit { get; set; }
        public long? space_used { get; set; }
        public long? space_left { get; set; }
        public int? passed { get; set; }
        public int? failed { get; set; }
        public int? total { get; set; }
        public List<ImagesHackUploadImage> images { get; set; }
    }
}