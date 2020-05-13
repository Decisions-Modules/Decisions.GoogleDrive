using System;
using System.IO;
using System.Threading.Tasks;
using DriveLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriveLibraryTests
{
    [TestClass]
    public class ConnectionTests
    {
        public static string clientId = "ENTER CLIENT ID HERE";
        public static string clientSecret = "ENTER CLIENT SECRET HERE";
        public static string dataStore = "D:/TestFolder/";

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void NoClientSecretTest()
        {
            Drive.ClientId = "";
            Drive.ClientSecret = "";

            Connection connection = new Connection("test");

            connection.Connect();
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void InvalidDataStoreTest()
        {
            Drive.ClientId = clientId;
            Drive.ClientSecret = clientSecret;
            Drive.DataStore = "D:/Okeysdfd/sdfsdhgf/dsfsf";

            Connection connection = new Connection("test");

            connection.Connect();
        }

        [TestMethod]
        public void ConnectionTest()
        {
            Drive.ClientId = clientId;
            Drive.ClientSecret = clientSecret;
            Drive.DataStore = dataStore;
            Connection connection = new Connection("test");
            connection.Connect();

            Assert.IsTrue(connection.IsConnected());
        }
    }
}
