USE [msdb];
GO

IF NOT EXISTS 
(
	SELECT name 
	FROM dbo.syscategories 
	WHERE name=N'[Uncategorized (Local)]' 
		AND category_class=1
)
BEGIN
	EXEC dbo.sp_add_category 
		@class=N'JOB', 
		@type=N'LOCAL', 
		@name=N'[Uncategorized (Local)]';
END
GO


EXEC dbo.sp_add_job  
    @job_name = N'Content_ChangeTracking',
	@category_name=N'[Uncategorized (Local)]',
	@owner_login_name=N'sa',
	@notify_level_eventlog=0 ;
GO 


EXEC sp_add_jobstep  
    @job_name = N'Content_ChangeTracking',  
	@step_id = 1, 
    @step_name = N'Copy Content_InProcess changes',  
    @subsystem = N'TSQL',  
    @command = N'

use Content_InProcess;
go 

declare @tableName nvarchar(255);
declare @guidName nvarchar(255);
declare @query nvarchar(max);

create table #contentCDCTables 
(
   processed bit,
   tableName nvarchar(255),
   guidName nvarchar(255)
);

insert into #contentCDCTables values (0,''tPcProductClassificationDetail'',''ProdClassificationDetailGUID'');
insert into #contentCDCTables values (0,''tPcProductClassificationDescription'',''ProdClassificationDescriptionGUID'');
insert into #contentCDCTables values (0,''tPcRelatedControls'',''RelatedControlGuid'');
insert into #contentCDCTables values (0,''tChChargeQuota'',''ChargeQuotaGuid'');
insert into #contentCDCTables values (0,''tChChargeQuotaNumberMap'',''ChargeQuotaNumberGUID'');
insert into #contentCDCTables values (0,''tPcNotes '',''NoteGuid'');
insert into #contentCDCTables values (0,''tGcAgencyMap '',''AgencyMapGuid'');
insert into #contentCDCTables values (0,''tGcAgency '',''AgencyGuid'');
insert into #contentCDCTables values (0,''tGcAgencyDescription '',''AgencyDescriptionGuid'');
insert into #contentCDCTables values (0,''tPcDetailControl '',''DetailControlGuid'');
insert into #contentCDCTables values (0,''tPcDetailControlMap '',''DetailControlMapGuid'');
insert into #contentCDCTables values (0,''tdrDocumentControlMap '',''DocumentControlGuid'');

select top 1 
   @tablename = tableName,
   @guidName = guidName
from #contentCDCTables 
where 
   Processed = 0;


while @tablename is not null begin
   set @query = N''
   declare @changeVersion as bigint;
    
   if exists 
   (
       select top 1 1
       from [ContentUpdates].[dbo].[tChangeTracking]
       where TableName = '''''' + @tablename + ''''''
   ) 
   begin
       select top 1 @changeVersion = ChangeVersion
       from [ContentUpdates].[dbo].[tChangeTracking]
       where TableName = '''''' + @tablename + ''''''
       order by ChangeVersion desc;
   end
   else begin
       set @changeVersion = 0;
   end
    
   insert into [ContentUpdates].[dbo].[tChangeTracking]
   (EntityId, TableName, ChangeVersion, Operation, DateChanged)
   select 
       '' + @guidName + '',
       '''''' + @tablename + '''''',
       sys_change_version,
       sys_change_operation,
       GETDATE()
   FROM CHANGETABLE(CHANGES dbo.'' + @tableName + '', @changeVersion) AS CT;
    
   '';
   exec sp_executesql @query;
    
   update #contentCDCTables 
   set Processed = 1 
   where tableName = @tablename;
    
   set @tablename = NULL;
   set @guidName = NULL;
   set @query = NULL;
    
   select top 1 
	   @tablename = tableName,
       @guidName = guidName
   from #contentCDCTables 
   where Processed = 0;
end	

	',   
    @retry_attempts = 0,  
    @retry_interval = 0,
	@on_success_action = 3,
	@database_name=N'Content_InProcess' ;
GO  


EXEC sp_add_jobstep  
    @job_name = N'Content_ChangeTracking',  
	@step_id = 2, 
    @step_name = N'Copy BindingRulings changes',  
    @subsystem = N'TSQL',  
    @command = N'

use BindingRulings;
go 

