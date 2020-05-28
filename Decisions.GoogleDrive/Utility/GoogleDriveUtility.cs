using DecisionsFramework.ServiceLayer.Services.DBQuery;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if(processAnswer!=null)
                    processAnswer(resp, result);
                result.IsSucceed = true;

            }
            catch (Google.GoogleApiException ex)
            {
                result.ErrorMessage = ex.Error.Message;
                result.HttpErrorCode = ex.HttpStatusCode;
                result.IsSucceed = false;
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
                if (status.Exception is Google.GoogleApiException)
                {
                    Google.GoogleApiException ex = (Google.GoogleApiException)status.Exception;
                    return new GoogleDriveBaseResult()
                    {
                        ErrorMessage = ex.Error.Message,
                        HttpErrorCode = ex.HttpStatusCode,
                        IsSucceed = false
                    };
                }
                throw status.Exception;
            }
        }

        private static GoogleDriveResultWithData<GoogleDriveFile> UploadRequest(Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request)
        {
            var result = request.Upload();
            if (result.Status == UploadStatus.Completed)
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
                    return new GoogleDriveResultWithData<GoogleDriveFile>(){ IsSucceed = false, ErrorMessage = "Unknown error" };
            }

            if (result.Exception is Google.GoogleApiException)
            {
                Google.GoogleApiException ex = (Google.GoogleApiException)result.Exception;
                return new GoogleDriveResultWithData<GoogleDriveFile>()
                {
                    ErrorMessage = ex.Error.Message,
                    HttpErrorCode = ex.HttpStatusCode,
                    IsSucceed = false
                };
            }
            throw result.Exception;
        }

        private static GoogleDriveResultWithData<List<Google.Apis.Drive.v3.Data.File>> GetResources(Connection connection, bool wantsFiles, string folderId = null)
        {
            CorrectFolderId(ref folderId);
            CheckConnectionOrException(connection);

            List<Google.Apis.Drive.v3.Data.File> files = new List<Google.Apis.Drive.v3.Data.File>();

            FilesResource.ListRequest listRequest = connection.Service.Files.List();
            if(wantsFiles)
                listRequest.Q = $"mimeType != 'application/vnd.google-apps.folder' and '{folderId}' in parents and trashed = false";
            else
                listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and '{folderId}' in parents and trashed = false";

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
            //result.Data = files.Select(t => new GoogleDriveFile(t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
            return result;
        }
    };

    
}
