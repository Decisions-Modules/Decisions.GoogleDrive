    using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Decisions.GoogleDrive;
using DecisionsFramework.ServiceLayer.Utilities;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using File = Google.Apis.Drive.v3.Data.File;

namespace Decisions.GoogleDrive
{
    public static partial class GoogleDrive
    {

        private static void CheckConnectionOrException(Connection connection)
        {
            if (!connection.IsConnected())
                throw new ArgumentException("Invalid connection object. It's not connected");
        }

        private static void CheckPermissionOrException(GoogleDrivePermission permission)
        {
            if (permission.Type != GoogleDrivePermType.anyone && permission.Email == null)
                throw new ArgumentException("This permission type requires an email.");
            if (permission.Type == GoogleDrivePermType.unknown || permission.Role == GoogleDriveRole.unknown)
                throw new ArgumentException("Invalid arguments passed.");
        }

        private static void CorrectFolderId(ref string folderId)
        {
            folderId = String.IsNullOrEmpty(folderId) ? "root" : folderId;
        }

        public static bool DoesFileExist(Connection connection, string fileId)
        {
            CheckConnectionOrException(connection);

            var request = connection.Service.Files.Get(fileId);

            try
            {
                _ = request.Execute();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static GoogleDriveFile[] GetFiles(Connection connection, string folderId=null)
        {
            CorrectFolderId(ref folderId);
            CheckConnectionOrException(connection); 

            FilesResource.ListRequest listRequest = connection.Service.Files.List();
            listRequest.Q = $"mimeType != 'application/vnd.google-apps.folder' and '{folderId}' in parents and trashed = false";
            listRequest.Fields = "files(id, name, mimeType, description, webViewLink)";
            var fileList = listRequest.Execute();
            List<File> files = fileList.Files.ToList();
            while (fileList.NextPageToken != null)
            {
                listRequest.PageToken = fileList.NextPageToken;
                fileList = listRequest.Execute();
                files.AddRange(fileList.Files);
            }
            GoogleDriveFile[] tmp = files.Select(t => new GoogleDriveFile(t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
            return tmp;
        }

        public static void DeleteFile(Connection connection, string fileId)
        {
            CheckConnectionOrException(connection);

            var request = connection.Service.Files.Delete(fileId);
            var response = request.Execute();
            var _ = response.Length;//FIXME

        }

        public static GoogleDrivePermission[] GetFilePermissions(Connection connection, string fileId)
        {
            CheckConnectionOrException(connection); 
            
            var req = connection.Service.Files.Get(fileId);
            req.Fields = "permissions";
            var response = req.Execute();

            if(response.Permissions == null)
                return new GoogleDrivePermission[] {};

            return response.Permissions.Select(x => new GoogleDrivePermission(x.Id, x.EmailAddress, TranslatePermType(x.Type), TranslateRole(x.Role))).ToArray();
        }

        public static GoogleDrivePermission SetFilePermissions(Connection connection, string fileId, GoogleDrivePermission permission)
        {
            CheckConnectionOrException(connection);
            CheckPermissionOrException(permission);

            if (!DoesFileExist(connection, fileId))
                throw new ArgumentException("File wasn't found.");

            var request = connection.Service.Permissions.Create(new Permission()
            {
                EmailAddress = permission.Email,
                Type = permission.Type.ToString(),
                Role = permission.Role.ToString()
            }, fileId);

            var resp = request.Execute();
            return new GoogleDrivePermission(resp.Id, resp.EmailAddress, permission.Type, permission.Role);
        }

        public static bool DownloadFile(Connection connection, string fileId, Stream output, Action<IDownloadProgress> progressTracker = null)
        {
            CheckConnectionOrException(connection);

            if (!DoesFileExist(connection, fileId))
                throw new ArgumentException("File does not exist.");

            var request = connection.Service.Files.Get(fileId);
            if(progressTracker != null)
                request.MediaDownloader.ProgressChanged += progressTracker;
            var resp = request.DownloadWithStatus(output);
            if (resp.Status == DownloadStatus.Completed)
                return true;
            return false;
        }

        public static GoogleDriveFile UploadFile(Connection connection, Stream stream, string name, string parentFolderId = null, Action<IUploadProgress> progessUpdate = null)
        {
            CorrectFolderId(ref parentFolderId);
            CheckConnectionOrException(connection);

            if (string.IsNullOrEmpty(name))
                throw new Exception("Name cannot be null or empty.");
            
            var fileMetadata = new File()
            {
                Name = name,
                MimeType = MimeMapping.GetMimeMapping(name),
                Parents = new List<string> { parentFolderId }
            };
            var request = connection.Service.Files.Create(fileMetadata, stream, MimeMapping.GetMimeMapping(name));
            request.Fields = "id, name, mimeType, description, webViewLink";
            if (progessUpdate != null)
                request.ProgressChanged += progessUpdate;
            var result = request.Upload();
            if (result.Status == UploadStatus.Completed)
            {
                var file = request.ResponseBody;
                if (file != null)
                {
                    return new GoogleDriveFile(file.Id, file.Name, file.Description, file.WebViewLink);
                }
            }

            return null;
        }
    }
}
