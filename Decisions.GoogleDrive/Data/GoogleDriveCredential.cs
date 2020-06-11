using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;
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

    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GoogleDriveCredentialType { Token, ServiceAccount };

    [DataContract]
    public class GoogleDriveServiceAccountCredential
    {
        [DataMember]
        public string Email = "";
        [DataMember]
        [LongTextPropertyEditorAttribute]
        public string PrivateKey = "";
    }

    [DataContract]
    public class GoogleDriveCredential
    {
        [DataMember]
        public GoogleDriveCredentialType CredentinalType { get; set; }

        [DataMember]
        [PropertyHiddenByValue("CredentinalType", GoogleDriveCredentialType.Token, false)]
        [TokenPicker]
        public string Token { get; set; }

        [DataMember]
        [PropertyHiddenByValue("CredentinalType", GoogleDriveCredentialType.ServiceAccount, false)]
        public GoogleDriveServiceAccountCredential ServiceAccount { get; set; }
    }

    public class GoogleDriveUserCredential
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string DataStore { get; set; }
    }
}
