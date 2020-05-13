using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveLibrary;
using DriveLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriveLibraryTests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public void ListFilesTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            DriveFile[] files = Drive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFile[]));
            Assert.IsTrue(files.Length > 0);
        }

        [TestMethod]
        public void ListFilesInFolderTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
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
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());
            UploadFileTest();
            var file = (Drive.GetFiles(connection)).First(x => x.Name == "test.txt");
            Drive.DeleteFile(connection, file);
            file = (Drive.GetFiles(connection)).FirstOrDefault(x => x.Name == "test.txt");
            Assert.IsNull(file);
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

            DriveFile[] files = Drive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFile[]));
            Assert.IsTrue(files.Length > 0);

            var perms = Drive.GetFilePermissions(connection, files.First(x => x.Name == "au480dev.rar"));

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

            DriveFile[] files = Drive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFile[]));
            Assert.IsTrue(files.Length > 0);

            var fle = files.First(x => x.Name == "au480dev.rar");
            var perm = Drive.SetFilePermissions(connection, fle, DrivePermType.user, DriveRole.writer, "gasser45123@gmail.com");
            Drive.SetFilePermissions(connection, fle, DrivePermType.anyone, DriveRole.reader);

            Assert.IsTrue(perm.Id != "");
            Debug.WriteLine("Set perm to file " + fle.Name);
        }

        [TestMethod]
        public void DownloadFileTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            DriveFile[] files = Drive.GetFiles(connection);

            Assert.IsInstanceOfType(files, typeof(DriveFile[]));
            Assert.IsTrue(files.Length > 0);

            var fle = files.First(x => x.Name == "au480dev.rar");
            using (FileStream fs = File.OpenWrite("D:/temp.rar"))
            {
                var status = Drive.DownloadFile(connection, fle, fs, progress =>
                {
                    Debug.WriteLine("Progress: " + progress.BytesDownloaded);
                });
                Assert.IsTrue(status);
            }

            Assert.IsTrue(File.Exists("D:/temp.rar"));
        }

        [TestMethod]
        public void UploadFileTest()
        {
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            using (FileStream fs = File.OpenRead("D:/test.txt"))
            {
                var file = Drive.UploadFile(connection, fs, "test.txt", null, progress =>
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
            Drive.ClientId = ConnectionTests.clientId;
            Drive.ClientSecret = ConnectionTests.clientSecret;
            Drive.DataStore = ConnectionTests.dataStore;
            Connection connection = new Connection("test");
            connection.Connect();
            Assert.IsTrue(connection.IsConnected());

            DriveFolder[] folders = Drive.GetFolders(connection);

            Assert.IsInstanceOfType(folders, typeof(DriveFolder[]));
            Assert.IsTrue(folders.Length > 0);

            using (FileStream fs = File.OpenRead("D:/test.txt"))
            {
                var file = Drive.UploadFile(connection, fs, "test.txt", folders[0], progress =>
                {
                    Debug.WriteLine("Progress: " + progress.BytesSent);
                });
                Assert.IsTrue(file.Id != "" && file.SharingLink != "");
                Debug.WriteLine("File link: " + file.SharingLink);
            }
        }
    }
}
