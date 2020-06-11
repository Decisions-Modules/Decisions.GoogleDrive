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
    [AutoRegisterStep("Download File", GoogleDriveCategory)]
    [Writable]
    public class DownloadFile : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), FILE_ID), new DataDescription(typeof(string), LOCAL_FILE_PATH) };
                return base.InputData.Concat(data).ToArray();
            }
        }
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(DONE_OUTCOME) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data)
        {
            var fileId = (string)data.Data[FILE_ID];
            var filePath = (string)data.Data[LOCAL_FILE_PATH];

            return GoogleDriveUtility.DownloadFile(connection, fileId, filePath);
        }
    }
}
