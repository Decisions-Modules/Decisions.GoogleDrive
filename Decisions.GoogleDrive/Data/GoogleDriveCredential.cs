using DecisionsFramework.Design.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    [DataContract]
    public class GoogleDriveCredential
    {
        [DataMember]
        public string ClientId = "";
        [DataMember]
        public string ClientSecret = "";
        [DataMember]
        public string DataStore = "";
    }

    [DataContract]
    public class GoogleDriveServiceAccountCredential
    {
        [DataMember]
        public string Email = "";
        [DataMember]
        [LongTextPropertyEditorAttribute]
        public string PrivateKey = "";
    }
}
