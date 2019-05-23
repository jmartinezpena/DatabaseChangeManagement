CREATE TABLE [dbo].[EmployeeRole]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_EmployeeRole_Id] DEFAULT NEWSEQUENTIALID(),
    [EmployeeId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_EmployeeRole] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_EmployeeRole_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([Id]),
    CONSTRAINT [FK_EmployeeRole_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id])
);