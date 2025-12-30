print 'Inserting into Characteristic table'

set identity_insert BeatSaver.Characteristic on;

merge BeatSaver.Characteristic as tgt
using( values 
	(1, 'Standard'),
	(2, 'OneSaber'),
	(3, 'NoArrows'),
	(4, 'Rotation90Degree'),
	(5, 'Rotation360Degree'),
	(6, 'Lightshow'),
	(7, 'Lawless'),
	(8, 'Legacy'),
	(9, '360Degree'),
	(10, '90Degree')
) as src(Id, Name)
on tgt.Id = src.Id
when matched then
	update set tgt.Name = src.Name
when not matched then
	insert (Id, Name)
	values (src.Id, src.Name);

set identity_insert BeatSaver.Characteristic off;