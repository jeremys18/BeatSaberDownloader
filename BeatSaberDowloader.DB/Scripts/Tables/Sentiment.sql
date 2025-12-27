print 'Inserting into Sentiment table'

set identity_insert BeatSaver.Sentiment on

merge BeatSaver.Sentiment as tgt
using( values 
	(1, 'PENDING'),
	(2, 'VERY_NEGATIVE'),
	(3, 'MOSTLY_NEGATIVE'),
	(4, 'MIXED'),
	(5, 'MOSTLY_POSITIVE'),
	(6, 'VERY_POSITIVE')
) as src(Id, Name)
on tgt.Id = src.Id
when matched then
	update set tgt.Name = src.Name
when not matched then
	insert (Id, Name) values (src.Id, src.Name);

set identity_insert BeatSaver.Sentiment off