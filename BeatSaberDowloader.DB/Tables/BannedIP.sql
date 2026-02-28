CREATE TABLE [Server].[BannedIP]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [IP] VARCHAR(15) NOT NULL, 
    [Updated] DATETIME2 NOT NULL 
)
