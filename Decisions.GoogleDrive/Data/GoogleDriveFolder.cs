using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Download;

namespace Decisions.GoogleDrive
{
    [DataContract]
    public class GoogleDriveFolder
    {
        internal GoogleDriveFolder(string id, string name, string desc, string link)
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
