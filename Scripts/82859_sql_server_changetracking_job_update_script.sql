
-- todo: create these entities if not exist

alter table -[ContentUpdates].[dbo].[tChangeTracking]
add Operation nchar(1) null
go 

update -[ContentUpdates].[dbo].[tChangeTracking]
set Operation = 'I'
go 

alter table -[ContentUpdates].[dbo].[tChangeTracking]
alter column Operation nchar(1) not null
go



-- update 2nd step
exec -msdb..sp_update_jobstep
	@job_name = N'Content_ChangeTracking',
	@step_id = 2,
	@subsystem = N'TSQL',
	@command = N'
use Content_InProcess;
go --drop table #contentCDCTables
   --go

declare @tableName as varchar(255);
declare @guidName nvarchar(255);
declare @guidName0 nvarchar(255);
declare @guidName1 nvarchar(255);
declare @guidName2 nvarchar(255);
declare @query nvarchar(max);

create table #contentCDCTables 
(
   processed bit,
   tableName nvarchar(255),
   guidName nvarchar(255),
   guidName0 nvarchar(255),
   guidName1 nvarchar(255),
   guidName2 nvarchar(255)
);

insert into #contentCDCTables values (0,''tPcProductClassificationDescription'',''ProdClassificationDescriptionGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tPcProductClassificationDetail'',''ProdClassificationDetailGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tPcReportUnitofMeasure'',''ReportUOMGuid'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tPcProductClassification'',''ProdClassificationGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tpcDetailControlMap'',''DetailControlMapGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tpcDetailControl'',''DetailControlGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tGcGroupCode'',''CountryGroupGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tgcAgencyMap'',''AgencyMapGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tgcAgencyDescription'',''AgencyDescriptionGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tgcAgency'',''AgencyGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tPcRelatedControls'',''RelatedControlGuid'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tPcAdditionalCode'',''AdditionalCodeGuid'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tPcAdditionalCodeDescription'',''AdditionalCodeDescriptionGuid'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tgcDeclarableElements'',''DeclarableElementGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tgcDeclarableElementUOM'',''DeclarableElementUOMGuid'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChChargeQuota'',''ChargeQuotaGuid'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChChargeQuotaNumberMap'',''ChargeQuotaNumberGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tPcNotes'',''NoteGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChChargeDetailExclusion'',''ChargeDetailExclusionGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChChargeDetail'',''ChargeDetailGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChChargeDetailType'',''ChargeDetailTypeGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChChargeFormula'',''FormulaGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChDetailMaxMin'',''DetailMaxMinGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChDetailRate'',''ChargeDetailRateGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChRestrictedCompanies'',''RestrictedCompanyGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tGcNotes'',''ChildContentNoteGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tPcProductClassificationUse'',''ProdClassificationGUID'',''ProdClassificationUse'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tdrDocumentControlMap'',''DocumentControlGuid'',''DocumentGuid'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tGcGlobalCodes'',''FieldName'',''Code'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tChPcMap'',''ProdClassificationDetailGUID'',''ChargeDetailGUID'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tGcCountryGroup'',''CountryGroupGUID'',''CountryCode'',''EffectivityDate'',''ExpirationDate'');


select top 1 
   @tablename = tableName,
   @guidName = guidName,
   @guidName0 = guidName0,
   @guidName1 = guidName1,
   @guidName2 = guidName2
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
   (GuidChanged, GuidChanged0, GuidChanged1, GuidChanged2, DateChanged, TableName, ChangeVersion, Operation)
   select 
       CAST('' + @guidName + '' AS nvarchar(max)),
       CAST('' + @guidName0 + '' AS nvarchar(max)),
       CAST('' + @guidName1 + '' AS nvarchar(max)),
       CAST('' + @guidName2 + '' AS nvarchar(max)),
       GETDATE(),
       '''''' + @tablename + '''''',
       sys_change_version,
       sys_change_operation
   FROM CHANGETABLE(CHANGES dbo.'' + @tableName + '', @changeVersion) AS CT;
    
   '';
   exec sp_executesql @query;
    
   update #contentCDCTables 
   set Processed = 1 
   where tableName = @tablename;
    
   set @tablename = NULL;
   set @guidName = NULL;
   set @guidName0 = NULL;
   set @guidName1 = NULL;
   set @guidName2 = NULL;
   set @query = NULL;
    
   select top 1 @tablename = tableName,
       @guidName = guidName,
       @guidName0 = guidName0,
       @guidName1 = guidName1,
       @guidName2 = guidName2
   from #contentCDCTables where Processed = 0;
end	
	
	',
	@database_name=N'Content_InProcess';
go




-- update 3rd step
exec -msdb..sp_update_jobstep
	@job_name = N'Content_ChangeTracking',
	@step_id = 3,
	@subsystem = N'TSQL',
	@command = N'
