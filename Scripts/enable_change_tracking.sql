use [Content_InProcess];
go 
    if not exists 
    (
        SELECT top 1 1
        FROM sys.change_tracking_databases
        WHERE database_id = DB_ID('Content_InProcess')
    ) 
    BEGIN 
        ALTER DATABASE [Content_InProcess]
        SET CHANGE_TRACKING = ON
        (CHANGE_RETENTION = 5 DAYS, AUTO_CLEANUP = ON)

        ALTER TABLE [dbo].[tPcProductClassificationDetail] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tPcProductClassificationDescription] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tPcRelatedControls] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tChChargeQuota] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tChChargeQuotaNumberMap] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tPcNotes] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tGcAgencyMap] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tGcAgency] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tGcAgencyDescription] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tPcDetailControl] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tPcDetailControlMap] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tdrDocumentControlMap] ENABLE CHANGE_TRACKING
    END


use [BindingRulings];
go
    if not exists
    (
        SELECT top 1 1
        FROM sys.change_tracking_databases
        WHERE database_id = DB_ID('BindingRulings')
    )
    BEGIN
        ALTER DATABASE [BindingRulings]
        SET CHANGE_TRACKING = ON
        (CHANGE_RETENTION = 5 DAYS, AUTO_CLEANUP = ON)

        ALTER TABLE [dbo].[tbrRulings] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tbrRulingsType] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tbrPcMap] ENABLE CHANGE_TRACKING
        ALTER TABLE [dbo].[tbrDescription] ENABLE CHANGE_TRACKING
    END