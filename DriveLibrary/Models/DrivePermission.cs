using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DriveLibrary.Models
{
    public enum DriveRole
    {
        owner,
        organizer,
        fileOrganizer,
        writer,
        commenter,
        reader,
        unknown
    }

    public enum DrivePermType
    {
        user,
        group,
        domain,
        anyone,
        unknown
    }

    [DataContract]
    public class DrivePermission
    {
        internal DrivePermission(string id, string email, DrivePermType type, DriveRole role)
        {
            Id = id;
            Email = email;
            Type = type;
            Role = role;
        }

        [DataMember]
        public readonly string Id;
        [DataMember]
        public readonly string Email;
        [DataMember]
        public readonly DrivePermType Type;
        [DataMember]
        public readonly DriveRole Role;
    }
}
