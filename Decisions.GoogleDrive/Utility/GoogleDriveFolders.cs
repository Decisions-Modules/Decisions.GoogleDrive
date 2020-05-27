﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decisions.GoogleDrive;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace Decisions.GoogleDrive
{
    public static partial class GoogleDrive
    {

        public static bool DoesFolderExist(Connection connection, GoogleDriveFolder folder)
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

        public static GoogleDriveFolder[] GetFolders(Connection connection, GoogleDriveFolder folder = null)
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
            GoogleDriveFolder[] tmp = files.Select(t => new GoogleDriveFolder(t.Id, t.Name, t.Description, t.WebViewLink)).ToArray();
            return tmp;
        }

        public static GoogleDriveFolder CreateFolder(Connection connection, string name, GoogleDriveFolder root = null)
        {
            string rootId = root == null ? "root" : root.Id;

            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");

            File fileMetaData = new File {Name = name, MimeType = "application/vnd.google-apps.folder", Parents = new [] {rootId}};
            var action = connection.Service.Files.Create(fileMetaData);
            action.Fields = "id, name, mimeType, description, webViewLink";
            File file = action.Execute();
            return new GoogleDriveFolder(file.Id, file.Name, file.Description, file.WebViewLink);
        }

        public static string DeleteFolder(Connection connection, GoogleDriveFolder fldr)
        {
            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");

            var request = connection.Service.Files.Delete(fldr.Id);
            var res = request.Execute();//FIXME
            return res;
        }

        private static GoogleDriveRole TranslateRole(string role)
        {
            switch (role)
            {
                case "owner":
                    return GoogleDriveRole.owner;
                case "organizer":
                    return GoogleDriveRole.organizer;
                case "fileOrganizer":
                    return GoogleDriveRole.fileOrganizer;
                case "writer":
                    return GoogleDriveRole.writer;
                case "commenter":
                    return GoogleDriveRole.commenter;
                case "reader":
                    return GoogleDriveRole.reader;
                default:
                    return GoogleDriveRole.unknown;
            }
        }

        private static GoogleDrivePermType TranslatePermType(string role)
        {
            switch (role)
            {
                case "user":
                    return GoogleDrivePermType.user;
                case "group":
                    return GoogleDrivePermType.group;
                case "domain":
                    return GoogleDrivePermType.domain;
                case "anyone":
                    return GoogleDrivePermType.anyone;
                default:
                    return GoogleDrivePermType.unknown;
            }
        }

        public static GoogleDrivePermission[] GetFolderPermissions(Connection connection, GoogleDriveFolder fldr)
        {
            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");
            var req = connection.Service.Files.Get(fldr.Id);
            req.Fields = "permissions";
            var response = req.Execute();

            if (response.Permissions == null)
                return new GoogleDrivePermission[] { };

            return response.Permissions.Select(x => new GoogleDrivePermission(x.Id, x.EmailAddress, TranslatePermType(x.Type), TranslateRole(x.Role))).ToArray();
        }

        public static GoogleDrivePermission SetFolderPermissions(Connection connection, GoogleDriveFolder foldr, GoogleDrivePermission permission)/*DrivePermType type,
            DriveRole role, string email = null)*/
        {
            if (!connection.IsConnected() || connection.Service == null)
                throw new Exception("Invalid connection object.");
            if(permission.Type != GoogleDrivePermType.anyone && permission.Email == null)
                throw new Exception("This permission type requires an email.");
            if(permission.Type == GoogleDrivePermType.unknown || permission.Role == GoogleDriveRole.unknown)
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
            return new GoogleDrivePermission(resp.Id, resp.EmailAddress, permission.Type, permission.Role);
        }
    }
}