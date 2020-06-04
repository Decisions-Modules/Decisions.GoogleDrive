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
                return Concat(base.InputData, new DataDescription(typeof(string), FILE_ID), new DataDescription(typeof(string), LOCAL_FILE_PATH));
            }
        }
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return Concat(base.OutcomeScenarios, new OutcomeScenarioData(DONE_OUTCOME));
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
