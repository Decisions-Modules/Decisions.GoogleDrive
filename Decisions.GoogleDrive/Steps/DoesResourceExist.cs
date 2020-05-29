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
    [AutoRegisterStep("Does Resource Exist", GoogleDriveCategory)]
    [Writable]
    public class DoesResourceExist:AbstractStep
    {

        protected override OutcomeScenarioData CorrectOutcomeScenario 
        {
            get
            {
                return new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveResourceType), RESULT) );
            }
        }

        public DoesResourceExist() 
        {
            InputDataList.Add(new DataDescription(typeof(string), FILE_OR_FOLDER_ID));
        }

        protected override GoogleDriveBaseResult ExecuteStep(StepStartData data)
        {
            var credentinal = (GoogleDriveCredential) data.Data[CREDENTINAL_DATA];
            var fileOrFolderId = (string) data.Data[FILE_OR_FOLDER_ID];

            return StepsCore.DoesResourceExist(credentinal, fileOrFolderId);
        }
    }
}