declare @tableName nvarchar(255);
declare @guidName nvarchar(255);
declare @query nvarchar(max);

create table #contentCDCTables 
(
   processed bit,
   tableName nvarchar(255),
   guidName nvarchar(255)
);

insert into #contentCDCTables values (0,''tbrRulings'',''BindingRulingGuid'');
insert into #contentCDCTables values (0,''tbrRulingsType'',''BindingRulingTypeGuid'');
insert into #contentCDCTables values (0,''tbrPcMap'',''brPcMapGuid'');
insert into #contentCDCTables values (0,''tbrDescription'',''BindingRulingDescriptionGuid'');

select top 1 
   @tablename = tableName,
   @guidName = guidName
from #contentCDCTables 
where 
   Processed = 0;


while @tablename is not null begin
   set @query = N''
   declare @changeVersion as bigint;
    
   if exists 
   (
       select top 1 1
       from [ContentUpdates].[dbo].[tChangeTracking]
       where TableName = '''''' + @tablename + ''''''
   ) 
   begin
       select top 1 @changeVersion = ChangeVersion
       from [ContentUpdates].[dbo].[tChangeTracking]
       where TableName = '''''' + @tablename + ''''''
       order by ChangeVersion desc;
   end
   else begin
       set @changeVersion = 0;
   end
    
   insert into [ContentUpdates].[dbo].[tChangeTracking]
   (EntityId, TableName, ChangeVersion, Operation, DateChanged)
   select 
       '' + @guidName + '',
       '''''' + @tablename + '''''',
       sys_change_version,
       sys_change_operation,
       GETDATE()
   FROM CHANGETABLE(CHANGES dbo.'' + @tableName + '', @changeVersion) AS CT;
    
   '';
   exec sp_executesql @query;
    
   update #contentCDCTables 
   set Processed = 1 
   where tableName = @tablename;
    
   set @tablename = NULL;
   set @guidName = NULL;
   set @query = NULL;
    
   select top 1 
	   @tablename = tableName,
       @guidName = guidName
   from #contentCDCTables 
   where Processed = 0;
end	
	
	',   
    @retry_attempts = 0,  
    @retry_interval = 0,
	@on_success_action = 3,
	@database_name=N'BindingRulings' ;
GO  

EXEC sp_add_jobstep  
    @job_name = N'Content_ChangeTracking',
	@step_id = 3, 
    @step_name = N'Delete changes',  
    @subsystem = N'TSQL',  
    @command = N'

	delete from [ContentUpdates].[dbo].[tChangeTracking] 
	where DateChanged < (GETDATE() - 30)

	',   
    @retry_attempts = 0,  
    @retry_interval = 0,
	@on_success_action = 3,
	@database_name=N'master' ;
GO  


EXEC dbo.sp_add_schedule  
    @schedule_name = N'11am not sat',  
    @freq_type=8, 
	@freq_interval=63, 
	@freq_subday_type=1, 
	@freq_recurrence_factor=1, 
	@active_end_date=99991231, 
	@active_start_time=110000, 
	@active_end_time=235959 ;  
GO

EXEC sp_attach_schedule  
   @job_name = N'Content_ChangeTracking',
   @schedule_name = N'11am not sat';  
GO


EXEC dbo.sp_add_schedule  
    @schedule_name = N'3pm not sat',  
	@freq_type=8, 
	@freq_interval=63, 
	@freq_subday_type=1, 
	@freq_recurrence_factor=1, 
	@active_end_date=99991231, 
	@active_start_time=150000, 
	@active_end_time=235959 ;  
GO

EXEC sp_attach_schedule  
   @job_name = N'Content_ChangeTracking',
   @schedule_name = N'3pm not sat';  
GO


EXEC dbo.sp_add_schedule  
    @schedule_name = N'7AM Daily',  
	@freq_type=4, 
	@freq_interval=1, 
	@freq_subday_type=1, 
	@freq_recurrence_factor=0, 
	@active_end_date=99991231, 
	@active_start_time=70000, 
	@active_end_time=235959 ;  
GO

EXEC sp_attach_schedule  
   @job_name = N'Content_ChangeTracking',
   @schedule_name = N'7AM Daily';  
GO


EXEC dbo.sp_add_jobserver  
    @job_name = N'Content_ChangeTracking',
	@server_name = N'(local)' ;
GO 
