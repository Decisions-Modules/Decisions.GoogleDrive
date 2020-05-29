    using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Decisions.GoogleDrive;
using DecisionsFramework.Design.Flow.Service.Debugging;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Utilities;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Upload;

namespace Decisions.GoogleDrive
{
    public static partial class GoogleDrive
    {
        public static GoogleDriveResultWithData<GoogleDriveFile[]> GetFiles(Connection connection, string folderId=null)
        {
            var rawResult = GetResources(connection, true, folderId);

            var result = new GoogleDriveResultWithData<GoogleDriveFile[]>(rawResult);
            if(rawResult.Data!=null)
                result.Data = rawResult.Data.Select(t => new GoogleDriveFile(t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
            return result;
        }

        public static GoogleDriveBaseResult DownloadFile(Connection connection, string fileId, System.IO.Stream output, Action<IDownloadProgress> progressTracker = null)
        {
            CheckConnectionOrException(connection);

            var request = connection.Service.Files.Get(fileId);
            if(progressTracker != null)
                request.MediaDownloader.ProgressChanged += progressTracker;
            return DownloadRequest(request, output);
        }

        public static GoogleDriveResultWithData<GoogleDriveFile> UploadFile(Connection connection, System.IO.Stream stream, string fileName, string parentFolderId = null, Action<IUploadProgress> progessUpdate = null)
        {
            CorrectFolderId(ref parentFolderId);
            CheckConnectionOrException(connection);

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName", "fileName cannot be null or empty.");
            
            var fileMetadata = new File()
            {
                Name = fileName,
                MimeType = MimeMapping.GetMimeMapping(fileName),
                Parents = new List<string> { parentFolderId }
            };
            var request = connection.Service.Files.Create(fileMetadata, stream, MimeMapping.GetMimeMapping(fileName));
            request.Fields = "id, name, mimeType, description, webViewLink";
            if (progessUpdate != null)
                request.ProgressChanged += progessUpdate;

            GoogleDriveResultWithData<GoogleDriveFile> result =  UploadRequest(request);

            return result;
        }
    }
}
