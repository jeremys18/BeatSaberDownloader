CREATE TABLE [BeatSaver].[Version]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [CoverURL] VARCHAR(200) NOT NULL, 
    [CreatedAt] DATETIME NOT NULL, 
    [DeletedAt] DATETIME NULL, 
    [DownloadURL] VARCHAR(200) NOT NULL, 
    [Feedback] NVARCHAR(MAX) NULL, 
    [Hash] VARCHAR(100) NOT NULL, 
    [Key] VARCHAR(100) NULL, 
    [PreviewURL] VARCHAR(100) NOT NULL, 
    [SageScore] SMALLINT NOT NULL, 
    [ScheduledAt] DATETIME NULL, 
    [StateId] INT NOT NULL, 
    [TestplayAt] DATETIME NULL, 
    [SongId] INT NOT NULL,

    constraint FK_Version_Song FOREIGN KEY (SongId) REFERENCES BeatSaver.Song(SongId),
    constraint FK_Version_State FOREIGN KEY (StateId) REFERENCES BeatSaver.State(Id)
)
