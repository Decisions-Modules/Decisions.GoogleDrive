using Decisions.GoogleDrive;
using DecisionsFramework.Design.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveLibrary.Steps
{
    [AutoRegisterMethodsOnClass(true, "Integration/Google Drive/Folders")]
    public static class FolderSteps
    {
        public static GoogleDriveResultWithData<bool> DoesFolderExist(GoogleDriveCredential credential, string googleDriveFolderId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.DoesFolderExist(connection, googleDriveFolderId);
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
            var folderResult= GoogleDrive.CreateFolder(connection, newFolderName, parentGoogleDriveFolderId);
            return folderResult;
        }

        public static void DeleteFolder(GoogleDriveCredential credential, string parentGoogleDriveFolderId)
        {
            var connection = Connection.Create(credential);
            var res = GoogleDrive.DeleteFolder(connection, parentGoogleDriveFolderId);
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission[]> GetFolderPermissions(GoogleDriveCredential credential, string parentGoogleDriveFolderId)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.GetFolderPermissions(connection, parentGoogleDriveFolderId);
        }

        public static GoogleDriveResultWithData<GoogleDrivePermission> SetFolderPermissions(GoogleDriveCredential credential, string parentGoogleDriveFolderId, GoogleDrivePermission permission)
        {
            var connection = Connection.Create(credential);
            var res = GoogleDrive.SetFolderPermissions(connection, parentGoogleDriveFolderId, permission);
            return res;
        }

    }
}
