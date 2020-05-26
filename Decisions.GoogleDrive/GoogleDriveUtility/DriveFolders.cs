using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decisions.GoogleDrive;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace Decisions.GoogleDrive
{
    public static partial class Drive
    {



        public static bool DoesFolderExist(Connection connection, DriveFolder folder)
        {
            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");
            var request = connection.Service.Files.Get(folder.Id);

            try
            {
                _ = request.Execute();
                return true;
            }
            catch {
                return false;
            }
        }

        public static DriveFolder[] GetFolders(Connection connection, DriveFolder folder = null)
        {
            string folderId = folder == null ? "root" : folder.Id;

            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");

            FilesResource.ListRequest listRequest = connection.Service.Files.List();
            listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and '{folderId}' in parents and trashed = false";
            listRequest.Fields = "files(id, name, mimeType, description, webViewLink)";
            var fileList = listRequest.Execute();
            List<File> files = fileList.Files.ToList();
            while (fileList.NextPageToken != null)
            {
                listRequest.PageToken = fileList.NextPageToken;
                fileList = listRequest.Execute();
                files.AddRange(fileList.Files);
            }
            DriveFolder[] tmp = files.Select(t => new DriveFolder(t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
            return tmp;
        }

        public static DriveFolder CreateFolder(Connection connection, string name, DriveFolder root = null)
        {
            string rootId = root == null ? "root" : root.Id;

            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");

            File fileMetaData = new File {Name = name, MimeType = "application/vnd.google-apps.folder", Parents = new [] {rootId}};
            var action = connection.Service.Files.Create(fileMetaData);
            action.Fields = "id, name, mimeType, description, webViewLink";
            File file = action.Execute();
            return new DriveFolder(file.Id, file.Name, file.Description, file.WebViewLink);
        }

        public static string DeleteFolder(Connection connection, DriveFolder fldr)
        {
            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");

            var request = connection.Service.Files.Delete(fldr.Id);
            var res = request.Execute();//FIXME
            return res;
        }

        private static DriveRole TranslateRole(string role)
        {
            switch (role)
            {
                case "owner":
                    return DriveRole.owner;
                case "organizer":
                    return DriveRole.organizer;
                case "fileOrganizer":
                    return DriveRole.fileOrganizer;
                case "writer":
                    return DriveRole.writer;
                case "commenter":
                    return DriveRole.commenter;
                case "reader":
                    return DriveRole.reader;
                default:
                    return DriveRole.unknown;
            }
        }

        private static DrivePermType TranslatePermType(string role)
        {
            switch (role)
            {
                case "user":
                    return DrivePermType.user;
                case "group":
                    return DrivePermType.group;
                case "domain":
                    return DrivePermType.domain;
                case "anyone":
                    return DrivePermType.anyone;
                default:
                    return DrivePermType.unknown;
            }
        }

        public static DrivePermission[] GetFolderPermissions(Connection connection, DriveFolder fldr)
        {
            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");
            var req = connection.Service.Files.Get(fldr.Id);
            req.Fields = "permissions";
            var response = req.Execute();

            if (response.Permissions == null)
                return new DrivePermission[] { };

            return response.Permissions.Select(x => new DrivePermission(x.Id, x.EmailAddress, TranslatePermType(x.Type), TranslateRole(x.Role))).ToArray();
        }

        public static DrivePermission SetFolderPermissions(Connection connection, DriveFolder foldr, DrivePermission permission)/*DrivePermType type,
            DriveRole role, string email = null)*/
        {
            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");
            if(permission.Type != DrivePermType.anyone && permission.Email == null)
                throw new Exception("This permission type requires an email.");
            if(permission.Type == DrivePermType.unknown || permission.Role == DriveRole.unknown)
                throw new Exception("Invalid arguments passed.");
            if(!DoesFolderExist(connection, foldr))
                throw new Exception("Folder wasn't found.");

            var request = connection.Service.Permissions.Create(new Permission()
            {
                EmailAddress = permission.Email,
                Type = permission.Type.ToString(),
                Role = permission.Role.ToString()
            }, foldr.Id);

            var resp = request.Execute();
            return new DrivePermission(resp.Id, resp.EmailAddress, permission.Type, permission.Role);
        }
    }
}
