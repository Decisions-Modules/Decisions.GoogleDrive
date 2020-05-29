using DecisionsFramework.Design.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    public static class StepsCore
    {
        public static GoogleDriveResultWithData<GoogleDriveResourceType> DoesResourceExist(GoogleDriveCredential credential, string googleDriveFileOrFolderId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.DoesResourceExist(connection, googleDriveFileOrFolderId);
        }

        public static GoogleDriveResultWithData<GoogleDriveFile[]> GetFileList(GoogleDriveCredential credential, string googleDriveFolderId)
        {
            var connection = Connection.Create(credential);
            var files = GoogleDrive.GetFiles(connection, googleDriveFolderId);
            return files;
        }

        public static GoogleDriveBaseResult DeleteResource(GoogleDriveCredential credential, string googleDriveFileOrFolderId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.DeleteResource(connection, googleDriveFileOrFolderId);
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission[]> GetResourcePermissions(GoogleDriveCredential credential, string googleDriveFileOrFolderId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.GetResourcePermissions(connection, googleDriveFileOrFolderId);
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission> SetResourcePermissions(GoogleDriveCredential credential, string googleDriveFileOrFolderId, GoogleDrivePermission permission)
        {
            var connection = Connection.Create(credential);
            var res = GoogleDrive.SetResourcePermissions(connection, googleDriveFileOrFolderId, permission);
            return res;
        }

        public static GoogleDriveBaseResult DownloadFile(GoogleDriveCredential credential, string googleDriveFileId, string localFilePath)
        {
            var connection = Connection.Create(credential);

            using (System.IO.FileStream fs = System.IO.File.OpenWrite(localFilePath))
            {
                var res = GoogleDrive.DownloadFile(connection, googleDriveFileId, fs);
                fs.Close();
                if (!res.IsSucceed)
                    System.IO.File.Delete(localFilePath);
                return res;
            }
        }

        public static GoogleDriveResultWithData<GoogleDriveFile> UploadFile(GoogleDriveCredential credential, string googleDriveFolderId, string localFilePath)
        {
            var connection = Connection.Create(credential);
            var fileName = System.IO.Path.GetFileName(localFilePath);
            using (System.IO.FileStream fs = System.IO.File.OpenRead(localFilePath))
            {
                var file = GoogleDrive.UploadFile(connection, fs, fileName, googleDriveFolderId);
                fs.Close();
                return file;
            }
        }

        public static GoogleDriveResultWithData<GoogleDriveFolder[]> GetFolderList(GoogleDriveCredential credential, string googleDriveFolderId)
        {
            var connection = Connection.Create(credential);
            var folders = GoogleDrive.GetFolders(connection, googleDriveFolderId);
            return folders;
        }

        public static GoogleDriveResultWithData<GoogleDriveFolder> CreateFolder(GoogleDriveCredential credential, string parentGoogleDriveFolderId, string newFolderName)
        {
            var connection = Connection.Create(credential);
            var folderResult = GoogleDrive.CreateFolder(connection, newFolderName, parentGoogleDriveFolderId);
            return folderResult;
        }



    }
}
