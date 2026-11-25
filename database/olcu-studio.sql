-- ölçü studio paneli veri tabanı: OlcuStudioDb'yi sıfırdan kurar (varsa silinir).
-- Çalıştırma: sqlcmd -S .\SQLEXPRESS -E -f 65001 -i database\olcu-studio.sql

USE [master];
GO
IF DB_ID('OlcuStudioDb') IS NOT NULL
BEGIN
    ALTER DATABASE [OlcuStudioDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [OlcuStudioDb];
END
GO
CREATE DATABASE [OlcuStudioDb] COLLATE Turkish_CI_AS;
GO
USE [OlcuStudioDb];
GO

CREATE TABLE [dbo].[Users](
    [UserId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(200) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL
);
GO

CREATE TABLE [dbo].[Projects](
    [ProjectId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProjectName] NVARCHAR(150) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Stage] TINYINT NOT NULL DEFAULT (1) CHECK ([Stage] BETWEEN 1 AND 5),
    [Category] TINYINT NOT NULL DEFAULT (1) CHECK ([Category] BETWEEN 1 AND 3),
    [District] NVARCHAR(50) NULL,
    [CustomerId] INT NOT NULL,
    [CreatedAt] DATETIME NULL DEFAULT (GETDATE()),
    [EndDate] DATETIME NULL,
    CONSTRAINT [FK_Projects_Users] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Users]([UserId])
);
GO

-- Demo verisi uygulama Development ortamında açılırken eklenir (Data/DemoSeeder.cs).
