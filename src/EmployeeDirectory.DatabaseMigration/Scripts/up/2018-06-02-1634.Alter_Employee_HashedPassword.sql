ALTER TABLE [dbo].[Employee] ADD
    [HashedPassword] NVARCHAR(100) NULL;
GO

DECLARE @Austin INT;
SET @Austin = 1;

-- Create initial employee with username 'admin' and password 'password'.
INSERT INTO [dbo].[Employee] ([Email], [HashedPassword], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'admin@example.com',
    '$2a$10$gcEzk9UTkSPeAszrKkhB6ex9n9l0yBi4bxLQ/xb2meck/zxaFy.SC',
    'System',
    'Administrator',
    'System Administrator',
    @Austin,
    null
);

-- Ensure that any existing employees can log in with initial password 'password'.
UPDATE [dbo].[Employee] SET
    [HashedPassword] = '$2a$10$gcEzk9UTkSPeAszrKkhB6ex9n9l0yBi4bxLQ/xb2meck/zxaFy.SC'
WHERE [HashedPassword] IS NULL;

ALTER TABLE [dbo].[Employee] ALTER COLUMN
    [HashedPassword] NVARCHAR(100) NOT NULL;