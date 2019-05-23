ALTER TABLE [dbo].[Employee] ADD
    [SportTeam] nvarchar (255) null
GO

UPDATE [dbo].[Employee] SET [SportTeam] = 'Tigres'
GO