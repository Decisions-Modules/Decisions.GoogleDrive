using DecisionsFramework.Design.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GoogleDriveRole
    {
        owner,
        organizer,
        fileOrganizer,
        writer,
        commenter,
        reader,
        unknown
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GoogleDrivePermType
    {
        user,
        group,
        domain,
        anyone,
        unknown
    }

    [DataContract]
    public class GoogleDrivePermission
    {
        public GoogleDrivePermission(string id, string email, GoogleDrivePermType type, GoogleDriveRole role)
        {
            Id = id;
            Email = email;
            Type = type;
            Role = role;
        }
        public GoogleDrivePermission()
        { }

        [DataMember]
        public string Id { get; }

        [DataMember]
        [PropertyClassificationAttribute("User's Email", 1)]
        public string Email { get; set; }

        [DataMember]
        [PropertyClassificationAttribute("Permission Type", 2)]
        public GoogleDrivePermType Type { get; set; }

        [DataMember]
        [PropertyClassificationAttribute("Role", 3)]
        public GoogleDriveRole Role { get; set; }
    }
}
