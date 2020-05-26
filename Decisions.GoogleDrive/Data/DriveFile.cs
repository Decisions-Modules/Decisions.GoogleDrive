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
    public class DriveFile
    {
       /* [IgnoreDataMember]
        private readonly Connection _connection;

        public bool Exists()
        {
            return Drive.DoesFileExist(_connection, this);
        }
        public void Delete()
        {
            Drive.DeleteFile(_connection, this);
        }
        public DrivePermission[] GetPermissions()
        {
            return Drive.GetFilePermissions(_connection, this);
        }
        public DrivePermission SetPermissions(DrivePermType type, DriveRole role, string email = null)
        {
            return Drive.SetFilePermissions(_connection, this, type, role, email);
        }
        public bool Download(Stream output, Action<IDownloadProgress> progressTracker = null)
        {
            return Drive.DownloadFile(_connection, this, output, progressTracker);
        }*/
        
        
        internal DriveFile(string id, string name, string desc, string link)
        {
            //_connection = cnct;
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
