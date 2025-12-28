print 'Inserting into Environment table'

set identity_insert BeatSaver.Environment ON;

merge BeatSaver.Environment as tgt
using( values 
	(1, 'DefaultEnvironment'),
	(2, 'TriangleEnvironment'),
	(3, 'NiceEnvironment'),
	(4, 'BigMirrorEnvironment'),
	(5, 'KDAEnvironment'),
	(6, 'MonstercatEnvironment'),
	(7, 'CrabRaveEnvironment'),
	(8, 'DragonsEnvironment'),
	(9, 'OriginsEnvironment'),
	(10, 'PanicEnvironment'),
	(11, 'RocketEnvironment'),
	(12, 'GreenDayEnvironment'),
	(13, 'GreenDayGrenadeEnvironment'),
	(14, 'TimbalandEnvironment'),
	(15, 'FitBeatEnvironment'),
	(16, 'LinkinParkEnvironment'),
	(17, 'BTSEnvironment'),
	(18, 'KaleidoscopeEnvironment'),
	(19, 'InterscopeEnvironment'),
	(20, 'SkrillexEnvironment'),
	(21, 'BillieEnvironment'),
	(22, 'HalloweenEnvironment'),
	(23, 'GagaEnvironment'),
	(24, 'GlassDesertEnvironment'),
	(25, 'MultiplayerEnvironment'),
	(26, 'WeaveEnvironment'),
	(27, 'PyroEnvironment'),
	(28, 'EDMEnvironment'),
	(29, 'TheSecondEnvironment'),
	(30, 'LizzoEnvironment'),
	(31, 'TheWeekndEnvironment'),
	(32, 'RockMixtapeEnvironment'),
	(33, 'Dragons2Environment'),
	(34, 'Panic2Environment'),
	(35, 'QueenEnvironment'),
	(36, 'LinkinPark2Environment'),
	(37, 'TheRollingStonesEnvironment'),
	(38, 'LatticeEnvironment'),
	(39, 'DaftPunkEnvironment'),
	(40, 'HipHopEnvironment'),
	(41, 'ColliderEnvironment'),
	(42, 'BritneyEnvironment'),
	(43, 'Monstercat2Environment'),
	(44, 'MetallicaEnvironment'),
	(45, 'Halloween2Environment'),
	(46, 'GridEnvironment')
) as src (Id, Name)
on tgt.Id = src.Id
when matched then
	update set 
		tgt.Name = src.Name
when not matched then
	insert (Id, Name)
	values (src.Id, src.Name);

set identity_insert BeatSaver.Environment OFF;