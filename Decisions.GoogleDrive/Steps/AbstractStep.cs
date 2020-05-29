using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.Mapping.InputImpl;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer.Services.ContextData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{

    //[AutoRegisterStep("Step Name", "Step Category")]
    [Writable]
    public abstract class AbstractStep : ISyncStep, IDataConsumer, IDataProducer//, IDefaultInputMappingStep
    {
        public const string GoogleDriveCategory = "Integration/Google Drive";
        
        protected const string ERROR_OUTCOME = "Error";
        protected const string RESULT_OUTCOME = "Result";
        protected const string DONE_OUTCOME = "Done";
        protected const string ERROR_OUTCOME_DATA_NAME = "Error info";
        protected const string RESULT = "RESULT";
        

        protected const string CREDENTINAL_DATA = "Credentinal";
        protected const string FILE_OR_FOLDER_ID = "File Or Folder Id";
        protected const string FILE_ID = "File Id";
        protected const string PARENT_FOLDER_ID = "Parent Folder Id";
        protected const string LOCAL_FILE_PATH = "Local File Path";
        protected const string PERMISSION = "Permission";
        protected const string NEW_FOLDER_NAME = "New Folder Name";

        public AbstractStep() 
        {
            InputDataList = new List<DataDescription>() { new DataDescription(typeof(GoogleDriveCredential), CREDENTINAL_DATA) };
        }

        protected List<DataDescription> InputDataList;
        [PropertyHidden]
        public DataDescription[] InputData
        {
            get
            {
                return InputDataList.ToArray();
            }
        }

        protected abstract OutcomeScenarioData CorrectOutcomeScenario { get; }
        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[]
                {
                       CorrectOutcomeScenario,
                       new OutcomeScenarioData(ERROR_OUTCOME, new DataDescription(typeof(GoogleDriveErrorInfo), ERROR_OUTCOME_DATA_NAME))
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            GoogleDriveBaseResult res = ExecuteStep(data);

            if (res.IsSucceed)
            {
                var outputData = CorrectOutcomeScenario.OutputData;
                var ExitPointName=CorrectOutcomeScenario.ExitPointName;

                if (outputData!=null && outputData.Length > 0)
                    return new ResultData(ExitPointName, new DataPair[] { new DataPair(outputData[0].Name, res.DataObj) });
                else
                    return new ResultData(ExitPointName);
            }
            else
                return new ResultData(ERROR_OUTCOME, new DataPair[] { new DataPair(ERROR_OUTCOME_DATA_NAME, res.DataObj) });
        }

        protected abstract GoogleDriveBaseResult ExecuteStep(StepStartData data);

    }
}
