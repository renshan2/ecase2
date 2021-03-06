/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultSetsByUser]    Script Date: 04/12/2013 10:20:05 ******/
DROP PROCEDURE [dbo].[GetSavedSearchResultSetsByUser]
GO
/****** Object:  StoredProcedure [dbo].[DeleteSavedSearchResultsById]    Script Date: 04/12/2013 10:20:05 ******/
DROP PROCEDURE [dbo].[DeleteSavedSearchResultsById]
GO
/****** Object:  StoredProcedure [dbo].[CreateSavedSearchResult]    Script Date: 04/12/2013 10:20:05 ******/
DROP PROCEDURE [dbo].[CreateSavedSearchResult]
GO
/****** Object:  StoredProcedure [dbo].[CreateSavedSearchResult]    Script Date: 04/12/2013 10:20:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Matthew Dupré
-- Create date: 3/1/2013
-- Description:	Creates a new SavedSearchResult Entry
-- =============================================
CREATE PROCEDURE [dbo].[CreateSavedSearchResult]
	@Id as bigint = 0,
	@Name as nvarchar(50),
	@Description as nvarchar(MAX),
	@Query as nvarchar(MAX),
	@Owner as nvarchar(250)
	--,
	--@IsShared as bit
AS
BEGIN
	SET NOCOUNT ON;

	IF (@Id > 0 AND EXISTS( SELECT 1 FROM SavedSearchResults WHERE Id = @Id ))
		BEGIN
			UPDATE	SavedSearchResults
			SET
					Name = @Name,
					Description = @Description,
					OriginalQuery = @Query,
					Owner = @Owner
					--,
					--IsShared = @IsShared
			WHERE
					Id = @Id
			SELECT @Id
		END
	ELSE
		BEGIN
			INSERT INTO SavedSearchResults
						(
							Name,
							Description,
							Owner,
							OriginalQuery
							--,
							--IsShared
						)
					VALUES 
						(
							@Name,
							@Description,
							@Owner,
							@Query
							--,
							--@IsShared
						)
			SELECT CONVERT(bigint,SCOPE_IDENTITY())
		END
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteSavedSearchResultsById]    Script Date: 04/12/2013 10:20:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Matthew Dupré
-- Create date: 3/1/2013
-- Description:	Gets Saved Search Result Sets for a User
-- =============================================
CREATE PROCEDURE [dbo].[DeleteSavedSearchResultsById] 
	@Id as bigint
AS
BEGIN
	SET NOCOUNT ON;
    
    SET XACT_ABORT ON --Rollback automatically on an error
    BEGIN TRANSACTION
		DELETE FROM SavedSearchResultPermissions
		WHERE
					SavedSearchResultsId = @Id;
    
		DELETE FROM	SavedSearchResultItems
		WHERE
					SavedSearchResultId = @Id;
		
		DELETE FROM SavedSearchResults
		WHERE
					Id = @Id;
	COMMIT TRANSACTION
END
GO
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultSetsByUser]    Script Date: 04/12/2013 10:20:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Matthew Dupré
-- Create date: 3/1/2013
-- Description:	Gets Saved Search Result Sets for a User
-- =============================================
CREATE PROCEDURE [dbo].[GetSavedSearchResultSetsByUser] 
	@User as nvarchar(250)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT		ssr.Id,
				ssr.Name,
				ssr.Description,
				ssr.Owner,
				ssr.OriginalQuery,
				COUNT(ssri.Id) as 'ResultsCount',
				--This statement is a SQL Trick to create a comma separated set of values
				ShareWith = SUBSTRING((SELECT ( ', ' + PermissionName )
								   FROM SavedSearchResultPermissions t2
								   WHERE ssr.Id = t2.SavedSearchResultsId
								   ORDER BY 
									  ssr.Id,
									  PermissionName
								   FOR XML PATH( '' )
								  ), 3, 1000 )
	FROM		SavedSearchResults ssr
	LEFT OUTER
		JOIN	SavedSearchResultItems ssri
	ON			ssr.Id = ssri.SavedSearchResultId
	WHERE		ssr.Owner = @User
		OR		@User IN (SELECT PermissionName FROM SavedSearchResultPermissions ssrp WHERE ssrp.SavedSearchResultsId = ssr.Id)
	GROUP BY	ssr.Id,
				ssr.Name,
				ssr.Description,
				ssr.Owner,
				ssr.OriginalQuery
				
END
GO