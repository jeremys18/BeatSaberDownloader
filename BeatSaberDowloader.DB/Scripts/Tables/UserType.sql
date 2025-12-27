print 'Inserting into UserType table'

set identity_insert BeatSaver.UserType on

merge BeatSaver.UserType as tgt
using ( values
	(1, 'DISCORD'),
	(2, 'SIMPLE'),
	(3, 'DUAL')
) as src(Id, Name)
on tgt.Id = src.Id
when matched then
	update set tgt.Name = src.Name
when not matched then
	insert (Id, Name) values (src.Id, src.Name);

set identity_insert BeatSaver.UserType off