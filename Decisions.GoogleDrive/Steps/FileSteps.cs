using DecisionsFramework.Design.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    [AutoRegisterMethodsOnClass(true, "Integration/Google Drive/Files")]
    public static class FileSteps
    {
        public static GoogleDriveResultWithData<bool> DoesFileExist(GoogleDriveCredential credential, string googleDriveFileId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.DoesFileExist(connection, googleDriveFileId);
        }

        public static GoogleDriveResultWithData<GoogleDriveFile[]> GetFileList(GoogleDriveCredential credential, string googleDriveFolderId)
        {
            var connection = Connection.Create(credential);
            var files = GoogleDrive.GetFiles(connection, googleDriveFolderId);
            return files;
        }

        public static GoogleDriveBaseResult DeleteFile(GoogleDriveCredential credential, string googleDriveFileId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.DeleteFile(connection, googleDriveFileId);
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission[]> GetFilePermissions(GoogleDriveCredential credential, string googleDriveFileId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.GetFilePermissions(connection, googleDriveFileId);
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission> SetFilePermissions(GoogleDriveCredential credential, string googleDriveFileId, GoogleDrivePermission permission)
        {
            var connection = Connection.Create(credential);
            var res = GoogleDrive.SetFilePermissions(connection, googleDriveFileId, permission);
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



    }
}
