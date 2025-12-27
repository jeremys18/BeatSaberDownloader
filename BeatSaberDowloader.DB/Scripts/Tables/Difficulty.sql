print 'Inserting into Difficulty2 table'

set identity_insert BeatSaver.Difficulty2 ON;

merge BeatSaver.Difficulty2 as tgt
using( values 
	(1, 'Easy'),
	(2, 'Normal'),
	(3, 'Hard'),
	(4, 'Expert'),
	(5, 'ExpertPlus')
)as src (Id, Name)
on tgt.Id = src.Id
when matched then
	update set Name = src.Name
when not matched then
	insert (Id, Name)
	values (src.Id, src.Name);

set identity_insert BeatSaver.Difficulty2 OFF;