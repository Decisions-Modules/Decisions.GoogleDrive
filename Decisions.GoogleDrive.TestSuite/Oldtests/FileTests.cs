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
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            DriveFile[] files = Drive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFile[]));
            Assert.IsTrue(files.Length > 0);
        }

        [TestMethod]
        public void ListFilesInFolderTest()
        {

            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] folders = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(folders, typeof(DriveFolder[]));
            Assert.IsTrue(folders.Length > 0);

            var files = Drive.GetFiles(connection, folders[1]);

            Assert.IsInstanceOfType(files, typeof(DriveFile[]));
            Assert.IsTrue(files.Length > 0);
        }

        [TestMethod]
        public void DeleteFileTest()
        {
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());
            UploadFileTest();
            var file = (Drive.GetFiles(connection)).First(x => x.Name == TestData.TestFileName);
            Drive.DeleteFile(connection, file);
            file = (Drive.GetFiles(connection)).FirstOrDefault(x => x.Name == TestData.TestFileName);
            Assert.IsNull(file);
        }

        [TestMethod]
        public void GetPermsTest()
        {
            UploadFileTest();

            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            DriveFile[] files = Drive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFile[]));
            Assert.IsTrue(files.Length > 0);

            var perms = Drive.GetFilePermissions(connection, files.First(x => x.Name == TestData.TestFileName));

            Assert.IsTrue(perms.Any(x => x.Role == DriveRole.owner && x.Type == DrivePermType.user));
        }

        [TestMethod]
        public void SetPermsTest()
        {
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            DriveFile[] files = Drive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFile[]));
            Assert.IsTrue(files.Length > 0);

            var file = files.First(x => x.Name == TestData.TestFileName);
            var perm = Drive.SetFilePermissions(connection, file, new DrivePermission(null, TestData.TestEmail, DrivePermType.user, DriveRole.writer) );
            Drive.SetFilePermissions(connection, file, new DrivePermission(null, null, DrivePermType.anyone, DriveRole.reader) );

            Assert.IsTrue(perm.Id != "");
            Debug.WriteLine("Set perm to file " + file.Name);
        }

        [TestMethod]
        public void DownloadFileTest()
        {
            UploadFileTest();
            File.Delete(TestFileFullName);

            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            DriveFile[] files = Drive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFile[]));
            Assert.IsTrue(files.Length > 0);

            var file = files.First(x => x.Name == TestData.TestFileName);
            using (FileStream fs = File.OpenWrite(TestFileFullName))
            {
                var status = Drive.DownloadFile(connection, file, fs, progress =>
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
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            using (FileStream fs = File.OpenRead(TestFileFullName))
            {
                var file = Drive.UploadFile(connection, fs, TestData.TestFileName, null, progress =>
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
            Connection connection = new Connection();
            connection.Connect(TestData.GetCredential());
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] folders = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(folders, typeof(DriveFolder[]));
            Assert.IsTrue(folders.Length > 0);

            using (FileStream fs = File.OpenRead(TestFileFullName))
            {
                var file = Drive.UploadFile(connection, fs, TestData.TestFileName, folders[0], progress =>
                {
                    Debug.WriteLine("Progress: " + progress.BytesSent);
                });
                Assert.IsTrue(file.Id != "" && file.SharingLink != "");
                Debug.WriteLine("File link: " + file.SharingLink);
            }
        }
    }
}
