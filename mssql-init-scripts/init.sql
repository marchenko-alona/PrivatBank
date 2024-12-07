IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PrivatBankDB')
BEGIN
    CREATE DATABASE PrivatBankDB;
END;

PRINT 'Checking if database exists...';

DECLARE @sql NVARCHAR(MAX);

SET @sql = N'
USE PrivatBankDB;

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = ''Orders'' AND xtype = ''U'')
BEGIN
    CREATE TABLE Orders (
        Id INT IDENTITY PRIMARY KEY,
        ClientId NVARCHAR(50) NOT NULL,
        ClientIp NVARCHAR(50) NULL,
        DepartmentAddress NVARCHAR(255) NOT NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        Currency NVARCHAR(10) NOT NULL,
        Status INT DEFAULT 0 NOT NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = ''P'' AND name = ''insert_order'')
BEGIN
    EXEC(''
    CREATE PROCEDURE insert_order
        @request_id INT OUTPUT, 
        @client_id NVARCHAR(50), 
        @department_address NVARCHAR(255), 
        @amount DECIMAL(18, 2), 
        @currency NVARCHAR(10),
        @client_ip NVARCHAR(50) = NULL,
        @status INT = 0
    AS
    BEGIN
       INSERT INTO Orders(ClientId, DepartmentAddress, Amount, Currency, Status, ClientIp)
       VALUES (@client_id, @department_address, @amount, @currency, @status, @client_ip);
        
        SET @request_id = SCOPE_IDENTITY(); 
    END;
    '');
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = ''P'' AND name = ''get_orders'')
BEGIN
    EXEC(''
        CREATE PROCEDURE get_orders
                @client_id_param NVARCHAR(50),
                @department_address_param NVARCHAR(255)
            AS
            BEGIN
                SELECT 
                    Amount, 
                    Currency, 
                    Status, 
                    ClientIp, 
                    DepartmentAddress,
                    ClientId
                FROM Orders
                WHERE ClientId = @client_id_param AND DepartmentAddress = @department_address_param;
            END;
    '');
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = ''P'' AND name = ''get_order_by_id'')
BEGIN
    EXEC(''
    CREATE PROCEDURE get_order_by_id
        @client_id NVARCHAR(50) OUTPUT, 
        @department_address NVARCHAR(255) OUTPUT, 
        @amount DECIMAL OUTPUT, 
        @currency NVARCHAR(10) OUTPUT, 
        @status INT OUTPUT, 
        @client_ip NVARCHAR(50) OUTPUT,
        @order_id INT
        AS
        BEGIN
            SELECT 
                @client_id = ClientId, 
                @department_address = DepartmentAddress, 
                @amount = Amount, 
                @currency = Currency, 
                @status = Status, 
                @client_ip = ClientIp
            FROM Orders
            WHERE Id = @order_id;
        END;
    '');
END;
';

EXEC sp_executesql @sql;
