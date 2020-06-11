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
    [AutoRegisterStep("Upload File", GoogleDriveCategory)]
    [Writable]
    public class UploadFile : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), PARENT_FOLDER_ID), new DataDescription(typeof(string), LOCAL_FILE_PATH) };
                return base.InputData.Concat(data).ToArray();
            }
        }
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveFile), RESULT)) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data)
        {
            var folderId = (string)data.Data[PARENT_FOLDER_ID];
            var localFilePath = (string)data.Data[LOCAL_FILE_PATH];
            var fileName = System.IO.Path.GetFileName(localFilePath);

            return GoogleDriveUtility.UploadFile(connection, localFilePath, fileName, folderId);
        }
    }
}
