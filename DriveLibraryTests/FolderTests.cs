using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DriveLibrary;
using DriveLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriveLibraryTests
{
    [TestClass]
    public class FolderTests
    {
        [TestMethod]
        public void ListFoldersTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] files = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFolder[]));
            Assert.IsTrue(files.Length > 0);
        }

        [TestMethod]
        public void ListSubfoldersTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] files = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFolder[]));

            var subfolders = Drive.GetFolders(connection, files[1]);

            Assert.IsInstanceOfType(subfolders, typeof(DriveFolder[]));
        }

        [TestMethod]
        public void CreateFolderTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            DriveFolder newFolder = Drive.CreateFolder(connection, "TEST FOLDER 123");

            Assert.IsTrue(newFolder.Id != "");

            DriveFolder[] files = Drive.GetFolders(connection);
            Assert.IsInstanceOfType(files, typeof(DriveFolder[]));
            Assert.IsTrue(files.Length > 0);

            Assert.IsTrue(files.Any(x => x.Name == "TEST FOLDER 123"));
        }

        [TestMethod]
        public void DeleteFolderTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] files = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFolder[]));
            Assert.IsTrue(files.Length > 0);

            var folder = Drive.CreateFolder(connection, "OkFolder");
            Drive.DeleteFolder(connection, folder);
        }

        [TestMethod]
        public void GetPermsTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] files = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFolder[]));
            Assert.IsTrue(files.Length > 0);

            var perms = Drive.GetFolderPermissions(connection, files[0]);

            Assert.IsTrue(perms.Any(x => x.Role == DriveRole.owner && x.Type == DrivePermType.user));
        }

        [TestMethod]
        public void SetPermsTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] files = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFolder[]));
            Assert.IsTrue(files.Length > 0);

            var perm = Drive.SetFolderPermissions(connection, files[0], DrivePermType.user, DriveRole.writer, "gasser45123@gmail.com");
            Drive.SetFolderPermissions(connection, files[0], DrivePermType.anyone, DriveRole.reader);

            Assert.IsTrue(perm.Id != "");
            Debug.WriteLine("Set perm to folder " + files[0].Name);
        }
    }
}
