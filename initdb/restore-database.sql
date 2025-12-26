-- Wait for SQL Server to be ready
WAITFOR DELAY '00:00:05';
GO

-- Check if database exists by looking for actual data tables
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'PetShopDB' AND state_desc = 'ONLINE')
   OR NOT EXISTS (SELECT 1 FROM PetShopDB.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    -- Drop database if exists but incomplete
    IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PetShopDB')
    BEGIN
        ALTER DATABASE PetShopDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE PetShopDB;
        PRINT 'Dropped incomplete database';
    END
    
    -- Restore database from backup
    RESTORE DATABASE PetShopDB 
    FROM DISK = '/var/opt/mssql/backup/PetShopDB.bak'
    WITH MOVE 'PetShopDB' TO '/var/opt/mssql/data/PetShopDB.mdf',
         MOVE 'PetShopDB_log' TO '/var/opt/mssql/data/PetShopDB_log.ldf',
         REPLACE, RECOVERY;
    
    PRINT 'Database restored successfully from backup';
END
ELSE
BEGIN
    PRINT 'Database already exists with data, skipping restore';
END
GO
