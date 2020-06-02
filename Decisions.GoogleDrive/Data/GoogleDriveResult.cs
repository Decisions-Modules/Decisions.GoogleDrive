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
    public class GoogleDriveErrorInfo
    {
        [DataMember] public string ErrorMessage { get; set; }
        [DataMember] public HttpStatusCode? HttpErrorCode { get; set; }
    }

    public class GoogleDriveBaseResult: GoogleDriveErrorInfo
    {
        public bool IsSucceed { get; set; }
        public virtual object DataObj { get { return null; } }

        public bool FillFromException(Exception exception)
        {
            if (exception is Google.GoogleApiException)
            {
                var ex = (Google.GoogleApiException)exception;
                ErrorMessage = ex.Error?.Message ?? (ex.Message ?? ex.ToString());
                HttpErrorCode = ex.HttpStatusCode;
                IsSucceed = false;
                return true;
            }
            else if (exception is Google.Apis.Auth.OAuth2.Responses.TokenResponseException)
            {
                var ex = (Google.Apis.Auth.OAuth2.Responses.TokenResponseException)exception;
                ErrorMessage = ex.Error?.ToString() ?? (ex.Message ?? ex.ToString());
                HttpErrorCode = ex.StatusCode;
                IsSucceed = false;
                return true;
            }

            return false;
        }

    }

    public class GoogleDriveResultWithData<T> : GoogleDriveBaseResult
    {
        public T Data { get; set; }
        public override object DataObj { get { return Data; } }

        public GoogleDriveResultWithData() { }

        internal GoogleDriveResultWithData(GoogleDriveBaseResult baseResult)
        {
            ErrorMessage = baseResult.ErrorMessage;
            HttpErrorCode = baseResult.HttpErrorCode;
            IsSucceed = baseResult.IsSucceed;
        }
        
    }

   
}
