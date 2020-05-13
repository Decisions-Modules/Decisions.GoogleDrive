using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveLibrary.Models;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace DriveLibrary
{
    public static partial class Drive
    {
        internal static string[] Scopes = { DriveService.Scope.Drive };
        public static string ApplicationName = "";
        public static string ClientId = "";
        public static string ClientSecret = "";
        public static string DataStore = "";

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
            DriveFolder[] tmp = files.Select(t => new DriveFolder(connection, t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
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
            return new DriveFolder(connection, file.Id, file.Name, file.Description, file.WebViewLink);
        }

        public static void DeleteFolder(Connection connection, DriveFolder fldr)
        {
            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");

            var request = connection.Service.Files.Delete(fldr.Id);
            request.Execute();
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

        public static DrivePermission SetFolderPermissions(Connection connection, DriveFolder foldr, DrivePermType type,
            DriveRole role, string email = null)
        {
            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");
            if(type != DrivePermType.anyone && email == null)
                throw new Exception("This permission type requires an email.");
            if(type == DrivePermType.unknown || role == DriveRole.unknown)
                throw new Exception("Invalid arguments passed.");
            if(!DoesFolderExist(connection, foldr))
                throw new Exception("Folder wasn't found.");

            var request = connection.Service.Permissions.Create(new Permission()
            {
                EmailAddress = email,
                Type = type.ToString(),
                Role = role.ToString()
            }, foldr.Id);

            var resp = request.Execute();
            return new DrivePermission(resp.Id, resp.EmailAddress, type, role);
        }
    }
}
