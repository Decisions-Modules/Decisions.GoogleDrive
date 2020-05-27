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
        public static GoogleDriveFile[] GetFileList(GoogleDriveCredential credential, GoogleDriveFolder folder)
        {
            var connection = Connection.Create(credential);
            var files = GoogleDrive.GetFiles(connection, folder);
            return files;
        }

        public static void DeleteFile(GoogleDriveCredential credential, GoogleDriveFile file)
        {
            var connection = Connection.Create(credential);
            GoogleDrive.DeleteFile(connection, file);
        }

        public static GoogleDrivePermission[] GetFilePermissions(GoogleDriveCredential credential, GoogleDriveFile file)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.GetFilePermissions(connection, file);
        }

        public static GoogleDrivePermission SetFilePermissions(GoogleDriveCredential credential, GoogleDriveFile file, GoogleDrivePermission permission)
        {
            var connection = Connection.Create(credential);
            var res = GoogleDrive.SetFilePermissions(connection, file, permission);
            return res;
        }

        public static bool DownloadFile(GoogleDriveCredential credential, GoogleDriveFile remoteFile, string localFilePath)
        {
            var connection = Connection.Create(credential);

            using (FileStream fs = File.OpenWrite(localFilePath))
            {
                var status = GoogleDrive.DownloadFile(connection, remoteFile, fs);
                fs.Close();
                return status;
            }
        }

        public static GoogleDriveFile UploadFile(GoogleDriveCredential credential, GoogleDriveFolder folder, string localFilePath)
        {
            var connection = Connection.Create(credential);
            var fileName = Path.GetFileName(localFilePath);
            using (FileStream fs = File.OpenRead(localFilePath))
            {
                var file = GoogleDrive.UploadFile(connection, fs, fileName, folder);
                fs.Close();
                return file;
            }
        }

    }
}
