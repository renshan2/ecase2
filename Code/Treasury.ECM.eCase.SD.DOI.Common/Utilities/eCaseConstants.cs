using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities
{
    public static class eCaseConstants
    {
        public static class ContentTypeIds
        {
            public static readonly SPContentTypeId CASE_DOCUMENT = new SPContentTypeId("0x0101003041B5E0CD13446EA76F1EEBD337C805");
            public static readonly SPContentTypeId INVESTIGATION_DOCUMENT = new SPContentTypeId("0x01010038AD692CEEA64466A3DE01EF1A74C7F9");
            public static readonly SPContentTypeId SDO_DOCUMENT = new SPContentTypeId("0x0101003079571AD5D4460EB5CA06D46026B835");
            public static readonly SPContentTypeId RELATED_DATES = new SPContentTypeId("0x0102005BCB360AEA6746A48FB84ECC1C779399");
            public static readonly SPContentTypeId SHARE_WITH_EXTERNAL_USER = new SPContentTypeId("0x01010063392445c0004d7bb864992d4a3f12e6");
        }

        public static class ContentTypeNames
        {
            public static readonly string CASE = "eCase";
        }

        public static class PropertyBagKeys
        {
            public static readonly string ECASE_DB_CONNECTION_STRING = "eCaseDb";
            public static readonly string ECASE_DOC_ID_PREFIX = "eCaseDocIdPrefix";
            public static readonly string ECASE_CASE_LIST_ITEM_GUID = "eCaseCaseListItemGuid";
            public static readonly string ECASE_SAVED_SEARCH_RESULTS_SCOPE = "eCaseSavedSearchResults-Scope";
            public static readonly string ECASE_SAVED_SEARCH_RESULTS_MAX_RESULTS = "eCaseSavedSearchResults-MaxResults";
            public static readonly string ECASE_WORKFLOWS_TO_ASSOCIATE = "ecaseworkflowstoassociate";
        }

        public static class PropertyBagDefaultValues
        {
            //public static readonly string DEFAULT_WORKFLOW_NAMES = "Approval - SharePoint 2010|Collect Feedback - SharePoint 2010|Collect Signatures - SharePoint 2010";
            public static readonly string DEFAULT_WORKFLOW_NAMES = "Collect Signatures";
            public static readonly string ECASE_SAVED_SEARCH_RESULTS_MAX_RESULTS = "100000";
        }

        public static class ListInternalNames
        {
            public static readonly string ECASES_LIST = "Cases";
            public static readonly string REFERRAL_DOCUMENTS = "ReferralDocuments";
            public static readonly string INVESTIGATION_DOCUMENTS = "InvestigationDocuments";
            public static readonly string SDO_DOCUMENTS = "SDODocuments";
            public static readonly string CASE_RELATED_DATES = "CaseRelatedDates";
            public static readonly string MATTER_RELATED_DATES = "MatterRelatedDates";
            public static readonly string ACTIVITIES_AND_TASKS = "TasksAndActivities";
            public static readonly string LEGAL_ISSUES = "LegalIssues";
            public static readonly string SHARE_WITH_EXTERNAL_USERS = "ShareWithExternalUsers";
            public static readonly string WORKFLOW_HISTORY_LIST = "WorkflowHistoryList";
            public static readonly string TASKS_LIST = "TasksList";
            public static readonly string BUREAU_AUTONUMBER_LIST = "BureauAutonumberList";
            public static readonly string SAND_AUTONUMBER_LIST = "SusDebAutonumberList";
        }

        public static class FeatureIds
        {
            public static readonly Guid CASE_SITE_COMPONENTS = new Guid("a739371e-a68a-49f7-833b-b0e9a3ec6832");
            public static readonly Guid LEGACY_WORKFLOWS = new Guid("c845ed8d-9ce5-448c-bd3e-ea71350ce45b");
            public static readonly Guid WORKFLOWS = new Guid("0af5989a-3aea-4519-8ab0-85d91abe39ff");
        }

        public static class FieldGuids
        {
            public static readonly Guid OOTB_TITLE = new Guid("fa564e0f-0c70-4ab9-b863-0177e6ddd247");
            public static readonly Guid OOTB_ASSIGNEDTO = new Guid("53101f38-dd2e-458c-b245-0c236cc13d1a");
            public static readonly Guid OOTB_TAXKEYWORD = new Guid("23f27201-bee3-471e-b2e7-b64fd8b7ca38");
            public static readonly Guid OOTB_ALL_DAY_EVENT = new Guid("7d95d1f4-f5fd-4a70-90cd-b35abc9b5bc8");
            public static readonly Guid OOTB_END_DATE = new Guid("2684f9f2-54be-429f-ba06-76754fc056bf");
            public static readonly Guid OOTB_EVENT_DATE = new Guid("64cd368d-2f95-4bfc-a1f9-8d4324ecb007");
            public static readonly Guid OOTB_DUE_DATE = new Guid("cd21b4c2-6841-4f9e-a23a-738a65f99889");
            public static readonly Guid ECASES_LIST_ASSIGNEDTOSUPERVISOR = new Guid("6898cec8-06c6-4437-9390-bd36911d3bd9");
            public static readonly Guid ECASES_LIST_CASE_STATUS_LOOKUP = new Guid("f585eaed-888d-427d-8763-3e6c11550685");
            public static readonly Guid ECASES_LIST_CASEURL = new Guid("734612f0-972f-4ff2-971f-83d9e68df310");
            public static readonly Guid ECASES_LIST_DESCRIPTION = new Guid("9da97a8a-1da5-4a77-98d3-4bc10456e700");
            public static readonly Guid ECASES_LIST_UNIQUECASEID = new Guid("028a6924-b06e-4ec6-8e37-c33928ed02ce");
            public static readonly Guid ECASES_LIST_NEXTDUEDATEURL = new Guid("53C0A28A-3AE0-42FD-99BF-52EE69BCEBF8");
            public static readonly Guid ECASES_LIST_TASKDUEDATE = new Guid("cd21b4c2-6841-4f9e-a23a-738a65f99889");
            public static readonly Guid ECASES_LIST_UIL = new Guid("{e7c447ba-fc04-451c-9c8d-1695cdaadf17}");
            public static readonly Guid ECASES_LIST_JUDGE = new Guid("{90B46957-5EB1-4F27-9917-EFC743FB76B8}");            
            public static readonly Guid ECASE_STATUSES_LOCK_SITE = new Guid("1820AC05-5F89-4C01-8F33-9B2B2EAEA4EA");
            public static readonly Guid GROUPPROXY_LIST_GROUPMEMBER = new Guid("{f1a953a7-b95b-4805-8faa-38e3352a6b9d}");
        }

        public static class QueryStringKeys
        {
            public const string BATCH_COPY_MANY_ITEMS_QUERYSTRING_KEY = "serverItemsList";
        }

        public static class SessionKeys
        {
            public const string BATCH_COPY_ITEMS_SESSION_KEY_NAME = "eCase-BatchCopyItemsList";
            public const string DOC_ID_RENUMBER_SITE_ID = "eCase-DocIdRenumberSiteId";
            public const string DOC_ID_RENUMBER_WEB_ID = "eCase-DocIdRenumberWebId";
            public const string DOC_ID_RENUMBER_LIST_ID = "eCase-DocIdRenumberListId";
            public const string DOC_ID_RENUMBER_ITEMS = "eCase-DocIdRenumberItems";
            public const string SEARCH_STORED_REFERRER_URL = "eCase-SearchStoredReferrerUrl";
        }

        public static class Taxonomy
        {
            public static readonly string METADATA_GROUP_NAME = "eCase Terms";
            public static readonly string METADATA_TERMSET_TAX_JUDGE = "Tax Court Judges";
            public static readonly string METADATA_TERMSET_TAX_LIL = "Law Issue List";
            public static readonly string METADATA_TERMSET_TAX_KEYWORDS = "Keywords List";
        }
    }
}
