using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Download;

namespace DriveLibrary.Models
{
    [DataContract]
    public class DriveFolder
    {
        [IgnoreDataMember]
        private readonly Connection _connection;

        public bool Exists()
        {
            return Drive.DoesFolderExist(_connection, this);
        }
        public void Delete()
        {
            Drive.DeleteFolder(_connection, this);
        }
        public DrivePermission[] GetPermissions()
        {
            return Drive.GetFolderPermissions(_connection, this);
        }
        public DrivePermission SetPermissions(DrivePermType type, DriveRole role, string email = null)
        {
            return Drive.SetFolderPermissions(_connection, this, type, role, email);
        }
        public DriveFile[] GetFiles()
        {
            return Drive.GetFiles(_connection, this);
        }
        public DriveFolder[] GetFolders()
        {
            return Drive.GetFolders(_connection, this);
        }
        public DriveFolder CreateSubfolder(string name)
        {
            return Drive.CreateFolder(_connection, name, this);
        }

        internal DriveFolder(Connection cnct, string id, string name, string desc, string link)
        {
            _connection = cnct;
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
