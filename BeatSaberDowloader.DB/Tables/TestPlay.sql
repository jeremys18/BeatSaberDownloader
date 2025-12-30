CREATE TABLE [BeatSaver].[TestPlay]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [CreatedAt] DATETIME NOT NULL, 
    [Feedback] NVARCHAR(MAX) NOT NULL, 
    [FeedbackAt] DATETIME NOT NULL, 
    [UserId] INT NOT NULL, 
    [Video] NVARCHAR(100) NOT NULL, 
    [VersionId] INT NOT NULL,

    CONSTRAINT FK_TestPlay_User FOREIGN KEY (UserId) REFERENCES BeatSaver.[User](Id),
    constraint FK_TestPlay_Version FOREIGN KEY (VersionId) REFERENCES BeatSaver.Version(Id)
)
