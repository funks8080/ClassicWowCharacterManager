﻿CREATE TABLE [dbo].[Character]
(
	[Name] NVARCHAR(15) NOT NULL PRIMARY KEY, 
	[Faction] NVARCHAR(50) NOT NULL, 
	[Race] NVARCHAR(50) NOT NULL, 
	[Class] NVARCHAR(50) NOT NULL, 
	[Level] INT NOT NULL
)