CREATE TABLE [dbo].[RolePermission]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_RolePermission_Id] DEFAULT NEWSEQUENTIALID(),
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [Permission] INT NOT NULL,
    CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_RolePermission_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id])
);