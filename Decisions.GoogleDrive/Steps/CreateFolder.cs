using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    [AutoRegisterStep("Create Folder", GoogleDriveCategory)]
    [Writable]
    public class CreateFolder : AbstractStep
    {

        protected override OutcomeScenarioData CorrectOutcomeScenario
        {
            get
            {
                return new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveFolder), RESULT));
            }
        }

        public CreateFolder()
        {
            InputDataList.Add(new DataDescription(typeof(string), PARENT_FOLDER_ID));
            InputDataList.Add(new DataDescription(typeof(string), NEW_FOLDER_NAME));
        }

        protected override GoogleDriveBaseResult ExecuteStep(StepStartData data)
        {
            var credentinal = (GoogleDriveCredential)data.Data[CREDENTINAL_DATA];
            var folderId = (string)data.Data[PARENT_FOLDER_ID];
            var newFolderNmae = (string)data.Data[NEW_FOLDER_NAME];

            return StepsCore.CreateFolder(credentinal, folderId, newFolderNmae);
        }
    }
}
