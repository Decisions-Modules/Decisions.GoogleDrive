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
    public static partial class GoogleDriveUtility
    {
        public static GoogleDriveResultWithData<GoogleDriveFolder[]> GetFolders(Connection connection, string folderId=null)
        {
            var rawResult = GetResources(connection, false, folderId);

            var result = new GoogleDriveResultWithData<GoogleDriveFolder[]>(rawResult);
            if (rawResult.Data != null)
                result.Data = rawResult.Data.Select(t => new GoogleDriveFolder(t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
            return result;
        }

        public static GoogleDriveResultWithData<GoogleDriveFolder> CreateFolder(Connection connection, string folderName, string parentFolderId=null)
        {
            CorrectFolderId(ref parentFolderId);
            CheckConnectionOrException(connection);

            File fileMetaData = new File {Name = folderName, MimeType = "application/vnd.google-apps.folder", Parents = new [] { parentFolderId } };
            FilesResource.CreateRequest request = connection.Service.Files.Create(fileMetaData);
            request.Fields = "id, name, mimeType, description, webViewLink";

            var result = ExecuteRequest<FilesResource.CreateRequest, File, GoogleDriveResultWithData<GoogleDriveFolder> >(request, (file, res) => 
            {
                res.Data = new GoogleDriveFolder(file.Id, file.Name, file.Description, file.WebViewLink);
            });
            return result;
        }

    }
}
