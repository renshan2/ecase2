USE [eCaseManagement]
GO
USE [eCaseManagementMyBureau]
GO
/****** Object:  ForeignKey [FK_SavedSearchResultItems_SavedSearchResults]    Script Date: 03/20/2013 17:59:23 ******/
ALTER TABLE [dbo].[SavedSearchResultItems] DROP CONSTRAINT [FK_SavedSearchResultItems_SavedSearchResults]
GO
/****** Object:  StoredProcedure [dbo].[DeleteSavedSearchResultsById]    Script Date: 03/20/2013 17:59:23 ******/
DROP PROCEDURE [dbo].[DeleteSavedSearchResultsById]
GO
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultsById]    Script Date: 03/20/2013 17:59:23 ******/
DROP PROCEDURE [dbo].[GetSavedSearchResultsById]
GO
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultSetsById]    Script Date: 03/20/2013 17:59:23 ******/
DROP PROCEDURE [dbo].[GetSavedSearchResultSetsById]
GO
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultSetsByUser]    Script Date: 03/20/2013 17:59:23 ******/
DROP PROCEDURE [dbo].[GetSavedSearchResultSetsByUser]
GO
/****** Object:  Table [dbo].[SavedSearchResultItems]    Script Date: 03/20/2013 17:59:23 ******/
ALTER TABLE [dbo].[SavedSearchResultItems] DROP CONSTRAINT [FK_SavedSearchResultItems_SavedSearchResults]
GO
ALTER TABLE [dbo].[SavedSearchResultItems] DROP CONSTRAINT [DF_SavedSearchResultItems_Reviewed]
GO
ALTER TABLE [dbo].[SavedSearchResultItems] DROP CONSTRAINT [DF_SavedSearchResultItems_IncludeInSet]
GO
DROP TABLE [dbo].[SavedSearchResultItems]
GO
/****** Object:  StoredProcedure [dbo].[CreateSavedSearchResult]    Script Date: 03/20/2013 17:59:23 ******/
DROP PROCEDURE [dbo].[CreateSavedSearchResult]
GO
/****** Object:  Table [dbo].[SavedSearchResults]    Script Date: 03/20/2013 17:59:23 ******/
ALTER TABLE [dbo].[SavedSearchResults] DROP CONSTRAINT [DF_SavedSearchResults_Created]
GO
ALTER TABLE [dbo].[SavedSearchResults] DROP CONSTRAINT [DF_SavedSearchResults_Modified]
GO
ALTER TABLE [dbo].[SavedSearchResults] DROP CONSTRAINT [DF_SavedSearchResults_IsShared]
GO
DROP TABLE [dbo].[SavedSearchResults]
GO
/****** Object:  Table [dbo].[SavedSearchResults]    Script Date: 03/20/2013 17:59:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SavedSearchResults](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](50) NOT NULL,
    [Description] [nvarchar](max) NULL,
    [Created] [datetime] NOT NULL CONSTRAINT [DF_SavedSearchResults_Created]  DEFAULT (getdate()),
    [Modified] [datetime] NOT NULL CONSTRAINT [DF_SavedSearchResults_Modified]  DEFAULT (getdate()),
    [Owner] [nvarchar](250) NULL,
    [OriginalQuery] [nvarchar](max) NOT NULL,
    [IsShared] [bit] NOT NULL CONSTRAINT [DF_SavedSearchResults_IsShared]  DEFAULT ((0)),
 CONSTRAINT [PK_SavedSearchResults] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[CreateSavedSearchResult]    Script Date: 03/20/2013 17:59:23 ******/
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
/****** Object:  Table [dbo].[SavedSearchResultItems]    Script Date: 03/20/2013 17:59:23 ******/
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
    [Reviewed] [bit] NOT NULL CONSTRAINT [DF_SavedSearchResultItems_Reviewed]  DEFAULT ((0)),
    [IncludeInSet] [bit] NOT NULL CONSTRAINT [DF_SavedSearchResultItems_IncludeInSet]  DEFAULT ((0)),
 CONSTRAINT [PK_SavedSearchResultItems] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultSetsByUser]    Script Date: 03/20/2013 17:59:23 ******/
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
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultSetsById]    Script Date: 03/20/2013 17:59:23 ******/
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
/****** Object:  StoredProcedure [dbo].[GetSavedSearchResultsById]    Script Date: 03/20/2013 17:59:23 ******/
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
/****** Object:  StoredProcedure [dbo].[DeleteSavedSearchResultsById]    Script Date: 03/20/2013 17:59:23 ******/
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
/****** Object:  ForeignKey [FK_SavedSearchResultItems_SavedSearchResults]    Script Date: 03/20/2013 17:59:23 ******/
ALTER TABLE [dbo].[SavedSearchResultItems]  WITH CHECK ADD  CONSTRAINT [FK_SavedSearchResultItems_SavedSearchResults] FOREIGN KEY([SavedSearchResultId])
REFERENCES [dbo].[SavedSearchResults] ([Id])
GO
ALTER TABLE [dbo].[SavedSearchResultItems] CHECK CONSTRAINT [FK_SavedSearchResultItems_SavedSearchResults]
GO