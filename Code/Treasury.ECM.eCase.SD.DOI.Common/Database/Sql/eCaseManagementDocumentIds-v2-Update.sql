SELECT * FROM DocumentIds

BEGIN TRANSACTION New_Column

BEGIN TRY
	CREATE TABLE [dbo].[DocumentIds_new](
		[SiteGuid] [nchar](36) NOT NULL,
		[WebGuid] [nchar] (36) NOT NULL,
		[ListItemGuid] [nchar](36) NOT NULL,
		[Prefix] [nvarchar](10) NOT NULL,
		[DocId] [bigint] NOT NULL,
	 CONSTRAINT [PK_DocumentIds_new] PRIMARY KEY CLUSTERED 
	(
		[SiteGuid] ASC,
		[WebGuid] ASC,
		[ListItemGuid] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	INSERT INTO DocumentIds_new
	SELECT SiteGuid, '00000000-0000-0000-0000-000000000000' as WebGuid, ListItemGuid, Prefix, DocId
	FROM DocumentIds

	DROP TABLE DocumentIds
	
	exec sp_rename 'DocumentIds_new.PK_DocumentIds_new', 'PK_DocumentIds'
	exec sp_rename 'DocumentIds_new', 'DocumentIds'
	
	COMMIT TRANSACTION New_Column
END TRY
BEGIN CATCH
	SELECT
	ERROR_NUMBER() AS ErrorNumber
	,ERROR_SEVERITY() AS ErrorSeverity
	,ERROR_STATE() AS ErrorState
	,ERROR_PROCEDURE() AS ErrorProcedure
	,ERROR_LINE() AS ErrorLine
	,ERROR_MESSAGE() AS ErrorMessage;
	
	ROLLBACK TRANSACTION New_Column
END CATCH

SELECT * FROM DocumentIds




/****** Object:  StoredProcedure [dbo].[usp_GetCreateDocId]    Script Date: 03/20/2013 09:05:55 ******/
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
ALTER PROCEDURE [dbo].[usp_GetCreateDocId]
	-- Add the parameters for the stored procedure here
	@siteGuid nchar(36),
	@webGuid nchar(36),
	@listItemGuid nchar(36),
	@forceUpdate bit = 0,
	@pre nvarchar(10) output,
	@docId bigint output
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
		INSERT INTO dbo.DocumentIds(SiteGuid, WebGuid, ListItemGuid, Prefix, DocId) 
			VALUES (@siteGuid, @webGuid, @listItemGuid, @pre, @docId)
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
/****** Object:  StoredProcedure [dbo].[GetDocIdListByPrefix]    Script Date: 03/20/2013 09:05:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDocIdListByPrefix]
	@SiteGuid nchar(36),
	@WebGuid nchar(36),
	@Prefix nchar(10)
AS

SELECT	DocId
FROM	DocumentIds
WHERE	SiteGuid = @SiteGuid
	AND WebGuid = @WebGuid
	AND Prefix = @Prefix
ORDER BY DocId
GO

/****** Object:  StoredProcedure [dbo].[usp_GetCreateDocId]    Script Date: 03/20/2013 09:13:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetCreateDocId]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

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
    @docId bigint output
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
        INSERT INTO dbo.DocumentIds(SiteGuid, WebGuid, ListItemGuid, Prefix, DocId) 
            VALUES (@siteGuid, @webGuid, @listItemGuid, @pre, @docId)
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


' 
END
GO