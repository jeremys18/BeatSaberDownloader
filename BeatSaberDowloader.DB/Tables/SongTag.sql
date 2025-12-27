CREATE TABLE [BeatSaver].[SongTag]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [SongId] INT NOT NULL, 
    [TagId] INT NOT NULL,

    constraint FK_SongTag_Song foreign key (SongId) references BeatSaver.Song(SongId),
    constraint FK_SongTag_Tag foreign key (TagId) references BeatSaver.Tag(Id)
)
