using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Decisions.GoogleDrive;
using Decisions.OAuth;
using DecisionsFramework;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
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

        private ICredential credential;
        internal DriveService Service;

        private void Connect(GoogleDriveUserCredential clientSecretsData)
        {
            if (String.IsNullOrEmpty(clientSecretsData.ClientId))
                throw new ArgumentNullException("ClientID", "ClientID wasn't specified.");
            if (String.IsNullOrEmpty(clientSecretsData.ClientSecret))
                throw new ArgumentNullException("ClientSecret", "ClientSecret wasn't specified.");
            if (String.IsNullOrEmpty(clientSecretsData.DataStore))
                throw new ArgumentNullException("DataStore", "DataStore wasn't specified.");
            if (!Directory.Exists(clientSecretsData.DataStore))
                throw new DirectoryNotFoundException($"{clientSecretsData.DataStore} directory is either invalid or wasn't found.");

            UserCredential cr = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets() { ClientId = clientSecretsData.ClientId, ClientSecret = clientSecretsData.ClientSecret },
                Connection.Scopes,
                Connection.UserIdentifier,
                CancellationToken.None,
                new FileDataStore(clientSecretsData.DataStore, true)).Result;
            credential = cr;

            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Connection.ApplicationName,
            });
        }

        private void Connect(GoogleDriveServiceAccountCredential serviceAccountCredential)
        {
            if (String.IsNullOrEmpty(serviceAccountCredential.Email))
                throw new ArgumentNullException("serviceAccountCredential.Email", "Email wasn't specified.");
            if (String.IsNullOrEmpty(serviceAccountCredential.PrivateKey))
                throw new ArgumentNullException("serviceAccountCredential.PrivateKey", "privateKey wasn't specified.");

            credential = new ServiceAccountCredential(
              new ServiceAccountCredential.Initializer(serviceAccountCredential.Email)
              {
                  Scopes = Connection.Scopes
              }.FromPrivateKey(serviceAccountCredential.PrivateKey));

            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Connection.ApplicationName,
            });
        }

        private void Connect(string accessToken, string consumerKey, string consumerSecret)
        {
            if (String.IsNullOrEmpty(accessToken) )
                throw new ArgumentNullException("token", "token wasn't specified.");

            Google.Apis.Auth.OAuth2.Flows.GoogleAuthorizationCodeFlow googleAuthFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = consumerKey,
                    ClientSecret = consumerSecret,
                }
            });

            Google.Apis.Auth.OAuth2.Responses.TokenResponse responseToken = new TokenResponse()
            {
                AccessToken = accessToken,
                ExpiresInSeconds = 3599,
                Issued  = DateTime.Now,
                IssuedUtc = DateTime.UtcNow,
                TokenType = "Bearer",
            };

            credential = new UserCredential(googleAuthFlow, "", responseToken);

            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Connection.ApplicationName,
            });
        }

        public bool IsConnected()
        {
            if (credential == null || Service == null)
                return false;
            return true;
        }

        public static Connection Create(string anAccessToken, string ConsumerKey, string ConsumerSecret)
        {
            Connection connection = new Connection();
            connection.Connect(anAccessToken, ConsumerKey, ConsumerSecret);
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

        public static Connection Create(GoogleDriveUserCredential credential)
        {
            Connection connection = new Connection();

            connection.Connect(credential);

            if (!connection.IsConnected())
                throw new BusinessRuleException("Cannot connect to GoogleDrive service.");
            return connection;
        }

    }
}
