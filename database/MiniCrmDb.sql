-- Mini CRM veri tabanı (okul teslimindeki şema)
-- Çalıştırma: sqlcmd -S .\SQLEXPRESS -E -f 65001 -i database\MiniCrmDb.sql
-- Örnek veriler uydurmadır.

USE [master];
GO
IF DB_ID('MiniCrmDb') IS NOT NULL
BEGIN
    RAISERROR('MiniCrmDb zaten var; betik mevcut veri tabanına dokunmadan durdu.', 20, 1) WITH LOG;
END
GO
CREATE DATABASE [MiniCrmDb];
GO
USE [MiniCrmDb];
GO

CREATE TABLE [dbo].[Users](
    [UserId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
    [Password] NVARCHAR(100) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL
);
GO

CREATE TABLE [dbo].[Projects](
    [ProjectId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProjectName] NVARCHAR(150) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Status] NVARCHAR(50) NULL DEFAULT ('Başlamadı'),
    [CustomerId] INT NOT NULL,
    [CreatedAt] DATETIME NULL DEFAULT (GETDATE()),
    [EndDate] DATETIME NULL,
    CONSTRAINT [FK_Projects_Users] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Users]([UserId])
);
GO

INSERT INTO [dbo].[Users] ([FirstName], [LastName], [Email], [Password], [Role]) VALUES
(N'Yönetici', N'Hesabı', N'admin@ornek.test', N'admin123', N'Admin'),
(N'Ayşe', N'Yılmaz', N'ayse@ornek.test', N'ayse123', N'Customer'),
(N'Mehmet', N'Kaya', N'mehmet@ornek.test', N'mehmet123', N'Customer'),
(N'Zehra', N'Demir', N'zehra@ornek.test', N'zehra123', N'Customer'),
(N'Ali', N'Şahin', N'ali@ornek.test', N'ali123', N'Customer');
GO

INSERT INTO [dbo].[Projects] ([ProjectName], [Description], [Status], [CustomerId], [CreatedAt], [EndDate]) VALUES
(N'Mobil Takip Uygulaması', N'Sipariş takibi için mobil uygulama', N'Kodlanıyor', 2, '2026-04-20', '2026-07-15'),
(N'Kurumsal Web Sitesi', N'Beş sayfalık tanıtım sitesi', N'Tasarımda', 3, '2026-05-02', NULL),
(N'CRM Projesi', N'Müşteri ilişkileri yönetim paneli', N'Analiz Ediliyor', 4, '2026-05-10', '2026-06-30');
GO
