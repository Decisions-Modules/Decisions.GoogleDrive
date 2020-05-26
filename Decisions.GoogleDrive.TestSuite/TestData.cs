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
        public static DriveCredential GetCredential()
        {
            return new DriveCredential
            {
                ClientId = "your client id",
                ClientSecret = "your client secret",
                DataStore = @"C:\data\tmp"
            };
        }

        public static string LocalTestDir = @"C:\data\tmp\";
        public static string TestFileName = "test.txt";
        public static string TestFolderName = "_TestFolder_";

        public static string TestEmail= "gasser45123@gmail.com";


    }
}
