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
    [AutoRegisterStep("Set Resource Permission", GoogleDriveCategory)]
    [Writable]
    public class SetResourcePermission : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var res = new List<DataDescription>(base.InputData);
                res.Add(new DataDescription(typeof(string), FILE_OR_FOLDER_ID));
                res.Add(new DataDescription(typeof(GoogleDrivePermission), PERMISSION));
                return res.ToArray();
            }
        }
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var res = base.OutcomeScenarios;
                res[RESULT_OUTCOME_INDEX] = new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDrivePermission), RESULT));
                return res;
            }
        }

        protected override GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data)
        {
            var folderId = (string)data.Data[FILE_OR_FOLDER_ID];
            var permission = (GoogleDrivePermission)data.Data[PERMISSION];

            return GoogleDriveUtility.SetResourcePermissions(connection, folderId, permission); 
        }
    }
}
