using Decisions.GoogleDrive;
using Decisions.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDriveTests
{
    static class TestData
    {
        public static GoogleDriveUserCredential GetUserCredential()
        {
            return new GoogleDriveUserCredential
            {
                ClientId = "your client id",
                ClientSecret = "your client secret",

                DataStore = @"C:\data\tmp"
            };
        }

        public static GoogleDriveServiceAccountCredential GetServiceAccountCredential()
        {
            return new GoogleDriveServiceAccountCredential
            {
                Email = "service account email",
                PrivateKey = "service account's private key"
            };
        }
        private static string accessToken = "your AccessToken";

        public static OAuthToken GetTokenCredential()
        {

            var token = new OAuthToken();
            token.TokenData = accessToken;
                             
            return token;

        }

        public static string LocalTestDir = @"C:\data\tmp\";
        public static string TestFileName = "test.txt";
        public static string TestFolderName = "_TestFolder_";

        public static string TestEmail = "YourEmail@gmail.com";


        /*
         * In this method you can use one of the following authorization ways:
         *   return Connection.Create(TestData.GetServiceAccountCredential());
         *   return Connection.Create(accessToken, GetUserCredential().ClientId, GetUserCredential().ClientSecret);
         *   return Connection.Create(TestData.GetUserCredential());
         */
        public static Connection GetConnection()
        {
           return Connection.Create(TestData.GetUserCredential());
        }

    }
}
