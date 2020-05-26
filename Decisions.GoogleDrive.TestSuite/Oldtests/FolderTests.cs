using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Decisions;
using Decisions.GoogleDrive;
using Decisions.GoogleDriveTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriveLibraryTests
{
    //[TestClass]
    public class FolderTests
    {
        [TestMethod]
        public void ListFoldersTest()
        {
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] files = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFolder[]));
            Assert.IsTrue(files.Length > 0);
        }

        [TestMethod]
        public void ListSubfoldersTest()
        {
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] files = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFolder[]));

            var subfolders = Drive.GetFolders(connection, files[1]);

            Assert.IsInstanceOfType(subfolders, typeof(DriveFolder[]));
        }

        [TestMethod]
        public void CreateFolderTest()
        {
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
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
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
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
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
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
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] files = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFolder[]));
            Assert.IsTrue(files.Length > 0);

            var perm = Drive.SetFolderPermissions(connection, files[0], new DrivePermission(null, TestData.TestEmail, DrivePermType.user, DriveRole.writer));
            Drive.SetFolderPermissions(connection, files[0], new DrivePermission(null, null, DrivePermType.anyone, DriveRole.reader) );

            Assert.IsTrue(perm.Id != "");
            Debug.WriteLine("Set perm to folder " + files[0].Name);
        }
    }
}
