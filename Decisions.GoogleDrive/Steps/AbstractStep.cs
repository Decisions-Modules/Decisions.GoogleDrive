using Decisions.OAuth;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer.Services.ContextData;
using System;
using System.Collections.Generic;

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
        protected const string FILE_OR_FOLDER_ID = "File Or Folder Id";
        protected const string FILE_ID = "File Id";
        protected const string PARENT_FOLDER_ID = "Parent Folder Id";
        protected const string LOCAL_FILE_PATH = "Local File Path";
        protected const string PERMISSION = "Permission";
        protected const string NEW_FOLDER_NAME = "New Folder Name";

        [PropertyHidden]
        public virtual DataDescription[] InputData
        {
            get
            {
                return new DataDescription[] {
                  new DataDescription(typeof(GoogleDriveCredential), CREDENTINAL_DATA),
                };
            }
        }

        private const int ERROR_OUTCOME_INDEX = 0;
        private const int RESULT_OUTCOME_INDEX = 1;
        public virtual OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] {new OutcomeScenarioData(ERROR_OUTCOME, new DataDescription(typeof(GoogleDriveErrorInfo), ERROR_OUTCOME_DATA_NAME)) };
            }
        }

        private OAuthToken FindToken(string id)
        {
            ORM<OAuthToken> orm = new ORM<OAuthToken>();
            var token = orm.Fetch(id);
            if (token != null)
                return token;
            throw new EntityNotFoundException($"Can not find token with TokenId=\"{id}\"");
        }

        private Connection CreateConnection(StepStartData data)
        {
            var credentinal = (GoogleDriveCredential)data.Data[CREDENTINAL_DATA];

            if (credentinal != null)
            {
                switch(credentinal.CredentinalType)
                {
                    case GoogleDriveCredentialType.ServiceAccount:
                        return Connection.Create(credentinal.ServiceAccount);

                    case GoogleDriveCredentialType.Token:
                        var token = FindToken(credentinal.Token);
                        return Connection.Create(token.TokenData, token.ConsumerKey, token.ConsumerSecret );
                }
            }
            
            throw new BusinessRuleException($"Step needs {CREDENTINAL_DATA}");
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
                    return new ResultData(ERROR_OUTCOME, new DataPair[] { new DataPair(ERROR_OUTCOME_DATA_NAME, res.ErrorInfo) });
                }
            }
            catch (Exception ex)
            {
                GoogleDriveErrorInfo ErrInfo = new GoogleDriveErrorInfo() { ErrorMessage = ex.ToString(), HttpErrorCode = null};
                return new ResultData(ERROR_OUTCOME, new DataPair[] { new DataPair(ERROR_OUTCOME_DATA_NAME, ErrInfo) });
            }
        }

        protected  abstract GoogleDriveBaseResult ExecuteStep(Connection connection, StepStartData data);
    }
}
