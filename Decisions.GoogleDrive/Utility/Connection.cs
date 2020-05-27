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

        private Connection()
        {
        }
 
        private UserCredential _credential;
        internal DriveService Service;

        private void Connect(GoogleDriveCredential credential)
        {
            if(String.IsNullOrEmpty(credential.ClientId))
                throw new ArgumentNullException("ClientID", "ClientID wasn't specified.");
            if(String.IsNullOrEmpty(credential.ClientSecret))
                throw new ArgumentNullException("ClientSecret", "ClientSecret wasn't specified.");
            if(String.IsNullOrEmpty(credential.DataStore))
                throw new ArgumentNullException("DataStore", "DataStore wasn't specified.");
            if ( !Directory.Exists(credential.DataStore))
                throw new DirectoryNotFoundException($"{credential.DataStore} directory is either invalid or wasn't found.");

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
            if (_credential == null || Service == null)
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
