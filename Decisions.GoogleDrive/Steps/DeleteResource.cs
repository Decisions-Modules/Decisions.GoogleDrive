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
    [AutoRegisterStep("Delete Resource", GoogleDriveCategory)]
    [Writable]
    public class DeleteResource:AbstractStep
    {

        protected override OutcomeScenarioData CorrectOutcomeScenario
        {
            get
            {
                return new OutcomeScenarioData(DONE_OUTCOME);
            }
        }

        public DeleteResource()
        {
            InputDataList.Add(new DataDescription(typeof(string), FILE_OR_FOLDER_ID));
        }

        protected override GoogleDriveBaseResult ExecuteStep(StepStartData data)
        {
            var credentinal = (GoogleDriveCredential)data.Data[CREDENTINAL_DATA];
            var FolderId = (string)data.Data[FILE_OR_FOLDER_ID];

            return StepsCore.DeleteResource(credentinal, FolderId);
        }
    }
}
