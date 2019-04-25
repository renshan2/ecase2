use [WSS_Content_Apps_eCase_DOI]

GRANT EXECUTE ON dbo.CreateSavedSearchResult TO eCaseUser
GRANT EXECUTE ON dbo.DeleteSavedSearchResultsById TO eCaseUser
GRANT EXECUTE ON dbo.GetDocIdListByPrefix TO eCaseUser
GRANT EXECUTE ON dbo.GetSavedSearchResultsById TO eCaseUser
GRANT EXECUTE ON dbo.GetSavedSearchResultSetsById TO eCaseUser
GRANT EXECUTE ON dbo.GetSavedSearchResultSetsByUser TO eCaseUser
GRANT EXECUTE ON dbo.usp_CreateCaseWeb TO eCaseUser
GRANT EXECUTE ON dbo.usp_CreateSPObjectPermission TO eCaseUser
GRANT EXECUTE ON dbo.usp_DeleteSPObjectPermissions TO eCaseUser
GRANT EXECUTE ON dbo.usp_GetCaseWebs TO eCaseUser
GRANT EXECUTE ON dbo.usp_GetCreateDocId TO eCaseUser
GRANT EXECUTE ON dbo.usp_GetSPObjectPermissions TO eCaseUser