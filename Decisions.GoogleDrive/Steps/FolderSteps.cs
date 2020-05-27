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
        public static GoogleDriveFolder[] GetFolderList(GoogleDriveCredential credential, GoogleDriveFolder folder)
        {
            var connection = Connection.Create(credential);
            var folders = GoogleDrive.GetFolders(connection, folder);
            return folders;
        }

        public static GoogleDriveFolder CreateFolder(GoogleDriveCredential credential, GoogleDriveFolder parentFolder, string newFolderName)
        {
            var connection = Connection.Create(credential);
            GoogleDriveFolder newFolder = GoogleDrive.CreateFolder(connection, newFolderName, parentFolder);
            return newFolder;
        }

        public static void DeleteFolder(GoogleDriveCredential credential, GoogleDriveFolder folder)
        {
            var connection = Connection.Create(credential);
            var res = GoogleDrive.DeleteFolder(connection, folder);
        }

        public static GoogleDrivePermission[] GetFolderPermissions(GoogleDriveCredential credential, GoogleDriveFolder folder)
        {
            var connection = Connection.Create(credential);
            return GoogleDrive.GetFolderPermissions(connection, folder);
        }

        public static GoogleDrivePermission SetFolderPermissions(GoogleDriveCredential credential, GoogleDriveFolder folder, GoogleDrivePermission permission)
        {
            var connection = Connection.Create(credential);
            var res = GoogleDrive.SetFolderPermissions(connection, folder, permission);
            return res;
        }

    }
}
