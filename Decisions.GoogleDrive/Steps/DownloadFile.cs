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
    [AutoRegisterStep("Download File", GoogleDriveCategory)]
    [Writable]
    public class DownloadFile : AbstractStep
    {

        protected override OutcomeScenarioData CorrectOutcomeScenario
        {
            get
            {
                return new OutcomeScenarioData(DONE_OUTCOME);
            }
        }

        public DownloadFile()
        {
            InputDataList.Add(new DataDescription(typeof(string), FILE_ID));
            InputDataList.Add(new DataDescription(typeof(string), LOCAL_FILE_PATH));
        }

        protected override GoogleDriveBaseResult ExecuteStep(StepStartData data)
        {
            var credentinal = (GoogleDriveCredential)data.Data[CREDENTINAL_DATA];
            var fileId = (string)data.Data[FILE_ID];
            var filePath = (string)data.Data[LOCAL_FILE_PATH];

            return StepsCore.DownloadFile(credentinal, fileId, filePath);
        }
    }
}
