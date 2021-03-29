use [ContentUpdates];

-- drop table if exists tChangeTracking;
-- go

if not exists 
(
    SELECT top 1 1
    FROM sys.change_tracking_databases
    WHERE database_id = DB_ID('tChangeTracking')
) 
begin
    create table [tChangeTracking] 
    (
        [RowID] [bigint] IDENTITY(0, 1) NOT NULL,
        [EntityId] [nvarchar](255) NOT NULL, -- UNIQUEIDENTIFIER ?
        [TableName] [varchar](255) NOT NULL,
        [Operation] [nchar](1) NOT NULL,
        [ChangeVersion] [bigint] NOT NULL,
        [DateChanged] [datetime] NOT NULL,

        INDEX IX_tChangeTracking_TableName ([TableName])
    )
end
go
