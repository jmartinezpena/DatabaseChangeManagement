INSERT INTO [dbo].[Role] ([Name]) VALUES ('System Administrator');
INSERT INTO [dbo].[Role] ([Name]) VALUES ('Human Resources');
INSERT INTO [dbo].[Role] ([Name]) VALUES ('Manager');

DECLARE @ManageSecurity INT;
SET @ManageSecurity = 1;

DECLARE @RegisterEmployees INT;
SET @RegisterEmployees = 2;

DECLARE @DeleteEmployees INT;
SET @DeleteEmployees = 3;

DECLARE @EditEmployees INT;
SET @EditEmployees = 4;

DECLARE @SystemAdministratorRoleId UNIQUEIDENTIFIER;
SELECT @SystemAdministratorRoleId = [Id] FROM [dbo].[Role] WHERE [Name] = 'System Administrator';

DECLARE @HumanResourcesRoleId UNIQUEIDENTIFIER;
SELECT @HumanResourcesRoleId = [Id] FROM [dbo].[Role] WHERE [Name] = 'Human Resources';

DECLARE @ManagerRoleId UNIQUEIDENTIFIER;
SELECT @ManagerRoleId = [Id] FROM [dbo].[Role] WHERE [Name] = 'Manager';

DECLARE @AdminEmployeeId UNIQUEIDENTIFIER;
SELECT @AdminEmployeeId = [Id] FROM [dbo].[Employee] WHERE [Email] = 'admin@example.com';

IF (@AdminEmployeeId IS NULL)
BEGIN
    -- Create initial admin employee with password 'password'.
    INSERT INTO [dbo].[Employee] ([Email], [HashedPassword], [FirstName], [LastName], [Title], [Office], [PhoneNumber])
    VALUES (
        'admin@example.com',
        '$2a$10$gcEzk9UTkSPeAszrKkhB6ex9n9l0yBi4bxLQ/xb2meck/zxaFy.SC',
        'System',
        'Administrator',
        'System Administrator',
        1, -- Austin
        null
    );

    SELECT @AdminEmployeeId = [Id] FROM [dbo].[Employee] WHERE [Email] = 'admin@example.com';
END

-- System Administrators can do everything.
INSERT INTO [dbo].[RolePermission] ([RoleId], [Permission]) VALUES (@SystemAdministratorRoleId, @ManageSecurity);
INSERT INTO [dbo].[RolePermission] ([RoleId], [Permission]) VALUES (@SystemAdministratorRoleId, @RegisterEmployees);
INSERT INTO [dbo].[RolePermission] ([RoleId], [Permission]) VALUES (@SystemAdministratorRoleId, @DeleteEmployees);
INSERT INTO [dbo].[RolePermission] ([RoleId], [Permission]) VALUES (@SystemAdministratorRoleId, @EditEmployees);

-- Human Resources can do everything except manage security.
INSERT INTO [dbo].[RolePermission] ([RoleId], [Permission]) VALUES (@HumanResourcesRoleId, @RegisterEmployees);
INSERT INTO [dbo].[RolePermission] ([RoleId], [Permission]) VALUES (@HumanResourcesRoleId, @DeleteEmployees);
INSERT INTO [dbo].[RolePermission] ([RoleId], [Permission]) VALUES (@HumanResourcesRoleId, @EditEmployees);

-- Managers can edit existing employee records.
INSERT INTO [dbo].[RolePermission] ([RoleId], [Permission]) VALUES (@ManagerRoleId, @EditEmployees);

-- The initial 'admin' employee is a System Administrator.
INSERT INTO [dbo].[EmployeeRole] ([EmployeeId], [RoleId]) VALUES (@AdminEmployeeId,	@SystemAdministratorRoleId);