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
    public class FolderStepTests
    {
        private readonly GoogleDriveCredential credentional = TestData.GetCredential();
        private GoogleDriveFolder testFolder;

        [TestInitialize]
        public void InitTests()
        {
            testFolder = StepsCore.CreateFolder(credentional, null, TestData.TestFolderName).Data;
        }

        [TestCleanupAttribute]
        public void CleanupTests()
        {
            StepsCore.DeleteResource(credentional, testFolder.Id);
        }

        [TestMethod]
        public void ListFoldersTest()
        {
            var rootFileList = StepsCore.GetFolderList(credentional, null);
            Assert.IsTrue(rootFileList.IsSucceed);

            var testFolderFileList = StepsCore.GetFolderList(credentional, testFolder.Id);
            Assert.IsTrue(rootFileList.IsSucceed);

        }

        [TestMethod]
        public void CreateFolderTest()
        {
            GoogleDriveResultWithData<GoogleDriveFolder> createdFolder = null;
            try
            {
                createdFolder = StepsCore.CreateFolder(credentional, testFolder.Id, TestData.TestFolderName);
                Assert.IsTrue(createdFolder.IsSucceed);
                Assert.IsNotNull(createdFolder.Data);
            }
            finally
            {
                if (createdFolder != null)
                    StepsCore.DeleteResource(credentional, createdFolder.Data.Id);
            }

        }

        [TestMethod]
        public void DeleteFolderTest()
        {

            GoogleDriveResultWithData<GoogleDriveFolder> createdFolder = null;
            try
            {
                createdFolder = StepsCore.CreateFolder(credentional, testFolder.Id, TestData.TestFolderName);
                Assert.IsTrue(createdFolder.IsSucceed);

                var folderListBefore = StepsCore.GetFolderList(credentional, testFolder.Id);
                Assert.IsTrue(folderListBefore.IsSucceed);

                StepsCore.DeleteResource(credentional, createdFolder.Data.Id);

                var folderListAfter = StepsCore.GetFolderList(credentional, testFolder.Id);
                Assert.IsTrue(folderListAfter.IsSucceed);

                Assert.AreEqual(1, folderListBefore.Data.Length - folderListAfter.Data.Length);
            }
            finally
            {
                try
                {
                    if (createdFolder != null)
                        StepsCore.DeleteResource(credentional, createdFolder.Data.Id);
                }
                catch { }
            }
        }


        [TestMethod]
        public void GetPermsTest()
        {
            var perms = StepsCore.GetResourcePermissions(credentional, testFolder.Id);
            Assert.IsTrue(perms.IsSucceed);
            Assert.IsTrue(perms.Data.Any(x => x.Role == GoogleDriveRole.owner && x.Type == GoogleDrivePermType.user));
        }

        [TestMethod]
        public void SetPermsTest()
        {
            var permission = StepsCore.SetResourcePermissions(credentional, testFolder.Id, new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer));
            Assert.IsTrue(permission.IsSucceed);
            Assert.IsTrue(permission.Data.Id != "");

            var perms = StepsCore.GetResourcePermissions(credentional, testFolder.Id);

            var res = Enumerable.Any(perms.Data, (it) => { return it.Id == permission.Data.Id; });
            Assert.IsTrue(res);
        }


    }
}
