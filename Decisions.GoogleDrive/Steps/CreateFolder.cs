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
    [AutoRegisterStep("Create Folder", GoogleDriveCategory)]
    [Writable]
    public class CreateFolder : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                return Concat(base.InputData,
                    new DataDescription(typeof(string), PARENT_FOLDER_ID),
                    new DataDescription(typeof(string), NEW_FOLDER_NAME));
            }
        }
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return Concat(base.OutcomeScenarios, new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveFolder), RESULT)) );
            }
        }

        protected override GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data)
        {
            var parentFolderId = (string)data.Data[PARENT_FOLDER_ID];
            var newFolderName = (string)data.Data[NEW_FOLDER_NAME];

            return GoogleDriveUtility.CreateFolder(connection, newFolderName, parentFolderId);
        }
    }
}
