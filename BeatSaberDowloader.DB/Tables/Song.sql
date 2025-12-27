CREATE TABLE [BeatSaver].[Song]
(
	[SongId] INT NOT NULL PRIMARY KEY identity, 
    [Id] VARCHAR(20) NOT NULL, 
    [Automapper] BIT NOT NULL, 
    [BlQualified] BIT NOT NULL, 
    [BlRanked] BIT NOT NULL, 
    [Bookmarked] BIT NOT NULL, 
    [CreatedAt] DATETIME NOT NULL, 
    [DeclaredAiId] INT NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [LastPublishedAt] DATETIME NOT NULL, 
    [MetadataId] INT NOT NULL, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Qualified] BIT NOT NULL, 
    [Ranked] BIT NOT NULL, 
    [StatsId] INT NOT NULL, 
    [UpdatedAt] DATETIME NOT NULL, 
    [Uploaded] DATETIME NOT NULL, 
    [UploaderId] INT NOT NULL,

    constraint FK_Song_DeclaredAi FOREIGN KEY (DeclaredAiId) references BeatSaver.DeclaredAi(Id),
    constraint FK_Song_Metadata FOREIGN KEY (MetadataId) references BeatSaver.Metadata(Id),
    constraint FK_Song_Stats FOREIGN KEY (StatsId) references BeatSaver.Stats(Id),
    constraint FK_Song_Uploader FOREIGN KEY (UploaderId) references BeatSaver.[User](UserId)
)
