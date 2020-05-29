using Decisions.GoogleDrive;
using Decisions.GoogleDriveTests;
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

            testFolder = StepsCore.CreateFolder(credentional, null, TestData.TestFolderName).Data;

        }

        [TestCleanupAttribute]
        public void CleanupTests()
        {
            File.Delete(TestFileFullName);
            StepsCore.DeleteResource(credentional, testFolder.Id);
        }

        [TestMethod]
        public void DoesResourceExistTest()
        {
            var ShoulBeUnavailable = StepsCore.DoesResourceExist(credentional, "incorrect Id");
            Assert.AreEqual(GoogleDriveResourceType.Unavailable, ShoulBeUnavailable.Data);
            
            var ShoulBeFolder = StepsCore.DoesResourceExist(credentional, testFolder.Id);
            Assert.AreEqual(GoogleDriveResourceType.Folder, ShoulBeFolder.Data);

            var uploadResult = StepsCore.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var ShoulBeFile = StepsCore.DoesResourceExist(credentional, uploadResult.Data.Id);
                Assert.AreEqual(GoogleDriveResourceType.File, ShoulBeFile.Data);
            }
            finally
            {
                StepsCore.DeleteResource(credentional, uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void ListFilesTest()
        {
                var rootFileList = StepsCore.GetFileList(credentional, null);
                Assert.IsTrue(rootFileList.IsSucceed);
        }
        
        [TestMethod]
        public void ListFilesInFolderTest()
        {
            const int FILE_COUNT = 110;
            List<GoogleDriveFile> uploadedFiles = new List<GoogleDriveFile>(FILE_COUNT);

            try
            {
                var rootFileList = StepsCore.GetFileList(credentional, null);
                Assert.IsTrue(rootFileList.IsSucceed);

                for (int i = 0; i < FILE_COUNT; i++)
                    uploadedFiles.Add(StepsCore.UploadFile(credentional, testFolder.Id, TestFileFullName).Data);

                var testFolderFileList = StepsCore.GetFileList(credentional, testFolder.Id);
                Assert.IsTrue(testFolderFileList.IsSucceed);
                Assert.AreEqual(FILE_COUNT, testFolderFileList.Data.Length);
            }
            finally
            {
                uploadedFiles.ForEach(file => StepsCore.DeleteResource(credentional, file.Id));
            }
        }

        [TestMethod]
        public void DeleteFileTest()
        {
            var uploadResult = StepsCore.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var fileListBefore = StepsCore.GetFileList(credentional, testFolder.Id);
                Assert.IsTrue(fileListBefore.IsSucceed);

                var delResult=StepsCore.DeleteResource(credentional, uploadResult.Data.Id);
                Assert.IsTrue(delResult.IsSucceed);

                var fileListAfter = StepsCore.GetFileList(credentional, testFolder.Id);
                Assert.IsTrue(fileListAfter.IsSucceed);

                Assert.AreEqual(1, fileListBefore.Data.Length - fileListAfter.Data.Length);
            }
            finally
            {
                try
                {
                    StepsCore.DeleteResource(credentional, uploadResult.Data.Id);
                }
                catch { }
            }
        }

        [TestMethod]
        public void GetPermsTest()
        {
            var uploadResult = StepsCore.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var permissionResult = StepsCore.GetResourcePermissions(credentional, uploadResult.Data.Id);
                Assert.IsTrue(permissionResult.IsSucceed);
                Assert.IsTrue(permissionResult.Data.Length > 0);
            }
            finally
            {
                StepsCore.DeleteResource(credentional, uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void SetPermsTest()
        {
            var uploadResult = StepsCore.UploadFile(credentional, testFolder.Id, TestFileFullName);
            try
            {
                var newPermission = new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer);
                var permission = StepsCore.SetResourcePermissions(credentional, uploadResult.Data.Id, newPermission);
                Assert.IsTrue(permission.IsSucceed);

                var perms = StepsCore.GetResourcePermissions(credentional, uploadResult.Data.Id);
                var res = Enumerable.Any(perms.Data, (it) => { return it.Id == permission.Data.Id; });
                Assert.IsTrue(res);

            }
            finally
            {
                StepsCore.DeleteResource(credentional, uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void SetIncorrectPermsTest()
        {
            var newPermission = new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer);
            var permissionResult = StepsCore.SetResourcePermissions(credentional, "incorrect Id", newPermission);
            Assert.IsFalse( permissionResult.IsSucceed);

        }

        [TestMethod]
        public void DownloadFileTest()
        {
            var uploadResult = StepsCore.UploadFile(credentional, testFolder.Id, TestFileFullName);
            string localFileName = TestFileFullName + "_";
            try
            {
                var downloadResult = StepsCore.DownloadFile(credentional, uploadResult.Data.Id, localFileName);
                Assert.IsTrue(downloadResult.IsSucceed);
                Assert.IsTrue(File.Exists(localFileName));

                var origLen = new System.IO.FileInfo(TestFileFullName).Length;
                var actualLen = new System.IO.FileInfo(localFileName).Length;

                Assert.AreEqual(origLen, actualLen);

            }
            finally
            {
                StepsCore.DeleteResource(credentional, uploadResult.Data.Id);
                System.IO.File.Delete(localFileName);
            }
        }

        [TestMethod]
        public void DownloadIncorrectFileTest()
        {
            string localFileName = TestFileFullName + "_";
            try
            {
                var downloadResult = StepsCore.DownloadFile(credentional, "incorrect id", localFileName);
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
                uploadResult = StepsCore.UploadFile(credentional, testFolder.Id, TestFileFullName);
                Assert.IsTrue(uploadResult.IsSucceed);
                Assert.IsNotNull(uploadResult.Data);
            }
            finally
            {
                if(uploadResult != null && uploadResult.Data!=null)
                  StepsCore.DeleteResource(credentional, uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void ListFilesInIncorrectFolderTest()
        {

            var testFolderFileList = StepsCore.GetFileList(credentional, "incorrect Id");
            Assert.IsFalse(testFolderFileList.IsSucceed);
        }

    }
}
