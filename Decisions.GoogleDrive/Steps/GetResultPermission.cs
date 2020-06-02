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
    [AutoRegisterStep("Get Resource Permission", GoogleDriveCategory)]
    [Writable]
    public class GetResultPermission : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var res = new List<DataDescription>(base.InputData);
                res.Add(new DataDescription(typeof(string), FILE_OR_FOLDER_ID));
                return res.ToArray();
            }
        }

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var res = base.OutcomeScenarios;
                res[RESULT_OUTCOME_INDEX] = new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDrivePermission[]), RESULT));
                return res;
            }
        }

        protected override GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data)
        {
            var folderId = (string)data.Data[FILE_OR_FOLDER_ID];
            return GoogleDriveUtility.GetResourcePermissions(connection, folderId);
        }
    }
}
