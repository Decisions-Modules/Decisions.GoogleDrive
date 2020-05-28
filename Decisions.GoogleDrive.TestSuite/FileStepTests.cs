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
            const int LINE_COUNT = 3;
            var stream = new System.IO.StreamWriter(TestFileFullName);
            for (int i = 0; i < LINE_COUNT; i++)
                stream.Write($"{i}qwertyuiop\n");
            stream.Close();

            testFolder = FolderSteps.CreateFolder(credentional, null, TestData.TestFolderName).Data;

        }

        [TestCleanupAttribute]
        public void CleanupTests()
        {
            File.Delete(TestFileFullName);
            FolderSteps.DeleteFolder(credentional, testFolder.Id);
        }

        [TestMethod]
        public void DoesFileExistTest()
        {

            var shouldBeFalse = FileSteps.DoesFileExist(credentional, "incorrect Id");
            Assert.IsFalse(shouldBeFalse.Data);
            
            var uploadResult = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);

            try
            {
                var shouldBeTrue = FileSteps.DoesFileExist(credentional, uploadResult.Data.Id);
                Assert.IsTrue(shouldBeTrue.Data);
            }
            finally
            {
                FileSteps.DeleteFile(credentional, uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void ListFilesTest()
        {
                var rootFileList = FileSteps.GetFileList(credentional, null);
                Assert.IsTrue(rootFileList.IsSucceed);
        }
        
        [TestMethod]
        public void ListFilesInFolderTest()
        {
            const int FILE_COUNT = 110;
            List<GoogleDriveFile> uploadedFiles = new List<GoogleDriveFile>(FILE_COUNT);

            try
            {
                var rootFileList = FileSteps.GetFileList(credentional, null);
                Assert.IsTrue(rootFileList.IsSucceed);

                for (int i = 0; i < FILE_COUNT; i++)
                    uploadedFiles.Add(FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName).Data);

                var testFolderFileList = FileSteps.GetFileList(credentional, testFolder.Id);
                Assert.IsTrue(testFolderFileList.IsSucceed);
                Assert.AreEqual(FILE_COUNT, testFolderFileList.Data.Length);
            }
            finally
            {
                uploadedFiles.ForEach(file => FileSteps.DeleteFile(credentional, file.Id));
            }
        }

        [TestMethod]
        public void DeleteFileTest()
        {
            var uploadResult = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var fileListBefore = FileSteps.GetFileList(credentional, testFolder.Id);
                Assert.IsTrue(fileListBefore.IsSucceed);

                var delResult=FileSteps.DeleteFile(credentional, uploadResult.Data.Id);
                Assert.IsTrue(delResult.IsSucceed);

                var fileListAfter = FileSteps.GetFileList(credentional, testFolder.Id);
                Assert.IsTrue(fileListAfter.IsSucceed);

                Assert.AreEqual(1, fileListBefore.Data.Length - fileListAfter.Data.Length);
            }
            finally
            {
                try
                {
                    FileSteps.DeleteFile(credentional, uploadResult.Data.Id);
                }
                catch { }
            }
        }

        [TestMethod]
        public void GetPermsTest()
        {
            var uploadResult = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var permissionResult = FileSteps.GetFilePermissions(credentional, uploadResult.Data.Id);
                Assert.IsTrue(permissionResult.IsSucceed);
                Assert.IsTrue(permissionResult.Data.Length > 0);
            }
            finally
            {
                FileSteps.DeleteFile(credentional, uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void SetPermsTest()
        {
            var uploadResult = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var newPermission = new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer);
                var permission = FileSteps.SetFilePermissions(credentional, uploadResult.Data.Id, newPermission);
                Assert.IsTrue(permission.IsSucceed);
            }
            finally
            {
                FileSteps.DeleteFile(credentional, uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void SetIncorrectPermsTest()
        {
            var newPermission = new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer);
            var permissionResult = FileSteps.SetFilePermissions(credentional, "incorrect Id", newPermission);
            Assert.IsFalse( permissionResult.IsSucceed);

        }

        [TestMethod]
        public void DownloadFileTest()
        {
            var uploadResult = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
            string localFileName = TestFileFullName + "_";
            try
            {
                var downloadResult = FileSteps.DownloadFile(credentional, uploadResult.Data.Id, localFileName);
                Assert.IsTrue(downloadResult.IsSucceed);
                Assert.IsTrue(File.Exists(localFileName));

                var origLen = new System.IO.FileInfo(TestFileFullName).Length;
                var actualLen = new System.IO.FileInfo(localFileName).Length;

                Assert.AreEqual(origLen, actualLen);

            }
            finally
            {
                FileSteps.DeleteFile(credentional, uploadResult.Data.Id);
                System.IO.File.Delete(localFileName);
            }
        }

        [TestMethod]
        public void DownloadIncorrectFileTest()
        {
            string localFileName = TestFileFullName + "_";
            try
            {
                var downloadResult = FileSteps.DownloadFile(credentional, "incorrect id", localFileName);
                Assert.IsFalse( downloadResult.IsSucceed);

            }
            finally
            {
                System.IO.File.Delete(localFileName);
            }
        }

        [TestMethod]
        public void UploadFileTest()
        {
            GoogleDriveResultWithData<GoogleDriveFile> uploadResult = null;
            try
            {
                uploadResult = FileSteps.UploadFile(credentional, testFolder.Id, TestFileFullName);
                Assert.IsTrue(uploadResult.IsSucceed);
                Assert.IsNotNull(uploadResult.Data);
            }
            finally
            {
                if(uploadResult != null && uploadResult.Data!=null)
                  FileSteps.DeleteFile(credentional, uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void ListFilesInIncorrectFolderTest()
        {

            var testFolderFileList = FileSteps.GetFileList(credentional, "incorrect Id");
            Assert.IsFalse(testFolderFileList.IsSucceed);
        }

    }
}
