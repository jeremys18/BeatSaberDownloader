CREATE TABLE [BeatSaver].[MetaData]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [BPM] DECIMAL(18, 8) NOT NULL, 
    [Duration] INT NOT NULL, 
    [LevelAuthorName] NVARCHAR(1000) NOT NULL, 
    [SongAuthorName] NVARCHAR(1000) NULL, 
    [SongName] NVARCHAR(300) NOT NULL, 
    [SongSubName] NVARCHAR(max) NULL
)
