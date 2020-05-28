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

        public static GoogleDriveResultWithData<bool> DoesFileExist(Connection connection, string fileId)
        {
            CheckConnectionOrException(connection);

            var request = connection.Service.Files.Get(fileId);

            GoogleDriveResultWithData<bool> result = ExecuteRequest<FilesResource.GetRequest, Google.Apis.Drive.v3.Data.File, GoogleDriveResultWithData<bool>>(request, (resp, res) =>
            {
                res.Data = true;
            });
             
            if (result.HttpErrorCode == HttpStatusCode.NotFound)
            {
                result.IsSucceed = true;
                result.Data = false;
            }

            return result;

        }

        public static GoogleDriveResultWithData<GoogleDriveFile[]> GetFiles(Connection connection, string folderId=null)
        {
            /*  CorrectFolderId(ref folderId);
              CheckConnectionOrException(connection);

              List<File> files = new List<File>();

              FilesResource.ListRequest listRequest = connection.Service.Files.List();
              listRequest.Q = $"mimeType != 'application/vnd.google-apps.folder' and '{folderId}' in parents and trashed = false";
              listRequest.Fields = "nextPageToken, files(id, name, mimeType, description, webViewLink)";

              FileList googleDriveFileList;
              GoogleDriveBaseResult partialResult;
              do {
                  googleDriveFileList = null;
                  partialResult = ExecuteRequest<FilesResource.ListRequest, FileList, GoogleDriveBaseResult>(listRequest, (aFileList, _) =>
                  {
                      googleDriveFileList = aFileList;
                      files.AddRange(googleDriveFileList.Files);
                      listRequest.PageToken = googleDriveFileList.NextPageToken;
                  });

                  if (!partialResult.IsSucceed)
                      return new GoogleDriveResultWithData<GoogleDriveFile[]>(partialResult);
              } while (googleDriveFileList != null && googleDriveFileList.NextPageToken != null);*/

            var rawResult = GetResources(connection, true, folderId);

            var result = new GoogleDriveResultWithData<GoogleDriveFile[]>(rawResult);
            if(rawResult.Data!=null)
                result.Data = rawResult.Data.Select(t => new GoogleDriveFile(t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
            return result;
        }

        public static GoogleDriveBaseResult DeleteFile(Connection connection, string fileId)
        {
            CheckConnectionOrException(connection);

            FilesResource.DeleteRequest request = connection.Service.Files.Delete(fileId);
            GoogleDriveBaseResult result = ExecuteRequest<FilesResource.DeleteRequest, string, GoogleDriveBaseResult>(request, null);
            return result;
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission[]> GetFilePermissions(Connection connection, string fileId)
        {
            CheckConnectionOrException(connection);

            FilesResource.GetRequest request = connection.Service.Files.Get(fileId);
            request.Fields = "permissions";

            GoogleDriveResultWithData<GoogleDrivePermission[]> result = ExecuteRequest<FilesResource.GetRequest, File, GoogleDriveResultWithData<GoogleDrivePermission[]>>(request, (resp, res) =>
            {
                if (resp.Permissions == null)
                    res.Data = new GoogleDrivePermission[] { };
                else
                    res.Data = resp.Permissions.Select(x => new GoogleDrivePermission(x.Id, x.EmailAddress, TranslatePermType(x.Type), TranslateRole(x.Role))).ToArray();
            });

            return result;
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission> SetFilePermissions(Connection connection, string fileId, GoogleDrivePermission permission)
        {
            CheckConnectionOrException(connection);
            CheckPermissionOrException(permission);

            PermissionsResource.CreateRequest request = connection.Service.Permissions.Create(new Permission()
            {
                EmailAddress = permission.Email,
                Type = permission.Type.ToString(),
                Role = permission.Role.ToString()
            }, fileId);

            GoogleDriveResultWithData<GoogleDrivePermission> result = ExecuteRequest<PermissionsResource.CreateRequest, Permission, GoogleDriveResultWithData<GoogleDrivePermission>>(request, (resp, res) =>
            {
                res.Data = new GoogleDrivePermission(resp.Id, resp.EmailAddress, permission.Type, permission.Role);
            });

            return result;
        }

        public static GoogleDriveBaseResult DownloadFile(Connection connection, string fileId, System.IO.Stream output, Action<IDownloadProgress> progressTracker = null)
        {
            CheckConnectionOrException(connection);

            /*if (!DoesFileExist(connection, fileId))
                throw new ArgumentException("File does not exist.");*/

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
