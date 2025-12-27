CREATE TABLE [BeatSaver].[Stats]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [Downloads] INT NOT NULL, 
    [Downvotes] INT NOT NULL, 
    [Plays] INT NOT NULL, 
    [Reviews] INT NOT NULL, 
    [Score] DECIMAL(18, 8) NOT NULL, 
    [ScoreOneDP] DECIMAL(18, 8) NOT NULL, 
    [SentimentId] INT NOT NULL, 
    [Upvotes] INT NOT NULL,

    CONSTRAINT FK_Stats_Sentiments FOREIGN KEY (SentimentId) REFERENCES BeatSaver.Sentiment(Id)
)
