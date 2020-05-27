using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decisions;
using Decisions.GoogleDrive;
using Decisions.GoogleDriveTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriveLibraryTests
{
   //[TestClass]
    public class FileTests
    {
        string TestFileFullName { get { return TestData.LocalTestDir + TestData.TestFileName; } }

        [TestInitialize]
        public void InitTests()
        {
            var stream = new System.IO.StreamWriter(TestFileFullName);
            stream.Write("qwertyuiop");
            stream.Close();
                
        }
        [TestCleanupAttribute]
        public void CleanupTests()
        {
            File.Delete(TestFileFullName);
        }

        [TestMethod]
        public void ListFilesTest()
        {
            Connection connection =  Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFile[] files = GoogleDrive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(GoogleDriveFile[]));
            Assert.IsTrue(files.Length > 0);
        }

        [TestMethod]
        public void ListFilesInFolderTest()
        {

            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFolder[] folders = GoogleDrive.GetFolders(connection);

            Assert.IsInstanceOfType(folders, typeof(GoogleDriveFolder[]));
            Assert.IsTrue(folders.Length > 0);

            var files = GoogleDrive.GetFiles(connection, folders[0].Id);

            Assert.IsInstanceOfType(files, typeof(GoogleDriveFile[]));
            Assert.IsTrue(files.Length > 0);
        }

        [TestMethod]
        public void DeleteFileTest()
        {
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());
            UploadFileTest();
            var file = (GoogleDrive.GetFiles(connection)).First(x => x.Name == TestData.TestFileName);
            GoogleDrive.DeleteFile(connection, file.Id);
            file = (GoogleDrive.GetFiles(connection)).FirstOrDefault(x => x.Name == TestData.TestFileName);
            Assert.IsNull(file);
        }

        [TestMethod]
        public void GetPermsTest()
        {
            UploadFileTest();

            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFile[] files = GoogleDrive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(GoogleDriveFile[]));
            Assert.IsTrue(files.Length > 0);

            var file = files.First(x => x.Name == TestData.TestFileName);
            var perms = GoogleDrive.GetFilePermissions(connection, file.Id);

            Assert.IsTrue(perms.Any(x => x.Role == GoogleDriveRole.owner && x.Type == GoogleDrivePermType.user));
        }

        [TestMethod]
        public void SetPermsTest()
        {
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFile[] files = GoogleDrive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(GoogleDriveFile[]));
            Assert.IsTrue(files.Length > 0);

            var file = files.First(x => x.Name == TestData.TestFileName);
            var perm = GoogleDrive.SetFilePermissions(connection, file.Id, new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer) );
            GoogleDrive.SetFilePermissions(connection, file.Id, new GoogleDrivePermission(null, null, GoogleDrivePermType.anyone, GoogleDriveRole.reader) );

            Assert.IsTrue(perm.Id != "");
            Debug.WriteLine("Set perm to file " + file.Name);
        }

        [TestMethod]
        public void DownloadFileTest()
        {
            UploadFileTest();
            File.Delete(TestFileFullName);

            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFile[] files = GoogleDrive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(GoogleDriveFile[]));
            Assert.IsTrue(files.Length > 0);

            var file = files.First(x => x.Name == TestData.TestFileName);
            using (FileStream fs = File.OpenWrite(TestFileFullName))
            {
                var status = GoogleDrive.DownloadFile(connection, file.Id, fs, progress =>
                {
                    Debug.WriteLine("Progress: " + progress.BytesDownloaded);
                });
                Assert.IsTrue(status);
            }

            Assert.IsTrue(File.Exists(TestFileFullName));
        }

        [TestMethod]
        public void UploadFileTest()
        {
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            using (FileStream fs = File.OpenRead(TestFileFullName))
            {
                var file = GoogleDrive.UploadFile(connection, fs, TestData.TestFileName, null, progress =>
                {
                    Debug.WriteLine("Progress: " + progress.BytesSent);
                });
                Assert.IsTrue(file.Id != "" && file.SharingLink != "");
                Debug.WriteLine("File link: " + file.SharingLink);
            }
        }

        [TestMethod]
        public void UploadFileInSubFolderTest()
        {
            Connection connection = Connection.Create(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            GoogleDriveFolder[] folders = GoogleDrive.GetFolders(connection);

            Assert.IsInstanceOfType(folders, typeof(GoogleDriveFolder[]));
            Assert.IsTrue(folders.Length > 0);

            using (FileStream fs = File.OpenRead(TestFileFullName))
            {
                var file = GoogleDrive.UploadFile(connection, fs, TestData.TestFileName, folders[0].Id, progress =>
                {
                    Debug.WriteLine("Progress: " + progress.BytesSent);
                });
                Assert.IsTrue(file.Id != "" && file.SharingLink != "");
                Debug.WriteLine("File link: " + file.SharingLink);
            }
        }
    }
}
