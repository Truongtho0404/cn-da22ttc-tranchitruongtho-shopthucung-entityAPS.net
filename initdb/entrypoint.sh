#!/bin/bash

# Start SQL Server in background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
echo "Waiting for SQL Server to start..."
sleep 30

# Check if database needs restore
echo "Checking if database exists..."
DB_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name='PetShopDB'" -h -1 | tr -d '[:space:]')

if [ "$DB_EXISTS" = "0" ]; then
    echo "Database does not exist. Restoring from backup..."
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "RESTORE DATABASE PetShopDB FROM DISK = '/var/opt/mssql/backup/PetShopDB.bak' WITH MOVE 'PetShopDB' TO '/var/opt/mssql/data/PetShopDB.mdf', MOVE 'PetShopDB_log' TO '/var/opt/mssql/data/PetShopDB_log.ldf', REPLACE;"
    echo "Database restored successfully!"
else
    # Check if database has the Pets table (main indicator of data)
    PETS_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM PetShopDB.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Pets'" -h -1 2>/dev/null | tr -d '[:space:]')
    
    if [ "$PETS_EXISTS" = "0" ] || [ -z "$PETS_EXISTS" ]; then
        echo "Database exists but has no data tables. Dropping and restoring..."
        /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "ALTER DATABASE PetShopDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE PetShopDB; RESTORE DATABASE PetShopDB FROM DISK = '/var/opt/mssql/backup/PetShopDB.bak' WITH MOVE 'PetShopDB' TO '/var/opt/mssql/data/PetShopDB.mdf', MOVE 'PetShopDB_log' TO '/var/opt/mssql/data/PetShopDB_log.ldf', REPLACE;"
        echo "Database restored successfully!"
    else
        echo "Database already exists with data. Skipping restore."
    fi
fi

# Keep container running
wait
