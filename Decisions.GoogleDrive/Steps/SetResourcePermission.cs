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
    [AutoRegisterStep("Set Resource Permission", GoogleDriveCategory)]
    [Writable]
    public class SetResourcePermission : AbstractStep
    {
        protected override OutcomeScenarioData CorrectOutcomeScenario
        {
            get
            {
                return new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDrivePermission), RESULT));
            }
        }

        public SetResourcePermission()
        {
            InputDataList.Add(new DataDescription(typeof(string), FILE_OR_FOLDER_ID));
            InputDataList.Add(new DataDescription(typeof(GoogleDrivePermission), PERMISSION));
        }

        protected override GoogleDriveBaseResult ExecuteStep(StepStartData data)
        {
            var credentinal = (GoogleDriveCredential)data.Data[CREDENTINAL_DATA];
            var folderId = (string)data.Data[FILE_OR_FOLDER_ID];
            var permission = (GoogleDrivePermission)data.Data[PERMISSION];

            return StepsCore.SetResourcePermissions(credentinal, folderId, permission);
        }
    }
}
