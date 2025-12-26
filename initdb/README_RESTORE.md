# Hướng dẫn restore database lần đầu

## Cách 1: Sử dụng SQL Server Management Studio (SSMS)
1. Kết nối đến SQL Server: `localhost,1433`
   - User: `sa`
   - Password: `YourStrong@Passw0rd`

2. Right-click vào **Databases** → **Restore Database**

3. Chọn **Device** → Browse → Add → Chọn file `PetShopDB.bak`

4. Click **OK** để restore

## Cách 2: Sử dụng Docker command

```bash
# Copy backup file vào container
docker cp initdb/PetShopDB.bak petshop_sqlserver:/var/opt/mssql/backup/

# Restore database
docker exec -it petshop_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -C -Q "RESTORE DATABASE PetShopDB FROM DISK = '/var/opt/mssql/backup/PetShopDB.bak' WITH MOVE 'PetShopDB' TO '/var/opt/mssql/data/PetShopDB.mdf', MOVE 'PetShopDB_log' TO '/var/opt/mssql/data/PetShopDB_log.ldf', REPLACE;"
```

## Cách 3: Tự động với script

```bash
# Chạy script restore (đã bao gồm trong docker-compose)
docker exec -it petshop_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -C -i /docker-entrypoint-initdb.d/restore-database.sql
```

## Kiểm tra database đã restore

```bash
docker exec -it petshop_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -C -Q "SELECT name FROM sys.databases; SELECT COUNT(*) as PetCount FROM PetShopDB.dbo.Pets;"
```

Nếu thấy database `PetShopDB` và có dữ liệu là thành công!
