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
        private readonly DriveCredential credentional = TestData.GetCredential();
        private DriveFolder testFolder;

        [TestInitialize]
        public void InitTests()
        {
            testFolder = FolderSteps.CreateFolder(credentional, null, TestData.TestFolderName);
        }

        [TestCleanupAttribute]
        public void CleanupTests()
        {
            FolderSteps.DeleteFolder(credentional, testFolder);
        }

        [TestMethod]
        public void ListFoldersTest()
        {
            var rootFileList = FolderSteps.GetFolderList(credentional, null);
            var testFolderFileList = FolderSteps.GetFolderList(credentional, testFolder);

            Assert.IsTrue(rootFileList.Length > 0 || testFolderFileList.Length > 0);
        }

        [TestMethod]
        public void CreateFolderTest()
        {
            DriveFolder createdFolder = null;
            try
            {
                createdFolder = FolderSteps.CreateFolder(credentional, testFolder, TestData.TestFolderName);
                Assert.IsNotNull(createdFolder);
            }
            finally
            {
                if (createdFolder != null)
                    FolderSteps.DeleteFolder(credentional, createdFolder);
            }

        }

        [TestMethod]
        public void DeleteFolderTest()
        {

            DriveFolder createdFolder = null;
            try
            {
                createdFolder = FolderSteps.CreateFolder(credentional, testFolder, TestData.TestFolderName);
                Assert.IsNotNull(createdFolder);

                var folderListBefore = FolderSteps.GetFolderList(credentional, testFolder);
                FolderSteps.DeleteFolder(credentional, createdFolder);
                var folderListAfter = FolderSteps.GetFolderList(credentional, testFolder);

                Assert.AreEqual(1, folderListBefore.Length - folderListAfter.Length);
            }
            finally
            {
                try
                {
                    if (createdFolder != null)
                        FolderSteps.DeleteFolder(credentional, createdFolder);
                }
                catch { }
            }
        }


        [TestMethod]
        public void GetPermsTest()
        {
            var perms = FolderSteps.GetFolderPermissions(credentional, testFolder);
            Assert.IsTrue(perms.Any(x => x.Role == DriveRole.owner && x.Type == DrivePermType.user));
        }

        [TestMethod]
        public void SetPermsTest()
        {
            var perm = FolderSteps.SetFolderPermissions(credentional, testFolder, new DrivePermission(null, TestData.TestEmail, DrivePermType.user, DriveRole.writer));
            Assert.IsTrue(perm.Id != "");
        }


    }
}
