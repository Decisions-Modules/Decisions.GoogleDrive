using DecisionsFramework.Design.Flow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    [AutoRegisterMethodsOnClass(true, "Integration/Google Drive/Files")]
    public static class FileSteps
    {
        public static bool DoesFileExist(GoogleDriveCredential credential, string googleDriveFileId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.DoesFileExist(connection, googleDriveFileId);
        }

        public static GoogleDriveFile[] GetFileList(GoogleDriveCredential credential, string googleDriveFolderId)
        {
            var connection = Connection.Create(credential);
            var files = GoogleDrive.GetFiles(connection, googleDriveFolderId);
            return files;
        }

        public static void DeleteFile(GoogleDriveCredential credential, string googleDriveFileId)
        {
            var connection = Connection.Create(credential);
            GoogleDrive.DeleteFile(connection, googleDriveFileId);
        }

        public static GoogleDrivePermission[] GetFilePermissions(GoogleDriveCredential credential, string googleDriveFileId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.GetFilePermissions(connection, googleDriveFileId);
        }

        public static GoogleDrivePermission SetFilePermissions(GoogleDriveCredential credential, string googleDriveFileId, GoogleDrivePermission permission)
        {
            var connection = Connection.Create(credential);
            var res = GoogleDrive.SetFilePermissions(connection, googleDriveFileId, permission);
            return res;
        }

        public static bool DownloadFile(GoogleDriveCredential credential, string googleDriveFileId, string localFilePath)
        {
            var connection = Connection.Create(credential);

            using (FileStream fs = File.OpenWrite(localFilePath))
            {
                var status = GoogleDrive.DownloadFile(connection, googleDriveFileId, fs);
                fs.Close();
                return status;
            }
        }

        public static GoogleDriveFile UploadFile(GoogleDriveCredential credential, string googleDriveFolderId, string localFilePath)
        {
            var connection = Connection.Create(credential);
            var fileName = Path.GetFileName(localFilePath);
            using (FileStream fs = File.OpenRead(localFilePath))
            {
                var file = GoogleDrive.UploadFile(connection, fs, fileName, googleDriveFolderId);
                fs.Close();
                return file;
            }
        }



    }
}
