using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace DriveLibrary
{
    public class Connection
    {
        public string UserIdentifier;

        public Connection(string identifier)
        {
            UserIdentifier = identifier;
        }

        
        private UserCredential _credential;
        internal DriveService Service;

        public void Connect()
        {
            if(Drive.ClientId == "")
                throw new InvalidDataException("ClientID wasn't specified.");
            if(Drive.ClientSecret == "")
                throw new InvalidDataException("ClientSecret wasn't specified.");
            if(Drive.DataStore == "" || !Directory.Exists(Drive.DataStore))
                throw new DirectoryNotFoundException("The DataStore directory is either invalid or wasn't found.");

            _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets() {ClientId = Drive.ClientId, ClientSecret = Drive.ClientSecret}, 
                Drive.Scopes,
                UserIdentifier,
                CancellationToken.None,
                new FileDataStore(Drive.DataStore, true)).Result;

            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = Drive.ApplicationName,
            });
        }

        public bool IsConnected()
        {
            if (_credential == null)
                return false;
            return true;
        }
    }
}
