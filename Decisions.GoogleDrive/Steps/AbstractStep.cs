using Decisions.OAuth;
using DecisionsFramework;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.Mapping.InputImpl;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer.Services.ContextData;
using DecisionsFramework.ServiceLayer.Services.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{


    [Writable]
    public abstract class AbstractStep : ISyncStep, IDataConsumer, IDataProducer
    {
        public const string GoogleDriveCategory = "Integration/Google Drive";
        
        protected const string ERROR_OUTCOME = "Error";
        protected const string RESULT_OUTCOME = "Result";
        protected const string DONE_OUTCOME = "Done";
        protected const string ERROR_OUTCOME_DATA_NAME = "Error info";
        protected const string RESULT = "RESULT";
        

        protected const string CREDENTINAL_DATA = "Credentinal";
        protected const string SERVICE_ACCOUNT_CREDENTINAL_DATA = "Service Account Credentinal";
        protected const string FILE_OR_FOLDER_ID = "File Or Folder Id";
        protected const string FILE_ID = "File Id";
        protected const string PARENT_FOLDER_ID = "Parent Folder Id";
        protected const string LOCAL_FILE_PATH = "Local File Path";
        protected const string PERMISSION = "Permission";
        protected const string NEW_FOLDER_NAME = "New Folder Name";
        
        /*OAuth2TokenResponse resp;
        OAuthToken token*/

        [PropertyHidden]
        public virtual DataDescription[] InputData
        {
            get
            {
                return new DataDescription[] {
                  new DataDescription(typeof(GoogleDriveCredential), CREDENTINAL_DATA),
                  new DataDescription(typeof(GoogleDriveServiceAccountCredential), SERVICE_ACCOUNT_CREDENTINAL_DATA)
                };
            }
        }

        protected const int ERROR_OUTCOME_INDEX = 1;
        protected const int RESULT_OUTCOME_INDEX = 0;
        public virtual OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] {null, new OutcomeScenarioData(ERROR_OUTCOME, new DataDescription(typeof(GoogleDriveErrorInfo), ERROR_OUTCOME_DATA_NAME)) };
            }
        }

        private Connection CreateConnection(StepStartData data)
        {
            var credentinal = (GoogleDriveCredential)data.Data[CREDENTINAL_DATA];
            var serviceAccountCredentinal = (GoogleDriveServiceAccountCredential)data.Data[SERVICE_ACCOUNT_CREDENTINAL_DATA];

            if (credentinal!=null)
                return Connection.Create(credentinal);
            else if(serviceAccountCredentinal!=null)
                return Connection.Create(serviceAccountCredentinal);
            
            throw new BusinessRuleException($"Step needs either {CREDENTINAL_DATA} or {SERVICE_ACCOUNT_CREDENTINAL_DATA}");
        }

        public ResultData Run(StepStartData data)
        {
            try
            {
                Connection connection = CreateConnection(data);
                GoogleDriveBaseResult res = ExecuteStep(connection, data);

                if (res.IsSucceed)
                {
                    var outputData = OutcomeScenarios[RESULT_OUTCOME_INDEX].OutputData;
                    var exitPointName = OutcomeScenarios[RESULT_OUTCOME_INDEX].ExitPointName;

                    if (outputData != null && outputData.Length > 0)
                        return new ResultData(exitPointName, new DataPair[] { new DataPair(outputData[0].Name, res.DataObj) });
                    else
                        return new ResultData(exitPointName);
                }
                else
                {
                    return new ResultData(ERROR_OUTCOME, new DataPair[] { new DataPair(ERROR_OUTCOME_DATA_NAME, res) });
                }
            }
            catch (Exception ex)
            {
                GoogleDriveErrorInfo ErrInfo = new GoogleDriveErrorInfo() { ErrorMessage = ex.ToString(), HttpErrorCode = null};
                return new ResultData(ERROR_OUTCOME, new DataPair[] { new DataPair(ERROR_OUTCOME_DATA_NAME, ErrInfo) });
                //throw new LoggedException("Error running step", ex);
            }
        }

        protected  abstract GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data);

    }
}
