using Decisions.GoogleDrive;
using Decisions.GoogleDriveTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDriveTests
{
    [TestClass]
    public class FileTests
    {
        string TestFileFullName { get { return TestData.LocalTestDir + TestData.TestFileName; } }

        private GoogleDriveFolder testFolder;

        private Connection GetConnection()
        {
            return TestData.GetConnection();
        }

        [TestInitialize]
        public void InitTests()
        {
            const int LINE_COUNT = 3;
            var stream = new System.IO.StreamWriter(TestFileFullName);
            for (int i = 0; i < LINE_COUNT; i++)
                stream.Write($"{i}qwertyuiop\n");
            stream.Close();

            testFolder = GoogleDriveUtility.CreateFolder(GetConnection(), TestData.TestFolderName, null).Data;
        }

        [TestCleanupAttribute]
        public void CleanupTests()
        {
            File.Delete(TestFileFullName);
            GoogleDriveUtility.DeleteResource(GetConnection(), testFolder.Id);
        }

        [TestMethod]
        public void DoesResourceExistTest()
        {
            var ShoulBeUnavailable = GoogleDriveUtility.DoesResourceExist(GetConnection(), "incorrect Id");
            Assert.AreEqual(GoogleDriveResourceType.Unavailable, ShoulBeUnavailable.Data);
            
            var ShoulBeFolder = GoogleDriveUtility.DoesResourceExist(GetConnection(), testFolder.Id);
            Assert.AreEqual(GoogleDriveResourceType.Folder, ShoulBeFolder.Data);

            var uploadResult = GoogleDriveUtility.UploadFile(GetConnection(), TestFileFullName, null, testFolder.Id);
            try
            {
                var ShoulBeFile = GoogleDriveUtility.DoesResourceExist(GetConnection(), uploadResult.Data.Id);
                Assert.AreEqual(GoogleDriveResourceType.File, ShoulBeFile.Data);
            }
            finally
            {
                GoogleDriveUtility.DeleteResource(GetConnection(), uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void GetFileListTest()
        {
                var rootFileList = GoogleDriveUtility.GetFiles(GetConnection(), null);
                Assert.IsTrue(rootFileList.IsSucceed);
        }
        
        [TestMethod]
        public void GetLongFileListTest()
        {
            const int FILE_COUNT = 110;
            List<GoogleDriveFile> uploadedFiles = new List<GoogleDriveFile>(FILE_COUNT);

            try
            {
                var rootFileList = GoogleDriveUtility.GetFiles(GetConnection(), null);
                Assert.IsTrue(rootFileList.IsSucceed);

                for (int i = 0; i < FILE_COUNT; i++)
                    uploadedFiles.Add(GoogleDriveUtility.UploadFile(GetConnection(), TestFileFullName, null, testFolder.Id).Data);

                var testFolderFileList = GoogleDriveUtility.GetFiles(GetConnection(), testFolder.Id);
                Assert.IsTrue(testFolderFileList.IsSucceed);
                Assert.AreEqual(FILE_COUNT, testFolderFileList.Data.Length);
            }
            finally
            {
                uploadedFiles.ForEach(file => GoogleDriveUtility.DeleteResource(GetConnection(), file.Id));
            }
        }

        [TestMethod]
        public void DeleteFileTest()
        {
            var uploadResult = GoogleDriveUtility.UploadFile(GetConnection(), TestFileFullName, null, testFolder.Id);
            try
            {
                var fileListBefore = GoogleDriveUtility.GetFiles(GetConnection(), testFolder.Id);
                Assert.IsTrue(fileListBefore.IsSucceed);

                var delResult=GoogleDriveUtility.DeleteResource(GetConnection(), uploadResult.Data.Id);
                Assert.IsTrue(delResult.IsSucceed);

                var fileListAfter = GoogleDriveUtility.GetFiles(GetConnection(), testFolder.Id);
                Assert.IsTrue(fileListAfter.IsSucceed);

                Assert.AreEqual(1, fileListBefore.Data.Length - fileListAfter.Data.Length);
            }
            finally
            {
                try
                {
                    GoogleDriveUtility.DeleteResource(GetConnection(), uploadResult.Data.Id);
                }
                catch { }
            }
        }

        [TestMethod]
        public void GetPermsTest()
        {
            var uploadResult = GoogleDriveUtility.UploadFile(GetConnection(), TestFileFullName, null, testFolder.Id);
            try
            {
                var permissionResult = GoogleDriveUtility.GetResourcePermissions(GetConnection(), uploadResult.Data.Id);
                Assert.IsTrue(permissionResult.IsSucceed);
                Assert.IsTrue(permissionResult.Data.Length > 0);
            }
            finally
            {
                GoogleDriveUtility.DeleteResource(GetConnection(), uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void SetPermsTest()
        {
            var uploadResult = GoogleDriveUtility.UploadFile(GetConnection(), TestFileFullName, null, testFolder.Id);
            try
            {
                var newPermission = new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer);
                var permission = GoogleDriveUtility.SetResourcePermissions(GetConnection(), uploadResult.Data.Id, newPermission);
                Assert.IsTrue(permission.IsSucceed);

                var perms = GoogleDriveUtility.GetResourcePermissions(GetConnection(), uploadResult.Data.Id);
                var res = Enumerable.Any(perms.Data, (it) => { return it.Id == permission.Data.Id; });
                Assert.IsTrue(res);

            }
            finally
            {
                GoogleDriveUtility.DeleteResource(GetConnection(), uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void SetIncorrectPermsTest()
        {
            var newPermission = new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer);
            var permissionResult = GoogleDriveUtility.SetResourcePermissions(GetConnection(), "incorrect Id", newPermission);
            Assert.IsFalse( permissionResult.IsSucceed);

        }

        [TestMethod]
        public void DownloadFileTest()
        {
            var uploadResult = GoogleDriveUtility.UploadFile(GetConnection(), TestFileFullName, null, testFolder.Id);
            string localFileName = TestFileFullName + "_";
            try
            {
                var downloadResult = GoogleDriveUtility.DownloadFile(GetConnection(), uploadResult.Data.Id, localFileName);
                Assert.IsTrue(downloadResult.IsSucceed);
                Assert.IsTrue(File.Exists(localFileName));

                var origLen = new System.IO.FileInfo(TestFileFullName).Length;
                var actualLen = new System.IO.FileInfo(localFileName).Length;

                Assert.AreEqual(origLen, actualLen);

            }
            finally
            {
                GoogleDriveUtility.DeleteResource(GetConnection(), uploadResult.Data.Id);
                System.IO.File.Delete(localFileName);
            }
        }

        [TestMethod]
        public void DownloadIncorrectFileTest()
        {
            string localFileName = TestFileFullName + "_";
            try
            {
                var downloadResult = GoogleDriveUtility.DownloadFile(GetConnection(), "incorrect id", localFileName);
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
                uploadResult = GoogleDriveUtility.UploadFile(GetConnection(), TestFileFullName, null, testFolder.Id);
                Assert.IsTrue(uploadResult.IsSucceed);
                Assert.IsNotNull(uploadResult.Data);
            }
            finally
            {
                if(uploadResult != null && uploadResult.Data!=null)
                  GoogleDriveUtility.DeleteResource(GetConnection(), uploadResult.Data.Id);
            }
        }

        [TestMethod]
        public void ListFilesInIncorrectFolderTest()
        {

            var testFolderFileList = GoogleDriveUtility.GetFiles(GetConnection(), "incorrect Id");
            Assert.IsFalse(testFolderFileList.IsSucceed);
        }

    }
}
