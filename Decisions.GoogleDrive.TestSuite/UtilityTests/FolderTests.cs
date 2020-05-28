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
   /*  //[TestClass]
    public class FolderTests
    {
     [TestMethod]
        public void ListFoldersTest()
        {
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFolder[] files = GoogleDrive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(GoogleDriveFolder[]));
            Assert.IsTrue(files.Length > 0);
        }

        [TestMethod]
        public void ListSubfoldersTest()
        {
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFolder[] folders = GoogleDrive.GetFolders(connection);

            Assert.IsInstanceOfType(folders, typeof(GoogleDriveFolder[]));

            var subfolders = GoogleDrive.GetFolders(connection, folders[0].Id);

            Assert.IsInstanceOfType(subfolders, typeof(GoogleDriveFolder[]));
        }

        [TestMethod]
        public void CreateFolderTest()
        {
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFolder newFolder = GoogleDrive.CreateFolder(connection, "TEST FOLDER 123");

            Assert.IsTrue(newFolder.Id != "");

            GoogleDriveFolder[] files = GoogleDrive.GetFolders(connection);
            Assert.IsInstanceOfType(files, typeof(GoogleDriveFolder[]));
            Assert.IsTrue(files.Length > 0);

            Assert.IsTrue(files.Any(x => x.Name == "TEST FOLDER 123"));
        }

        [TestMethod]
        public void DeleteFolderTest()
        { 
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFolder[] files = GoogleDrive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(GoogleDriveFolder[]));
            Assert.IsTrue(files.Length > 0);

            var folder = GoogleDrive.CreateFolder(connection, "OkFolder");
            GoogleDrive.DeleteFolder(connection, folder.Id);
        }

        [TestMethod]
        public void GetPermsTest()
        {
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFolder[] files = GoogleDrive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(GoogleDriveFolder[]));
            Assert.IsTrue(files.Length > 0);

            var perms = GoogleDrive.GetFolderPermissions(connection, files[0].Id);

            Assert.IsTrue(perms.Any(x => x.Role == GoogleDriveRole.owner && x.Type == GoogleDrivePermType.user));
        }

        [TestMethod]
        public void SetPermsTest()
        {
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFolder[] files = GoogleDrive.GetFolders(connection);

            Assert.IsInstanceOfType(files, typeof(GoogleDriveFolder[]));
            Assert.IsTrue(files.Length > 0);

            var perm = GoogleDrive.SetFolderPermissions(connection, files[0].Id, new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer));
            GoogleDrive.SetFolderPermissions(connection, files[0].Id, new GoogleDrivePermission(null, null, GoogleDrivePermType.anyone, GoogleDriveRole.reader) );

            Assert.IsTrue(perm.Id != "");
            Debug.WriteLine("Set perm to folder " + files[0].Name);
        }
    }*/
}
