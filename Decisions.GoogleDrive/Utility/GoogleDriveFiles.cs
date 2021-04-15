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
using Microsoft.AspNetCore.StaticFiles;

namespace Decisions.GoogleDrive
{
    public static partial class GoogleDriveUtility
    {
        public static GoogleDriveResultWithData<GoogleDriveFile[]> GetFiles(Connection connection, string folderId=null)
        {
            var rawResult = GetResources(connection, true, folderId);

            var result = new GoogleDriveResultWithData<GoogleDriveFile[]>(rawResult);
            if(rawResult.Data!=null)
                result.Data = rawResult.Data.Select(t => new GoogleDriveFile(t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
            return result;
        }

        public static GoogleDriveBaseResult DownloadFile(Connection connection, string fileId, string localFilePath, Action<IDownloadProgress> progressTracker = null)
        {
            CheckConnectionOrException(connection);

            try
            {
                using (System.IO.FileStream stream = System.IO.File.Create(localFilePath))
                {
                    var request = connection.Service.Files.Get(fileId);
                    if (progressTracker != null)
                        request.MediaDownloader.ProgressChanged += progressTracker;
                    var res = DownloadRequest(request, stream);

                    stream.Close();
                    if (!res.IsSucceed)
                        System.IO.File.Delete(localFilePath);
                    return res;
                }
            }
            catch
            {
                try { System.IO.File.Delete(localFilePath); } catch { };
                throw;
            }
        }

        public static GoogleDriveResultWithData<GoogleDriveFile> UploadFile(Connection connection, string localFilePath, string fileName=null, string parentFolderId = null, Action<IUploadProgress> progessUpdate = null)
        {
            CorrectFolderId(ref parentFolderId);
            CheckConnectionOrException(connection);
            if(fileName==null)
                fileName = System.IO.Path.GetFileName(localFilePath);

            if (string.IsNullOrEmpty(localFilePath))
                throw new ArgumentNullException("localFilePath", "localFilePath cannot be null or empty.");

            //MimeMapping is not part of .Net Core so we have to do it a different way
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            string mimeType;

            if (!provider.TryGetContentType(fileName, out mimeType))
            {
                mimeType = "application/octet-stream";
            }

            using (System.IO.FileStream stream = System.IO.File.OpenRead(localFilePath))
                try
                {
                    var fileMetadata = new File()
                    {
                        Name = fileName,
                        MimeType = mimeType,
                        Parents = new List<string> { parentFolderId }
                    };

                    var request = connection.Service.Files.Create(fileMetadata, stream, mimeType);
                    request.Fields = "id, name, mimeType, description, webViewLink";

                    if (progessUpdate != null)
                        request.ProgressChanged += progessUpdate;

                    GoogleDriveResultWithData<GoogleDriveFile> result = UploadRequest(request);

                    return result;
                }
                finally {
                    stream.Close();
                }
            
        }
    }
}
