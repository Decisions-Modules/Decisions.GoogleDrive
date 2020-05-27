using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Decisions.GoogleDrive;
using DecisionsFramework;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Decisions.GoogleDrive
{
    public class Connection
    {
        private static readonly string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };
        private static readonly string ApplicationName = "Decisions.Google.Drive";
        private static readonly string UserIdentifier = "Decisions.Google.Drive";

        public Connection()
        {
        }
 
        private UserCredential _credential;
        internal DriveService Service;

        public void Connect(GoogleDriveCredential credential)
        {
            if(String.IsNullOrEmpty(credential.ClientId))
                throw new ArgumentNullException("ClientID wasn't specified.");
            if(credential.ClientSecret == "")
                throw new InvalidDataException("ClientSecret wasn't specified.");
            if(credential.DataStore == "" || !Directory.Exists(credential.DataStore))
                throw new DirectoryNotFoundException("The DataStore directory is either invalid or wasn't found.");

            _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets() {ClientId = credential.ClientId, ClientSecret = credential.ClientSecret},
                Connection.Scopes,
                Connection.UserIdentifier,
                CancellationToken.None,
                new FileDataStore(credential.DataStore, true)).Result;

            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = Connection.ApplicationName,
            });
        }

        public bool IsConnected()
        {
            if (_credential == null)
                return false;
            return true;
        }

        public static Connection Create(GoogleDriveCredential credential)
        {
            Connection connection = new Connection();
            connection.Connect(credential);
            if (!connection.IsConnected())
                throw new BusinessRuleException("Cannot connect to GoogleDrive service.");
            return connection;

        }
    }
}
