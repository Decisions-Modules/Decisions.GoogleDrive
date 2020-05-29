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

    public class GoogleDriveBaseResult
    {
        public string ErrorMessage { get; set; }
        public HttpStatusCode HttpErrorCode { get; set; }
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

}
