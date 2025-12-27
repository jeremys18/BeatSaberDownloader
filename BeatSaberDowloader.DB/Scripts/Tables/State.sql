print 'Inserting into State table'

set identity_insert BeatSaver.State on

merge BeatSaver.State as tgt
using( values 
	(1, 'Uploaded'),
	(2, 'Testplay'),
	(3, 'Published'),
	(4, 'Feedback'),
	(5, 'Scheduled')
) as src(Id, Name) 
on tgt.Id = src.Id
when matched then
	update set tgt.Name = src.Name
when not matched then
	insert (Id, Name) values (src.Id, src.Name);

set identity_insert BeatSaver.State off