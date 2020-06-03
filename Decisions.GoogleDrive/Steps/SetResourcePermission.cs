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
                return Concat(base.InputData, new DataDescription(typeof(string), FILE_OR_FOLDER_ID), new DataDescription(typeof(GoogleDrivePermission), PERMISSION));
            }
        }
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return Concat(base.OutcomeScenarios, new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDrivePermission), RESULT)) );
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
