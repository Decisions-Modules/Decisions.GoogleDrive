﻿using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.GoogleDrive
{
    [AutoRegisterStep("Get Folder List", GoogleDriveCategory)]
    [Writable]
    public class GetFolderList : AbstractStep
    {

        protected override OutcomeScenarioData CorrectOutcomeScenario
        {
            get
            {
                return new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(GoogleDriveFolder[]), RESULT));
            }
        }
        /*protected override DataDescription ResultDataDescription
        {
            get
            {
                return new DataDescription(typeof(GoogleDriveFolder[]), RESULT);
            }
        }*/

        public GetFolderList()
        {
            InputDataList.Add(new DataDescription(typeof(string), PARENT_FOLDER_ID));
        }

        protected override GoogleDriveBaseResult ExecuteStep(StepStartData data)
        {
            var credentinal = (GoogleDriveCredential)data.Data[CREDENTINAL_DATA];
            var FolderId = (string)data.Data[PARENT_FOLDER_ID];

            return StepsCore.GetFolderList(credentinal, FolderId);
        }
    }
}
