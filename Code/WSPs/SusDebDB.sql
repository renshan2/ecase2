USE [eCaseMgmtSusDeb]
GO
/****** Object:  StoredProcedure [dbo].[CreateSavedSearchResult]    Script Date: 9/30/2013 10:36:29 PM ******/
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
	@Owner as nvarchar(250),
	@IsShared as bit
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
					Owner = @Owner,
					IsShared = @IsShared
			WHERE
					Id = @Id
		END
	ELSE
		BEGIN
			INSERT INTO SavedSearchResults
						(
							Name,
							Description,
							Owner,
							OriginalQuery,
							IsShared
						)
					VALUES 
						(
							@Name,
							@Description,
							@Owner,
							@Query,
							@IsShared
						)
			SELECT CONVERT(bigint,SCOPE_IDENTITY())
		END
END


GO
/****** Object:  StoredProcedure [dbo].[DeleteSavedSearchResultsById]    Script Date: 9/30/2013 10:36:29 PM ******/
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
		DELETE FROM	SavedSearchResultItems
		WHERE
					SavedSearchResultId = @Id;
		
		DELETE FROM SavedSearchResults
		WHERE
					Id = @Id;
	COMMIT TRANSACTION
END



GO
/****** Object:  StoredProcedure [dbo].[GetDocIdListByPrefix]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDocIdListByPrefix]
	@SiteGuid nchar(36),
	@WebGuid nchar(36),
	@Prefix nchar(10)
AS

SELECT  ListItemUniqueId
FROM	DocumentIds
WHERE	SiteGuid = @SiteGuid
	AND WebGuid = @WebGuid
	AND Prefix = @Prefix
ORDER BY DocId

GO
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultsById]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Matthew Dupré
-- Create date: 3/1/2013
-- Description:	Gets Saved Search Result Sets for a User
-- =============================================
CREATE PROCEDURE [dbo].[GetSavedSearchResultsById] 
	@Id as bigint
AS
BEGIN
	SET NOCOUNT ON;
    
    SELECT *,
     '<HitHighlightedProperties>' + HitHighlightedProperties + '</HitHighlightedProperties>' as 'HitHighlightedPropertiesXml'
    FROM SavedSearchResultItems
    WHERE SavedSearchResultId = @Id

END



GO
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultSetsById]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Matthew Dupré
-- Create date: 3/1/2013
-- Description:	Gets Saved Search Result Sets for a User
-- =============================================
CREATE PROCEDURE [dbo].[GetSavedSearchResultSetsById] 
	@Id as bigint
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
			Id,
			Name,
			Description,
			OriginalQuery,
			Owner,
			IsShared
	FROM	SavedSearchResults
	WHERE 
			Id = @Id

END



GO
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultSetsByUser]    Script Date: 9/30/2013 10:36:29 PM ******/
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
	SELECT 
			Id,
			Name,
			Description,
			OriginalQuery,
			Owner,
			IsShared
	FROM	SavedSearchResults
	WHERE 
			Owner = @User	OR
			IsShared = 1

END



