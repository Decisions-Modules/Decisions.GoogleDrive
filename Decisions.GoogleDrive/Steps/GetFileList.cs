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
    [AutoRegisterStep("Get File List", GoogleDriveCategory)]
    [Writable]
    public class GetFileList : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var res = new List<DataDescription>(base.InputData);
                res.Add(new DataDescription(typeof(string), PARENT_FOLDER_ID));
                return res.ToArray();
            }
        }

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var res = base.OutcomeScenarios;
                res[RESULT_OUTCOME_INDEX] = new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveFile[]), RESULT));
                return res;
            }
        }

        protected override GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data)
        {
            var FolderId = (string)data.Data[PARENT_FOLDER_ID];

            return GoogleDriveUtility.GetFiles(connection, FolderId);
        }
    }
}
