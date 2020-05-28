using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Decisions.GoogleDrive;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace Decisions.GoogleDrive
{
    public static partial class GoogleDrive
    {
        public static GoogleDriveResultWithData<bool> DoesFolderExist(Connection connection, string folderId)
        {
            CheckConnectionOrException(connection);

            var request = connection.Service.Files.Get(folderId);

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

        public static GoogleDriveResultWithData<GoogleDriveFolder[]> GetFolders(Connection connection, string folderId=null)
        {
            var rawResult = GetResources(connection, false, folderId);

            var result = new GoogleDriveResultWithData<GoogleDriveFolder[]>(rawResult);
            if (rawResult.Data != null)
                result.Data = rawResult.Data.Select(t => new GoogleDriveFolder(t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
            return result;
        }

        public static GoogleDriveResultWithData<GoogleDriveFolder> CreateFolder(Connection connection, string name, string folderId=null)
        {
            CorrectFolderId(ref folderId);
            CheckConnectionOrException(connection);

            File fileMetaData = new File {Name = name, MimeType = "application/vnd.google-apps.folder", Parents = new [] { folderId } };
            FilesResource.CreateRequest request = connection.Service.Files.Create(fileMetaData);
            request.Fields = "id, name, mimeType, description, webViewLink";

            var result = ExecuteRequest<FilesResource.CreateRequest, File, GoogleDriveResultWithData<GoogleDriveFolder> >(request, (file, res) => 
            {
                res.Data = new GoogleDriveFolder(file.Id, file.Name, file.Description, file.WebViewLink);
            });
            return result;
        }

        public static GoogleDriveBaseResult DeleteFolder(Connection connection, string folderId)
        {
            CheckConnectionOrException(connection);

            FilesResource.DeleteRequest request = connection.Service.Files.Delete(folderId);
            GoogleDriveBaseResult result = ExecuteRequest<FilesResource.DeleteRequest, string, GoogleDriveBaseResult>(request, null);
            return result;
        }


        public static GoogleDriveResultWithData<GoogleDrivePermission[]> GetFolderPermissions(Connection connection, string folderId)
        {
            return GetFilePermissions(connection, folderId);
            /*CheckConnectionOrException(connection);
            var req = connection.Service.Files.Get(folderId);
            req.Fields = "permissions";
            var response = req.Execute();

            if (response.Permissions == null)
                return new GoogleDrivePermission[] { };

            return response.Permissions.Select(x => new GoogleDrivePermission(x.Id, x.EmailAddress, TranslatePermType(x.Type), TranslateRole(x.Role))).ToArray();*/
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission> SetFolderPermissions(Connection connection, string folderId, GoogleDrivePermission permission)
        {
            return SetFilePermissions(connection, folderId, permission);
           /* CheckConnectionOrException(connection);
            CheckPermissionOrException(permission);

            //if (!DoesFolderExist(connection, folderId))
            //    throw new Exception("Folder wasn't found.");

            var request = connection.Service.Permissions.Create(new Permission()
            {
                EmailAddress = permission.Email,
                Type = permission.Type.ToString(),
                Role = permission.Role.ToString()
            }, folderId);

            var resp = request.Execute();
            return new GoogleDrivePermission(resp.Id, resp.EmailAddress, permission.Type, permission.Role);*/
        }
    }
}
