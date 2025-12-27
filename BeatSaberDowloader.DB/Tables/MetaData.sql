CREATE TABLE [BeatSaver].[MetaData]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [BPM] DECIMAL(18, 8) NOT NULL, 
    [Duration] INT NOT NULL, 
    [LevelAuthorName] NVARCHAR(MAX) NOT NULL, 
    [SongAuthorName] NVARCHAR(MAX) NOT NULL, 
    [SongName] NVARCHAR(MAX) NOT NULL, 
    [SongSubName] NVARCHAR(MAX) NOT NULL
)
