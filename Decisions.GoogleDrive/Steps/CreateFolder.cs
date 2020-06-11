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
                var data = new DataDescription[] {  new DataDescription(typeof(string), PARENT_FOLDER_ID), new DataDescription(typeof(string), NEW_FOLDER_NAME) };
                return base.InputData.Concat(data).ToArray();
            }
        }
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveFolder), RESULT)) };
                return base.OutcomeScenarios.Concat(data).ToArray();
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
