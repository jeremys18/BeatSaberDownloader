CREATE TABLE [BeatSaver].[Version]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [CoverURL] VARCHAR(100) NOT NULL, 
    [CreatedAt] DATETIME NOT NULL, 
    [DownloadURL] VARCHAR(100) NOT NULL, 
    [Feedback] NVARCHAR(MAX) NOT NULL, 
    [Hash] VARCHAR(50) NOT NULL, 
    [Key] VARCHAR(50) NOT NULL, 
    [PreviewURL] VARCHAR(100) NOT NULL, 
    [SageScore] SMALLINT NOT NULL, 
    [ScheduledAt] DATETIME NOT NULL, 
    [StateId] INT NOT NULL, 
    [TestplayAt] DATETIME NOT NULL, 
    [SongId] INT NOT NULL,

    constraint FK_Version_Song FOREIGN KEY (SongId) REFERENCES BeatSaver.Song(SongId),
    constraint FK_Version_State FOREIGN KEY (StateId) REFERENCES BeatSaver.State(Id)
)