GO
/****** Object:  StoredProcedure [dbo].[usp_CreateCaseWeb]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Adam Ormond
-- Create date: 11/27/2012
-- Description:	Creates a new entry in the CaseWebs table identifying some SPObject Guids for a case web
-- =============================================
CREATE PROCEDURE [dbo].[usp_CreateCaseWeb] 
	@siteGuid as nchar(36),
	@caseListItemGuid as nchar(36),
	@caseWebGuid as nchar(36),
	@activitiesTasksGuid as nchar(36),
	@caseRelatedDatesGuid as nchar(36)
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS( SELECT 1 FROM CaseWebs WHERE SiteGuid = @siteGuid AND CaseListItemGuid = @caseListItemGuid)
		UPDATE CaseWebs 
		SET CaseWebGuid = @caseWebGuid, ActivitiesTasksGuid = @activitiesTasksGuid, CaseRelatedDatesGuid = @caseRelatedDatesGuid
		WHERE SiteGuid = @siteGuid AND CaseListItemGuid = @caseListItemGuid
	ELSE
		INSERT INTO CaseWebs (SiteGuid, CaseListItemGuid, CaseWebGuid, ActivitiesTasksGuid, CaseRelatedDatesGuid)
		VALUES (@siteGuid, @caseListItemGuid, @caseWebGuid, @activitiesTasksGuid, @caseRelatedDatesGuid)
END


GO
/****** Object:  StoredProcedure [dbo].[usp_CreateSPObjectPermission]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Adam Ormond
-- Create date: 10/29/2012
-- Description:	Creates an entry in the SPObjectPermissions table
-- =============================================
CREATE PROCEDURE [dbo].[usp_CreateSPObjectPermission]
	@siteGuid as nchar(36),
	@caseWebGuid as nchar(36),
	@childWebGuid as nchar(36),
	@listGuid as nchar(36),
	@listItemGuid as nchar(36),
	@roleAssignments as xml
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(
		SELECT 1 FROM SPObjectPermissions 
		WHERE SiteGuid = @siteGuid AND CaseWebGuid = @caseWebGuid AND 
		(((ChildWebGuid IS NULL) AND (@childWebGuid IS NULL)) OR (ChildWebGuid = @childWebGuid)) AND
		(((ListGuid IS NULL) AND (@listGuid IS NULL)) OR (ListGuid = @listGuid)) AND
		(((ListItemGuid IS NULL) AND (@listItemGuid IS NULL)) OR (ListItemGuid = @listItemGuid))
		)
		UPDATE SPObjectPermissions SET RoleAssignments = @roleAssignments 
			WHERE SiteGuid = @siteGuid AND CaseWebGuid = @caseWebGuid AND 
			(((ChildWebGuid IS NULL) AND (@childWebGuid IS NULL)) OR (ChildWebGuid = @childWebGuid)) AND
			(((ListGuid IS NULL) AND (@listGuid IS NULL)) OR (ListGuid = @listGuid)) AND
			(((ListItemGuid IS NULL) AND (@listItemGuid IS NULL)) OR (ListItemGuid = @listItemGuid))
	ELSE
		INSERT INTO dbo.SPObjectPermissions(SiteGuid, CaseWebGuid, ChildWebGuid, ListGuid, ListItemGuid, RoleAssignments)
			VALUES (@siteGuid, @caseWebGuid, @childWebGuid, @listGuid, @listItemGuid, @roleAssignments)
END


GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteSPObjectPermissions]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Adam Ormond
-- Create date: 10/30/2012
-- Description:	Delete all table entries for a Site/CaseWeb
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteSPObjectPermissions]
	@siteGuid as nchar(36),
	@caseWebGuid as nchar(36)
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM SPObjectPermissions WHERE SiteGuid = @siteGuid AND CaseWebGuid = @caseWebGuid
END


GO
/****** Object:  StoredProcedure [dbo].[usp_GetCaseWebs]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Adam Ormond
-- Create date: 11/27/2012
-- Description:	Get all CaseWebs within a Site Collection
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetCaseWebs] 
	@siteGuid as nchar(36)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM CaseWebs WHERE SiteGuid = @siteGuid
END


GO
/****** Object:  StoredProcedure [dbo].[usp_GetCreateDocId]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Adam Ormond
-- Create date: 10/27/2012
-- Modified By: Matthew Dupre
-- Modified Date: 3/20/2013
-- Description:	Create or retrieve Document Id
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetCreateDocId]
	-- Add the parameters for the stored procedure here
	@siteGuid nchar(36),
	@webGuid nchar(36),
	@listItemGuid nchar(36),
	@forceUpdate bit = 0,
	@pre nvarchar(10) output,
	@docId bigint output,
	@uniqueId bigint
AS
BEGIN
	SET NOCOUNT ON;
	
	SET @docId = (SELECT MAX(DocId) FROM DocumentIds WHERE SiteGuid = @siteGuid and WebGuid = @webGuid AND Prefix = @pre)
	IF @docId IS NULL
		SET @docId = 1
	ELSE
		SET @docId += 1
	
	IF (SELECT COUNT(*) FROM dbo.DocumentIds WHERE SiteGuid = @siteGuid AND WebGuid = @WebGuid AND ListItemGuid = @listItemGuid) = 0
	BEGIN
		INSERT INTO dbo.DocumentIds(SiteGuid, WebGuid, ListItemGuid, Prefix, DocId, ListItemUniqueId) 
			VALUES (@siteGuid, @webGuid, @listItemGuid, @pre, @docId, @uniqueId)
	END
	ELSE
	BEGIN
		IF (@forceUpdate = 0)
		BEGIN
			SELECT @docId = DocId, @pre = Prefix FROM dbo.DocumentIds WHERE SiteGuid = @siteGuid AND WebGuid = @webGuid AND ListItemGuid = @listItemGuid
		END
		ELSE
		BEGIN
			UPDATE DocumentIds
				SET DocId = @docId,
					Prefix = @pre
			WHERE SiteGuid = @siteGuid AND WebGuid = @webGuid AND ListItemGuid = @listItemGuid
		END
	END
END


GO
/****** Object:  StoredProcedure [dbo].[usp_GetSPObjectPermissions]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Adam Ormond
-- Create date: 10/30/2012
-- Description:	Retrieve all table entries for a Site/CaseWeb
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetSPObjectPermissions]
	@siteGuid as nchar(36),
	@caseWebGuid as nchar(36)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM SPObjectPermissions WHERE SiteGuid = @siteGuid AND CaseWebGuid = @caseWebGuid
END


GO
/****** Object:  Table [dbo].[CaseWebs]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CaseWebs](
	[SiteGuid] [nchar](36) NOT NULL,
	[CaseListItemGuid] [nchar](36) NOT NULL,
	[CaseWebGuid] [nchar](36) NOT NULL,
	[ActivitiesTasksGuid] [nchar](36) NOT NULL,
	[CaseRelatedDatesGuid] [nchar](36) NOT NULL,
 CONSTRAINT [PK_CaseWebs] PRIMARY KEY CLUSTERED 
(
	[SiteGuid] ASC,
	[CaseListItemGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentIds]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentIds](
	[SiteGuid] [nchar](36) NOT NULL,
	[WebGuid] [nchar](36) NOT NULL,
	[ListItemGuid] [nchar](36) NOT NULL,
	[Prefix] [nvarchar](10) NOT NULL,
	[DocId] [bigint] NOT NULL,
	[ListItemUniqueId] [bigint] NULL,
 CONSTRAINT [PK_DocumentIds] PRIMARY KEY CLUSTERED 
(
	[SiteGuid] ASC,
	[WebGuid] ASC,
	[ListItemGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SavedSearchResultItems]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SavedSearchResultItems](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SavedSearchResultId] [bigint] NOT NULL,
	[WorkId] [nvarchar](50) NULL,
	[Rank] [int] NULL,
	[Author] [nvarchar](50) NULL,
	[Size] [int] NULL,
	[Path] [nvarchar](500) NULL,
	[Description] [nvarchar](max) NULL,
	[SiteName] [nvarchar](500) NULL,
	[HitHighlightedSummary] [nvarchar](max) NULL,
	[HitHighlightedProperties] [nvarchar](max) NULL,
	[ContentClass] [nvarchar](50) NULL,
	[IsDocument] [bit] NULL,
	[PictureThumbnailUrl] [nvarchar](500) NULL,
	[Url] [nvarchar](500) NULL,
	[ServerRedirectedUrl] [nvarchar](500) NULL,
	[FileExtension] [nvarchar](50) NULL,
	[SpSiteUrl] [nvarchar](500) NULL,
	[docvector] [nvarchar](500) NULL,
	[fcocount] [int] NULL,
	[fcoid] [nvarchar](50) NULL,
	[PictureWidth] [int] NULL,
	[PictureHeight] [int] NULL,
	[Reviewed] [bit] NOT NULL,
	[IncludeInSet] [bit] NOT NULL,
 CONSTRAINT [PK_SavedSearchResultItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SavedSearchResultPermissions]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SavedSearchResultPermissions](
	[SavedSearchResultsId] [bigint] NOT NULL,
	[PermissionName] [nvarchar](100) NOT NULL,
	[IsGroup] [bit] NOT NULL,
 CONSTRAINT [PK_SavedSearchResultPermissions] PRIMARY KEY CLUSTERED 
(
	[SavedSearchResultsId] ASC,
	[PermissionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SavedSearchResults]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SavedSearchResults](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Owner] [nvarchar](250) NULL,
	[OriginalQuery] [nvarchar](max) NOT NULL,
	[IsShared] [bit] NOT NULL,
 CONSTRAINT [PK_SavedSearchResults] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SPObjectPermissions]    Script Date: 9/30/2013 10:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SPObjectPermissions](
	[Idx] [bigint] IDENTITY(1,1) NOT NULL,
	[SiteGuid] [nchar](36) NOT NULL,
	[CaseWebGuid] [nchar](36) NOT NULL,
	[ChildWebGuid] [nchar](36) NULL,
	[ListGuid] [nchar](36) NULL,
	[ListItemGuid] [nchar](36) NULL,
	[RoleAssignments] [xml] NULL,
 CONSTRAINT [PK_SPObjectPermissions] PRIMARY KEY CLUSTERED 
(
	[Idx] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[SavedSearchResultItems] ADD  CONSTRAINT [DF_SavedSearchResultItems_Reviewed]  DEFAULT ((0)) FOR [Reviewed]
GO
ALTER TABLE [dbo].[SavedSearchResultItems] ADD  CONSTRAINT [DF_SavedSearchResultItems_IncludeInSet]  DEFAULT ((0)) FOR [IncludeInSet]
GO
ALTER TABLE [dbo].[SavedSearchResultPermissions] ADD  CONSTRAINT [DF_SavedSearchResultPermissions_IsGroup]  DEFAULT ((0)) FOR [IsGroup]
GO
ALTER TABLE [dbo].[SavedSearchResults] ADD  CONSTRAINT [DF_SavedSearchResults_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[SavedSearchResults] ADD  CONSTRAINT [DF_SavedSearchResults_Modified]  DEFAULT (getdate()) FOR [Modified]
GO
ALTER TABLE [dbo].[SavedSearchResults] ADD  CONSTRAINT [DF_SavedSearchResults_IsShared]  DEFAULT ((0)) FOR [IsShared]
GO
ALTER TABLE [dbo].[SavedSearchResultItems]  WITH CHECK ADD  CONSTRAINT [FK_SavedSearchResultItems_SavedSearchResults] FOREIGN KEY([SavedSearchResultId])
REFERENCES [dbo].[SavedSearchResults] ([Id])
GO
ALTER TABLE [dbo].[SavedSearchResultItems] CHECK CONSTRAINT [FK_SavedSearchResultItems_SavedSearchResults]
GO
ALTER TABLE [dbo].[SavedSearchResultPermissions]  WITH CHECK ADD  CONSTRAINT [FK_SavedSearchResultPermissions_SavedSearchResults] FOREIGN KEY([SavedSearchResultsId])
REFERENCES [dbo].[SavedSearchResults] ([Id])
GO
ALTER TABLE [dbo].[SavedSearchResultPermissions] CHECK CONSTRAINT [FK_SavedSearchResultPermissions_SavedSearchResults]
GO
