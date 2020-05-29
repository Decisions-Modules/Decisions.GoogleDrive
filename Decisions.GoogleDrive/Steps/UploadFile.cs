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
    [AutoRegisterStep("Upload File", GoogleDriveCategory)]
    [Writable]
    public class UploadFile : AbstractStep
    {

        protected override OutcomeScenarioData CorrectOutcomeScenario
        {
            get
            {
                return new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveFile), RESULT));
            }
        }

        public UploadFile()
        {
            InputDataList.Add(new DataDescription(typeof(string), PARENT_FOLDER_ID));
            InputDataList.Add(new DataDescription(typeof(string), LOCAL_FILE_PATH));
        }

        protected override GoogleDriveBaseResult ExecuteStep(StepStartData data)
        {
            var credentinal = (GoogleDriveCredential)data.Data[CREDENTINAL_DATA];
            var folderId = (string)data.Data[PARENT_FOLDER_ID];
            var filePath = (string)data.Data[LOCAL_FILE_PATH];

            return StepsCore.UploadFile(credentinal, folderId, filePath);
        }
    }
}
