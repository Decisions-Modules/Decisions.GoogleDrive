using DecisionsFramework.ServiceLayer.Services.DBQuery;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    public static partial class GoogleDriveUtility
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

        private static GoogleDriveRole TranslateRole(string role)
        {
            switch (role)
            {
                case "owner":
                    return GoogleDriveRole.owner;
                case "organizer":
                    return GoogleDriveRole.organizer;
                case "fileOrganizer":
                    return GoogleDriveRole.fileOrganizer;
                case "writer":
                    return GoogleDriveRole.writer;
                case "commenter":
                    return GoogleDriveRole.commenter;
                case "reader":
                    return GoogleDriveRole.reader;
                default:
                    return GoogleDriveRole.unknown;
            }
        }

        private static GoogleDrivePermType TranslatePermType(string role)
        {
            switch (role)
            {
                case "user":
                    return GoogleDrivePermType.user;
                case "group":
                    return GoogleDrivePermType.group;
                case "domain":
                    return GoogleDrivePermType.domain;
                case "anyone":
                    return GoogleDrivePermType.anyone;
                default:
                    return GoogleDrivePermType.unknown;
            }
        }

       

        private static TResult ExecuteRequest<TRequest, TResponse, TResult>(TRequest request, Action<TResponse,TResult> processAnswer)
                            where TResult : GoogleDriveBaseResult, new()
                            where TRequest : ClientServiceRequest<TResponse>
        {
            TResult result = new TResult();
            try
            {
                var resp = request.Execute();
                if (processAnswer != null)
                    processAnswer(resp, result);
                result.IsSucceed = true;

            }
            catch (Exception exception)
            {
                if(!result.FillFromException(exception))
                    throw;
            }

            return result;
        }

        private static GoogleDriveBaseResult DownloadRequest(Google.Apis.Drive.v3.FilesResource.GetRequest request, System.IO.Stream output)
        {

            var status = request.DownloadWithStatus(output);
            if (status.Status == DownloadStatus.Completed)
            {
                return new GoogleDriveBaseResult() { IsSucceed = true };
            }
            else
            {
                var result = new GoogleDriveBaseResult();
                if (result.FillFromException(status.Exception))
                    return result;
                throw status.Exception;              
            }
        }

        private static GoogleDriveResultWithData<GoogleDriveFile> UploadRequest(Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request)
        {
            var status = request.Upload();
            if (status.Status == UploadStatus.Completed)
            {
                var file = request.ResponseBody;
                if (file != null)
                {
                    return new GoogleDriveResultWithData<GoogleDriveFile>()
                    {
                        Data = new GoogleDriveFile(file.Id, file.Name, file.Description, file.WebViewLink),
                        IsSucceed = true
                    };
                }
                else
                    return new GoogleDriveResultWithData<GoogleDriveFile>(){ IsSucceed = false, ErrorInfo = new GoogleDriveErrorInfo { ErrorMessage="Unknown error" } };
            }

            var result = new GoogleDriveResultWithData<GoogleDriveFile>();
            if (result.FillFromException(status.Exception))
                return result;
            throw status.Exception;
          
        }

        const string FolderMimeType = "application/vnd.google-apps.folder";

        private static GoogleDriveResultWithData<List<Google.Apis.Drive.v3.Data.File>> GetResources(Connection connection, bool wantsFiles, string folderId = null)
        {
            CorrectFolderId(ref folderId);
            CheckConnectionOrException(connection);

            List<Google.Apis.Drive.v3.Data.File> files = new List<Google.Apis.Drive.v3.Data.File>();

            FilesResource.ListRequest listRequest = connection.Service.Files.List();
            if(wantsFiles)
                listRequest.Q = $"mimeType != '{FolderMimeType}' and '{folderId}' in parents and trashed = false";
            else
                listRequest.Q = $"mimeType = '{FolderMimeType}' and '{folderId}' in parents and trashed = false";

            listRequest.Fields = "nextPageToken, files(id, name, mimeType, description, webViewLink)";

            FileList googleDriveFileList;
            GoogleDriveBaseResult partialResult;
            do
            {
                googleDriveFileList = null;
                partialResult = ExecuteRequest<FilesResource.ListRequest, FileList, GoogleDriveBaseResult>(listRequest, (aFileList, _) =>
                {
                    googleDriveFileList = aFileList;
                    files.AddRange(googleDriveFileList.Files);
                    listRequest.PageToken = googleDriveFileList.NextPageToken;
                });

                if (!partialResult.IsSucceed)
                    return new GoogleDriveResultWithData<List<Google.Apis.Drive.v3.Data.File>>(partialResult);
            } while (googleDriveFileList != null && googleDriveFileList.NextPageToken != null);

            var result = new GoogleDriveResultWithData<List<Google.Apis.Drive.v3.Data.File>> (partialResult) { Data = files };

            return result;
        }

        public static GoogleDriveResultWithData<GoogleDriveResourceType> DoesResourceExist(Connection connection, string fileOrFolderId)
        {
            CheckConnectionOrException(connection);

            var request = connection.Service.Files.Get(fileOrFolderId);

            GoogleDriveResultWithData<GoogleDriveResourceType> result = ExecuteRequest<FilesResource.GetRequest, Google.Apis.Drive.v3.Data.File, GoogleDriveResultWithData<GoogleDriveResourceType>>(request, (resp, res) =>
            {
                if(resp.MimeType == FolderMimeType)
                    res.Data = GoogleDriveResourceType.Folder;
                else
                    res.Data = GoogleDriveResourceType.File;
            });

            if (result.ErrorInfo.HttpErrorCode == (int)HttpStatusCode.NotFound)
            {
                result.IsSucceed = true;
                result.Data = GoogleDriveResourceType.Unavailable;
            }

            return result;
        }

        public static GoogleDriveBaseResult DeleteResource(Connection connection, string fileOrFolderId)
        {
            CheckConnectionOrException(connection);

            FilesResource.DeleteRequest request = connection.Service.Files.Delete(fileOrFolderId);
            GoogleDriveBaseResult result = ExecuteRequest<FilesResource.DeleteRequest, string, GoogleDriveBaseResult>(request, null);
            return result;
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission[]> GetResourcePermissions(Connection connection, string fileOrFolderId)
        {
            CheckConnectionOrException(connection);

            FilesResource.GetRequest request = connection.Service.Files.Get(fileOrFolderId);
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

        public static GoogleDriveResultWithData<GoogleDrivePermission> SetResourcePermissions(Connection connection, string fileOrFolderId, GoogleDrivePermission permission)
        {
            CheckConnectionOrException(connection);
            CheckPermissionOrException(permission);

            PermissionsResource.CreateRequest request = connection.Service.Permissions.Create(new Permission()
            {
                EmailAddress = permission.Email,
                Type = permission.Type.ToString(),
                Role = permission.Role.ToString()
            }, fileOrFolderId);

            GoogleDriveResultWithData<GoogleDrivePermission> result = ExecuteRequest<PermissionsResource.CreateRequest, Permission, GoogleDriveResultWithData<GoogleDrivePermission>>(request, (resp, res) =>
            {
                res.Data = new GoogleDrivePermission(resp.Id, resp.EmailAddress, permission.Type, permission.Role);
            });

            return result;
        }
    };

    
}
