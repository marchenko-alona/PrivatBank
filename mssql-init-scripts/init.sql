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
        DepartmentAddress NVARCHAR(255) NOT NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        Currency NVARCHAR(10) NOT NULL,
        Status int DEFAULT 0 NOT NULL,
    );
END;
GO