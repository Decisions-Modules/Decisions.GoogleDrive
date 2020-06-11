using System;
using System.IO;
using System.Threading.Tasks;
using Decisions;
using Decisions.GoogleDrive;
using Decisions.GoogleDriveTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decisions.GoogleDriveTests
{
     [TestClass]
    public class ConnectionTests
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void NoClientSecretTest()
        {
            var credential = TestData.GetUserCredential();
            credential.ClientId = "";
            credential.ClientSecret = "";

            Connection connection = Connection.Create(credential);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void InvalidDataStoreTest()
        {
            var credential = TestData.GetUserCredential();
            credential.DataStore = "D:/Okeysdfd/sdfsdhgf/dsfsf";

            Connection connection = Connection.Create(credential);
        }

        [TestMethod]
        public void ConnectionTest()
        {
            Connection connection = Connection.Create(TestData.GetUserCredential());
            Assert.IsTrue(connection.IsConnected());
        }

        [TestMethod]
        public void ServiceAccountConnectionTest()
        {
            Connection connection = Connection.Create(TestData.GetServiceAccountCredential());
            Assert.IsTrue(connection.IsConnected());
        }

    }
}
