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
            testFolder = FolderSteps.CreateFolder(credentional, null, TestData.TestFolderName);
        }

        [TestCleanupAttribute]
        public void CleanupTests()
        {
            FolderSteps.DeleteFolder(credentional, testFolder.Id);
        }

        [TestMethod]
        public void ListFoldersTest()
        {
            var rootFileList = FolderSteps.GetFolderList(credentional, null);
            var testFolderFileList = FolderSteps.GetFolderList(credentional, testFolder.Id);

            Assert.IsTrue(rootFileList.Length > 0 || testFolderFileList.Length > 0);
        }

        [TestMethod]
        public void CreateFolderTest()
        {
            GoogleDriveFolder createdFolder = null;
            try
            {
                createdFolder = FolderSteps.CreateFolder(credentional, testFolder.Id, TestData.TestFolderName);
                Assert.IsNotNull(createdFolder);
            }
            finally
            {
                if (createdFolder != null)
                    FolderSteps.DeleteFolder(credentional, createdFolder.Id);
            }

        }

        [TestMethod]
        public void DeleteFolderTest()
        {

            GoogleDriveFolder createdFolder = null;
            try
            {
                createdFolder = FolderSteps.CreateFolder(credentional, testFolder.Id, TestData.TestFolderName);
                Assert.IsNotNull(createdFolder);

                var folderListBefore = FolderSteps.GetFolderList(credentional, testFolder.Id);
                FolderSteps.DeleteFolder(credentional, createdFolder.Id);
                var folderListAfter = FolderSteps.GetFolderList(credentional, testFolder.Id);

                Assert.AreEqual(1, folderListBefore.Length - folderListAfter.Length);
            }
            finally
            {
                try
                {
                    if (createdFolder != null)
                        FolderSteps.DeleteFolder(credentional, createdFolder.Id);
                }
                catch { }
            }
        }


        [TestMethod]
        public void GetPermsTest()
        {
            var perms = FolderSteps.GetFolderPermissions(credentional, testFolder.Id);
            Assert.IsTrue(perms.Any(x => x.Role == GoogleDriveRole.owner && x.Type == GoogleDrivePermType.user));
        }

        [TestMethod]
        public void SetPermsTest()
        {
            var perm = FolderSteps.SetFolderPermissions(credentional, testFolder.Id, new GoogleDrivePermission(null, TestData.TestEmail, GoogleDrivePermType.user, GoogleDriveRole.writer));
            Assert.IsTrue(perm.Id != "");
        }


    }
}
