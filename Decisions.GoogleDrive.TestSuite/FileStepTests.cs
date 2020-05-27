using Decisions.GoogleDrive;
using Decisions.GoogleDriveTests;
using DriveLibrary.Steps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDriveTests
{
    [TestClass]
    public class FileStepTests
    {
        string TestFileFullName { get { return TestData.LocalTestDir + TestData.TestFileName; } }

        private readonly GoogleDriveCredential credentional = TestData.GetCredential();
        private GoogleDriveFolder testFolder;

        [TestInitialize]
        public void InitTests()
        {
            var stream = new System.IO.StreamWriter(TestFileFullName);
            for (int i = 0; i < 1000; i++)
                stream.Write($"{i}qwertyuiop\n");
            stream.Close();

            testFolder = FolderSteps.CreateFolder(credentional, null, TestData.TestFolderName);

        }

        [TestCleanupAttribute]
        public void CleanupTests()
        {
            File.Delete(TestFileFullName);
            FolderSteps.DeleteFolder(credentional, testFolder.Id);
        }

        [TestMethod]
        public void ListFilesTest()
        {
            var rootFileList=FileSteps.GetFileList(credentional, null);
            var testFolderFileList = FileSteps.GetFileList(credentional, testFolder.Id);

            Assert.IsTrue(rootFileList.Length > 0 || testFolderFileList.Length > 0);
        }
        
        [TestMethod]
        public void ListFilesInFolderTest()
        {
            var file = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);

            try
            {
                var testFolderFileList = FileSteps.GetFileList(credentional, testFolder.Id);
                Assert.IsTrue(testFolderFileList.Length > 0);
            }
            finally {
                FileSteps.DeleteFile(credentional, file.Id);
            }
        }

        [TestMethod]
        public void DeleteFileTest()
        {
            var file = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var fileListBefore = FileSteps.GetFileList(credentional, testFolder.Id);
                Assert.IsTrue(fileListBefore.Length > 0);

                FileSteps.DeleteFile(credentional, file.Id);

                var fileListAfter = FileSteps.GetFileList(credentional, testFolder.Id);

                Assert.AreEqual(1, fileListBefore.Length - fileListAfter.Length);
            }
            finally
            {
                try
                {
                    FileSteps.DeleteFile(credentional, file.Id);
                }
                catch { }
            }
        }

        [TestMethod]
        public void GetPermsTest()
        {
            var file = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var permissions = FileSteps.GetFilePermissions(credentional, file.Id);
                Assert.IsTrue(permissions.Length > 0);
            }
            finally
            {
                FileSteps.DeleteFile(credentional, file.Id);
            }
        }

        [TestMethod]
        public void SetPermsTest()
        {
            var file = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var newPermission = new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer);
                var permission = FileSteps.SetFilePermissions(credentional, file.Id, newPermission);
                Assert.IsTrue(permission.Id != "");
            }
            finally
            {
                FileSteps.DeleteFile(credentional, file.Id);
            }
        }

        [TestMethod]
        public void DownloadFileTest()
        {
            var file = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
            string localFileName = TestFileFullName + "_";
            try
            {
                var fileListAfter = FileSteps.DownloadFile(credentional, file.Id, localFileName);
                Assert.IsTrue(File.Exists(localFileName));

                var origLen = new System.IO.FileInfo(TestFileFullName).Length;
                var actualLen = new System.IO.FileInfo(localFileName).Length;

                Assert.AreEqual(origLen, actualLen);
            }
            finally
            {
                FileSteps.DeleteFile(credentional, file.Id);
            }
        }

        [TestMethod]
        public void UploadFileTest()
        {
            GoogleDriveFile file = null;
            try
            {
                file = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
                Assert.IsNotNull(file); 
            }
            finally
            {
                if(file != null)
                  FileSteps.DeleteFile(credentional, file.Id);
            }
        }

    }
}
