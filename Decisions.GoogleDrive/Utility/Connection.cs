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
 
        private ICredential _credential;
        internal DriveService Service;

        private void Connect(GoogleDriveCredential clientSecretsData)
        {
            if(String.IsNullOrEmpty(clientSecretsData.ClientId))
                throw new ArgumentNullException("ClientID", "ClientID wasn't specified.");
            if(String.IsNullOrEmpty(clientSecretsData.ClientSecret))
                throw new ArgumentNullException("ClientSecret", "ClientSecret wasn't specified.");
            if(String.IsNullOrEmpty(clientSecretsData.DataStore))
                throw new ArgumentNullException("DataStore", "DataStore wasn't specified.");
            if ( !Directory.Exists(clientSecretsData.DataStore))
                throw new DirectoryNotFoundException($"{clientSecretsData.DataStore} directory is either invalid or wasn't found.");

            UserCredential userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets() {ClientId = clientSecretsData.ClientId, ClientSecret = clientSecretsData.ClientSecret},
                Connection.Scopes,
                Connection.UserIdentifier,
                CancellationToken.None,
                new FileDataStore(clientSecretsData.DataStore, true)).Result;

            _credential = userCredential;

            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = Connection.ApplicationName,
            });
        }

        private void Connect(GoogleDriveServiceAccountCredential serviceAccountCredential)
        {
            if (String.IsNullOrEmpty(serviceAccountCredential.Email))
                throw new ArgumentNullException("serviceAccountCredential.Email", "Email wasn't specified.");
            if (String.IsNullOrEmpty(serviceAccountCredential.PrivateKey))
                throw new ArgumentNullException("serviceAccountCredential.PrivateKey", "privateKey wasn't specified.");

            ServiceAccountCredential serviceCredential = new ServiceAccountCredential(
              new ServiceAccountCredential.Initializer(serviceAccountCredential.Email)
              {
                  Scopes = Connection.Scopes
              }.FromPrivateKey(serviceAccountCredential.PrivateKey) );

            _credential = serviceCredential;

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

        public static Connection Create(GoogleDriveServiceAccountCredential serviceAccountCredential)
        {
            Connection connection = new Connection();
            connection.Connect(serviceAccountCredential);
            if (!connection.IsConnected())
                throw new BusinessRuleException("Cannot connect to GoogleDrive service.");
            return connection;

        }

    }
}
