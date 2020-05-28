using Decisions.GoogleDrive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDriveTests
{
    static class TestData
    {
        public static GoogleDriveCredential GetCredential()
        {
            return new GoogleDriveCredential
            {
                ClientId = "your client id",
                ClientSecret = "your client secret",

                DataStore = @"C:\data\tmp"
            };
        }

        public static string LocalTestDir = @"C:\data\tmp\";
        public static string TestFileName = "test.txt";
        public static string TestFolderName = "_TestFolder_";

        public static string TestEmail= "kovalchuk.i.v.1976@gmail.com";

    }
}
