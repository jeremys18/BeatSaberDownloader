print 'Inserting into DeclaredAI table'

set identity_insert BeatSaver.DeclaredAI on

merge BeatSaver.DeclaredAI as tgt
using( values 
	(1, 'Admin'),
	(2, 'Uploader'),
	(3, 'SageScore'),
	(4, 'None')
) as src(Id, Name)
on tgt.Id = src.Id
when matched then
	update set tgt.Name = src.Name
when not matched then
	insert (Id, Name) values (src.Id, src.Name);

set identity_insert BeatSaver.DeclaredAI off