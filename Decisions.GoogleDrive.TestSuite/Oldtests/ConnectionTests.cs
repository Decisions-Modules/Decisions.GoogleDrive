using System;
using System.IO;
using System.Threading.Tasks;
using Decisions;
using Decisions.GoogleDrive;
using Decisions.GoogleDriveTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriveLibraryTests
{
    // [TestClass]
    public class ConnectionTests
    {


        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void NoClientSecretTest()
        {
            var credential = TestData.GetCredential();
            credential.ClientId = "";
            credential.ClientSecret = "";

            Connection connection = new Connection();

            connection.Connect(credential);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void InvalidDataStoreTest()
        {
            var credential = TestData.GetCredential();
            credential.DataStore = "D:/Okeysdfd/sdfsdhgf/dsfsf";

            Connection connection = new Connection();

            connection.Connect(credential);
        }

        [TestMethod]
        public void ConnectionTest()
        {
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());

            Assert.IsTrue(connection.IsConnected());
        }
    }
}
