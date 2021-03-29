1. create or update destination table for change tracking data

Destination table is used for keeping change tracking data. 
To upload or remove data from destination table separate SQL Server job is used.
Destination table must be created only once. If some changes were done to table structure, follow update procedure.

Create table:
- run "create_change_tracking_destination_table.sql" 

Update table:
- uncomment drop table statement in "create_change_tracking_destination_table.sql" script and run it.


2. enable change tracking

Change Tracking must be enabled for the database and all table before any change tracking operations.
- run "enable_change_tracking.sql"


3. create SQL Server job

SQL Server job is a procedure that will be uploading and cleaning data in destination table
- run "create_change_tracking_job.sql"