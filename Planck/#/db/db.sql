DROP TABLE IF EXISTS [PomoTasks];

-- tasks table
CREATE TABLE [PomoTasks]
(
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    [Name] NVARCHAR(160) NOT NULL,
    [StartedDate] DATETIME NOT NULL,
    [Breaks] INTEGER NOT NULL,
    [DurationS] INTEGER NOT NULL,
    [BreaksDurationS] INTEGER NOT NULL
); 
