using Decisions.Utilities.Data.DataStore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    [DataContract]
    public class GoogleDriveBaseResult
    {
        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public HttpStatusCode HttpErrorCode { get; set; }

        [DataMember]
        public bool IsSucceed { get; set; }
    }

    public class GoogleDriveResultWithData<T> : GoogleDriveBaseResult
    {
        public T Data { get; set; }

        public GoogleDriveResultWithData() { }

        internal GoogleDriveResultWithData(GoogleDriveBaseResult baseResult)
        {
            ErrorMessage = baseResult.ErrorMessage;
            HttpErrorCode = baseResult.HttpErrorCode;
            IsSucceed = baseResult.IsSucceed;
        }
        
    }

    /*[DataContract]
    public class GoogleDriveBoolResult : GoogleDriveBaseResult
    {
        [DataMember]
        public bool Data { get; set; }
    }

    [DataContract]
    public class GoogleDriveFileListResult : GoogleDriveBaseResult
    {
        [DataMember]
        public GoogleDriveFile[] Data { get; set; }

        public GoogleDriveFileListResult() { }
        internal GoogleDriveFileListResult(GoogleDriveBaseResult baseResult)
        {
            ErrorMessage = baseResult.ErrorMessage;
            HttpErrorCode = baseResult.HttpErrorCode;
            IsSucceed = baseResult.IsSucceed;
        }
    }

    [DataContract]
    public class GoogleDriveGetPermissionResult : GoogleDriveBaseResult
    {
        [DataMember]
        public GoogleDrivePermission[] Data { get; set; }
    }

    [DataContract]
    public class GoogleDriveSetPermissionResult : GoogleDriveBaseResult
    {
        [DataMember]
        public GoogleDrivePermission Data { get; set; }
    }

    [DataContract]
    public class GoogleDriveUploadResult : GoogleDriveBaseResult
    {
        [DataMember]
        public GoogleDriveFile Data { get; set; }
    }*/

}
