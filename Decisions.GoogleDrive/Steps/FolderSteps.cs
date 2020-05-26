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
        public static DriveFolder[] GetFolderList(DriveCredential credential, DriveFolder folder)
        {
            var connection = Connection.Create(credential);
            var folders = Drive.GetFolders(connection, folder);
            return folders;
        }

        public static DriveFolder CreateFolder(DriveCredential credential, DriveFolder parentFolder, string newFolderName)
        {
            var connection = Connection.Create(credential);
            DriveFolder newFolder = Drive.CreateFolder(connection, newFolderName, parentFolder);
            return newFolder;
        }

        public static void DeleteFolder(DriveCredential credential, DriveFolder folder)
        {
            var connection = Connection.Create(credential);
            var res = Drive.DeleteFolder(connection, folder);
        }

        public static DrivePermission[] GetFolderPermissions(DriveCredential credential, DriveFolder folder)
        {
            var connection = Connection.Create(credential);
            return Drive.GetFolderPermissions(connection, folder);
        }

        public static DrivePermission SetFolderPermissions(DriveCredential credential, DriveFolder folder, DrivePermission permission)
        {
            var connection = Connection.Create(credential);
            var res = Drive.SetFolderPermissions(connection, folder, permission);
            return res;
        }

    }
}