use DTS;
go --drop table #contentCDCTables
   --go

declare @tableName as varchar(255);
declare @guidName nvarchar(255);
declare @guidName0 nvarchar(255);
declare @guidName1 nvarchar(255);
declare @guidName2 nvarchar(255);
declare @query nvarchar(max);

create table #contentCDCTables 
(
   processed bit,
   tableName nvarchar(255),
   guidName nvarchar(255),
   guidName0 nvarchar(255),
   guidName1 nvarchar(255),
   guidName2 nvarchar(255)
);

insert into #contentCDCTables values (0,''tmeRegList'',''RegListID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tmeRegListCountryMap'',''RegListID'',''CountryCode'',''NULL'',''NULL'');

select top 1 @tablename = tableName,
   @guidName = guidName,
   @guidName0 = guidName0,
   @guidName1 = guidName1,
   @guidName2 = guidName2
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
   (GuidChanged, GuidChanged0, GuidChanged1, GuidChanged2, DateChanged, TableName, ChangeVersion, Operation)
   select 
       CAST('' + @guidName + '' AS nvarchar(max)),
       CAST('' + @guidName0 + '' AS nvarchar(max)),
       CAST('' + @guidName1 + '' AS nvarchar(max)),
       CAST('' + @guidName2 + '' AS nvarchar(max)),
       GETDATE(),
       '''''' + @tablename + '''''',
       sys_change_version,
       sys_change_operation
   FROM CHANGETABLE(CHANGES dbo.'' + @tableName + '', @changeVersion) AS CT;
    
   '';
   exec sp_executesql @query;
    
   update #contentCDCTables 
   set Processed = 1 
   where tableName = @tablename;
    
   set @tablename = NULL;
   set @guidName = NULL;
   set @guidName0 = NULL;
   set @guidName1 = NULL;
   set @guidName2 = NULL;
   set @query = NULL;
    
   select top 1 
       @tablename = tableName,
       @guidName = guidName,
       @guidName0 = guidName0,
       @guidName1 = guidName1,
       @guidName2 = guidName2
   from #contentCDCTables 
   where Processed = 0;
end	
	
	',
	@database_name=N'DTS';
go




-- update 4th step
exec msdb..sp_update_jobstep
	@job_name = N'Content_ChangeTracking',
	@step_id = 4,
	@subsystem = N'TSQL',
	@command = N'
use BindingRulings;
go --drop table #contentCDCTables
   --go

declare @tableName as varchar(255);
declare @guidName nvarchar(255);
declare @guidName0 nvarchar(255);
declare @guidName1 nvarchar(255);
declare @guidName2 nvarchar(255);
declare @query nvarchar(max);

create table #contentCDCTables 
(
   processed bit, 
   tableName nvarchar(255),
   guidName nvarchar(255),
   guidName0 nvarchar(255),
   guidName1 nvarchar(255),
   guidName2 nvarchar(255)
);

insert into #contentCDCTables values (0,''tbrRulings'',''BindingRulingGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tbrRulingsType'',''BindingRulingTypeGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tbrPcMap'',''brPcMapGUID'',''NULL'',''NULL'',''NULL'');
insert into #contentCDCTables values (0,''tbrDescription'',''BindingRulingDescriptionGUID'',''NULL'',''NULL'',''NULL'');

select top 1 @tablename = tableName,
   @guidName = guidName,
   @guidName0 = guidName0,
   @guidName1 = guidName1,
   @guidName2 = guidName2
from #contentCDCTables 
where Processed = 0;


while @tablename is not null 
begin
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
   (GuidChanged, GuidChanged0, GuidChanged1, GuidChanged2, DateChanged, TableName, ChangeVersion, Operation)
   select 
       CAST('' + @guidName + '' AS nvarchar(max)),
       CAST('' + @guidName0 + '' AS nvarchar(max)),
       CAST('' + @guidName1 + '' AS nvarchar(max)),
       CAST('' + @guidName2 + '' AS nvarchar(max)),
       GETDATE(),
       '''''' + @tablename + '''''',
       sys_change_version,
       sys_change_operation
   FROM CHANGETABLE(CHANGES dbo.'' + @tableName + '', @changeVersion) AS CT;
    
   '';
   exec sp_executesql @query;
    
   update #contentCDCTables 
   set Processed = 1 
   where tableName = @tablename;
    
   set @tablename = NULL;
   set @guidName = NULL;
   set @guidName0 = NULL;
   set @guidName1 = NULL;
   set @guidName2 = NULL;
   set @query = NULL;
    
   select top 1 
       @tablename = tableName,
       @guidName = guidName,
       @guidName0 = guidName0,
       @guidName1 = guidName1,
       @guidName2 = guidName2
   from #contentCDCTables 
   where Processed = 0;
end
	
	',
	@database_name=N'BindingRulings';
go
