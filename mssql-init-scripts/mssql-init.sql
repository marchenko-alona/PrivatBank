IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PrivatBankDB')
BEGIN
    CREATE DATABASE PrivatBankDB;
END;

GO

USE PrivatBankDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U')
BEGIN
    CREATE TABLE Orders (
        Id INT IDENTITY PRIMARY KEY,
        ClientId NVARCHAR(50) NOT NULL,
        ClientIp NVARCHAR(50) NULL,
        DepartmentAddress NVARCHAR(255) NOT NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        Currency NVARCHAR(10) NOT NULL,
        Status int DEFAULT 0 NOT NULL,
    );
END;
GO

CREATE PROCEDURE insert_order
    @ClientId NVARCHAR(50),
    @DepartmentAddress NVARCHAR(255),
    @Amount DECIMAL(18, 2),
    @Currency NVARCHAR(10),
    @ClientIp NVARCHAR(255) = NULL, 
    @RequestId INT OUTPUT
AS
BEGIN
    INSERT INTO Orders(ClientId, DepartmentAddress, Amount, Currency, Status, ClientIp)
    VALUES (@ClientId, @DepartmentAddress, @Amount, @Currency, 0, @ClientIp);
    
    SET @RequestId = SCOPE_IDENTITY(); 
END;

go

CREATE PROCEDURE get_orders
    @ClientId NVARCHAR(50),
    @DepartmentAddress NVARCHAR(255)
AS
BEGIN
    SELECT Id, Amount, Currency, Status, ClientIp, ClientId, DepartmentAddress
    FROM Orders
    WHERE ClientId = @ClientId AND DepartmentAddress = @DepartmentAddress;
END;



CREATE PROCEDURE get_order_by_id
    OrderId INT
AS
BEGIN
    SELECT Id, Amount, Currency, Status, ClientIp, ClientId, DepartmentAddress
    FROM Orders
    WHERE ClientId = @ClientId AND DepartmentAddress = @DepartmentAddress;
END;

