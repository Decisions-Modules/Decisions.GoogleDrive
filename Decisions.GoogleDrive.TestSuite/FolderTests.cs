using Decisions.GoogleDrive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDriveTests
{
    [TestClass]
    public class FolderTests
    {
        private GoogleDriveFolder testFolder;

        private Connection GetConnection()
        {
            return Connection.Create(TestData.GetServiceAccountCredential());
            //return Connection.Create(TestData.GetCredential());
        }

        [TestInitialize]
        public void InitTests()
        {
            testFolder = GoogleDriveUtility.CreateFolder(GetConnection(), TestData.TestFolderName, null).Data;
        }

        [TestCleanupAttribute]
        public void CleanupTests()
        {
            GoogleDriveUtility.DeleteResource(GetConnection(), testFolder.Id);
        }

        [TestMethod]
        public void ListFoldersTest()
        {
            var rootFileList = GoogleDriveUtility.GetFolders(GetConnection(), null);
            Assert.IsTrue(rootFileList.IsSucceed);

            var testFolderFileList = GoogleDriveUtility.GetFolders(GetConnection(), testFolder.Id);
            Assert.IsTrue(rootFileList.IsSucceed);

        }

        [TestMethod]
        public void CreateFolderTest()
        {
            GoogleDriveResultWithData<GoogleDriveFolder> createdFolder = null;
            try
            {
                createdFolder = GoogleDriveUtility.CreateFolder(GetConnection(), TestData.TestFolderName, testFolder.Id);
                Assert.IsTrue(createdFolder.IsSucceed);
                Assert.IsNotNull(createdFolder.Data);
            }
            finally
            {
                if (createdFolder != null)
                    GoogleDriveUtility.DeleteResource(GetConnection(), createdFolder.Data.Id);
            }

        }

        [TestMethod]
        public void DeleteFolderTest()
        {

            GoogleDriveResultWithData<GoogleDriveFolder> createdFolder = null;
            try
            {
                createdFolder = GoogleDriveUtility.CreateFolder(GetConnection(), TestData.TestFolderName, testFolder.Id);
                Assert.IsTrue(createdFolder.IsSucceed);

                var folderListBefore = GoogleDriveUtility.GetFolders(GetConnection(), testFolder.Id);
                Assert.IsTrue(folderListBefore.IsSucceed);

                GoogleDriveUtility.DeleteResource(GetConnection(), createdFolder.Data.Id);

                var folderListAfter = GoogleDriveUtility.GetFolders(GetConnection(), testFolder.Id);
                Assert.IsTrue(folderListAfter.IsSucceed);

                Assert.AreEqual(1, folderListBefore.Data.Length - folderListAfter.Data.Length);
            }
            finally
            {
                try
                {
                    if (createdFolder != null)
                        GoogleDriveUtility.DeleteResource(GetConnection(), createdFolder.Data.Id);
                }
                catch { }
            }
        }


        [TestMethod]
        public void GetPermsTest()
        {
            var perms = GoogleDriveUtility.GetResourcePermissions(GetConnection(), testFolder.Id);
            Assert.IsTrue(perms.IsSucceed);
            Assert.IsTrue(perms.Data.Any(x => x.Role == GoogleDriveRole.owner && x.Type == GoogleDrivePermType.user));
        }

        [TestMethod]
        public void SetPermsTest()
        {
            var permission = GoogleDriveUtility.SetResourcePermissions(GetConnection(), testFolder.Id, new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer));
            Assert.IsTrue(permission.IsSucceed);
            Assert.IsTrue(permission.Data.Id != "");

            var perms = GoogleDriveUtility.GetResourcePermissions(GetConnection(), testFolder.Id);

            var res = Enumerable.Any(perms.Data, (it) => { return it.Id == permission.Data.Id; });
            Assert.IsTrue(res);
        }


    }
}
