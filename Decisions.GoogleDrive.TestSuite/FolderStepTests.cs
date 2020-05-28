using Decisions.GoogleDrive;
using DriveLibrary.Steps;
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
            testFolder = FolderSteps.CreateFolder(credentional, null, TestData.TestFolderName).Data;
        }

        [TestCleanupAttribute]
        public void CleanupTests()
        {
            FolderSteps.DeleteFolder(credentional, testFolder.Id);
        }

        [TestMethod]
        public void DoesFolderExistTest()
        {
            var shouldBeFalse = FolderSteps.DoesFolderExist(credentional, "incorrect Id");
            Assert.IsTrue(shouldBeFalse.IsSucceed);
            Assert.IsFalse(shouldBeFalse.Data);

            var shouldBeTrue = FolderSteps.DoesFolderExist(credentional, testFolder.Id);
            Assert.IsTrue(shouldBeTrue.IsSucceed);
            Assert.IsTrue(shouldBeTrue.Data);
        }

        [TestMethod]
        public void ListFoldersTest()
        {
            var rootFileList = FolderSteps.GetFolderList(credentional, null);
            Assert.IsTrue(rootFileList.IsSucceed);

            var testFolderFileList = FolderSteps.GetFolderList(credentional, testFolder.Id);
            Assert.IsTrue(rootFileList.IsSucceed);

        }

        [TestMethod]
        public void CreateFolderTest()
        {
            GoogleDriveResultWithData<GoogleDriveFolder> createdFolder = null;
            try
            {
                createdFolder = FolderSteps.CreateFolder(credentional, testFolder.Id, TestData.TestFolderName);
                Assert.IsTrue(createdFolder.IsSucceed);
                Assert.IsNotNull(createdFolder.Data);
            }
            finally
            {
                if (createdFolder != null)
                    FolderSteps.DeleteFolder(credentional, createdFolder.Data.Id);
            }

        }

        [TestMethod]
        public void DeleteFolderTest()
        {

            GoogleDriveResultWithData<GoogleDriveFolder> createdFolder = null;
            try
            {
                createdFolder = FolderSteps.CreateFolder(credentional, testFolder.Id, TestData.TestFolderName);
                Assert.IsTrue(createdFolder.IsSucceed);

                var folderListBefore = FolderSteps.GetFolderList(credentional, testFolder.Id);
                Assert.IsTrue(folderListBefore.IsSucceed);

                FolderSteps.DeleteFolder(credentional, createdFolder.Data.Id);

                var folderListAfter = FolderSteps.GetFolderList(credentional, testFolder.Id);
                Assert.IsTrue(folderListAfter.IsSucceed);

                Assert.AreEqual(1, folderListBefore.Data.Length - folderListAfter.Data.Length);
            }
            finally
            {
                try
                {
                    if (createdFolder != null)
                        FolderSteps.DeleteFolder(credentional, createdFolder.Data.Id);
                }
                catch { }
            }
        }


        [TestMethod]
        public void GetPermsTest()
        {
            var perms = FolderSteps.GetFolderPermissions(credentional, testFolder.Id);
            Assert.IsTrue(perms.IsSucceed);
            Assert.IsTrue(perms.Data.Any(x => x.Role == GoogleDriveRole.owner && x.Type == GoogleDrivePermType.user));
        }

        [TestMethod]
        public void SetPermsTest()
        {
            var perm = FolderSteps.SetFolderPermissions(credentional, testFolder.Id, new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer));
            Assert.IsTrue(perm.IsSucceed);
            Assert.IsTrue(perm.Data.Id != "");
        }


    }
}
