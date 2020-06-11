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
    [AutoRegisterStep("Does Resource Exist", GoogleDriveCategory)]
    [Writable]
    public class DoesResourceExist:AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), FILE_OR_FOLDER_ID) };
                return base.InputData.Concat(data).ToArray();
            }
        }
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveResourceType), RESULT)) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data)
        {
            var fileOrFolderId = (string) data.Data[FILE_OR_FOLDER_ID];

            return GoogleDriveUtility.DoesResourceExist(connection, fileOrFolderId);
        }
    }
}
