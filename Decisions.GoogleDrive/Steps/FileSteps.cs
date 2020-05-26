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
        public static DriveFile[] GetFileList(DriveCredential credential, DriveFolder folder)
        {
            var connection = Connection.Create(credential);
            var files = Drive.GetFiles(connection, folder);
            return files;
        }

        public static void DeleteFile(DriveCredential credential, DriveFile file)
        {
            var connection = Connection.Create(credential);
            Drive.DeleteFile(connection, file);
        }

        public static DrivePermission[] GetFilePermissions(DriveCredential credential, DriveFile file)
        {
            var connection = Connection.Create(credential);
            return Drive.GetFilePermissions(connection, file);
        }

        public static DrivePermission SetFilePermissions(DriveCredential credential, DriveFile file, DrivePermission permission)
        {
            var connection = Connection.Create(credential);
            var res = Drive.SetFilePermissions(connection, file, permission);
            return res;
        }

        public static bool DownloadFile(DriveCredential credential, DriveFile remoteFile, string localFilePath)
        {
            var connection = Connection.Create(credential);

            using (FileStream fs = File.OpenWrite(localFilePath))
            {
                var status = Drive.DownloadFile(connection, remoteFile, fs);
                fs.Close();
                return status;
            }
        }

        public static DriveFile UploadFile(DriveCredential credential, DriveFolder folder, string localFilePath)
        {
            var connection = Connection.Create(credential);
            var fileName = Path.GetFileName(localFilePath);
            using (FileStream fs = File.OpenRead(localFilePath))
            {
                var file = Drive.UploadFile(connection, fs, fileName, folder);
                fs.Close();
                return file;
            }
        }

    }
}
