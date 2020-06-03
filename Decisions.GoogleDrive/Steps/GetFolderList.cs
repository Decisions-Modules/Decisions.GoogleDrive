using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    [AutoRegisterStep("Get Folder List", GoogleDriveCategory)]
    [Writable]
    public class GetFolderList : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                return Concat(base.InputData, new DataDescription(typeof(string), PARENT_FOLDER_ID));
            }
        }

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return Concat(base.OutcomeScenarios, new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveFolder[]), RESULT)) );
            }
        }

        protected override GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data)
        {
            var FolderId = (string)data.Data[PARENT_FOLDER_ID];

            return GoogleDriveUtility.GetFolders(connection, FolderId); //StepsCore.GetFolderList(credentinal, FolderId);
        }
    }
}
