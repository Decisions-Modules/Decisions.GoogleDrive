using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Download;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Decisions.GoogleDrive
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GoogleDriveResourceType { Unavailable = 0, File = 1, Folder=2 }

    [DataContract]
    public class GoogleDriveFile
    {

        internal GoogleDriveFile(string id, string name, string desc, string link)
        {
            Id = id;
            Name = name;
            Description = desc;
            SharingLink = link;
        }

        [DataMember]
        public readonly string Id;
        [DataMember]
        public readonly string Name;
        [DataMember]
        public readonly string Description;
        [DataMember]
        public readonly string SharingLink;

    }
}
