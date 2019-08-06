CREATE TABLE [dbo].[FactionRaceClass]
(
	[Faction] NVARCHAR(50) NOT NULL , 
	[Race] NVARCHAR(50) NOT NULL, 
	[Class] NVARCHAR(50) NOT NULL, 
	PRIMARY KEY ([Faction], [Race], [Class])
)
