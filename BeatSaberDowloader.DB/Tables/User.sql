CREATE TABLE [BeatSaver].[User]
(
	[UserId] INT NOT NULL PRIMARY KEY identity, 
    [Id] INT NOT NULL, 
    [Admin] BIT NOT NULL, 
    [Avatar] NVARCHAR(50) NOT NULL, 
    [Curator] BIT NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [PlaylistUrl] VARCHAR(100) NOT NULL, 
    [SeniorCurator] BIT NOT NULL, 
    [UserTypeId] INT NOT NULL,

    CONSTRAINT FK_User_UserType FOREIGN KEY (UserTypeId) REFERENCES BeatSaver.UserType(Id)
)
