DECLARE
	@HrRoleId uniqueidentifier
	,@NewEmployeeId uniqueidentifier	
	,@NewEmployeeEmail nvarchar (255);

SET @NewEmployeeEmail = 'armando@example.com'

SELECT @HrRoleId = [Id] FROM [dbo].[Role] WHERE [Name] = 'Human Resources'

INSERT INTO [dbo].[Employee]
		([Email]
		,[FirstName]
		,[LastName]
		,[Title]
		,[Office]
		,[PhoneNumber]
		,[HashedPassword])
	VALUES
		(@NewEmployeeEmail
		,'Armando'
		,'Martinez'
		,'Senior Consultant 1 - Engineering'
		,4
		,'834-138-1665'
		,'$2a$10$gcEzk9UTkSPeAszrKkhB6ex9n9l0yBi4bxLQ/xb2meck/zxaFy.SC')

SELECT @NewEmployeeId = [Id] FROM [dbo].[Employee] WHERE [Email] = @NewEmployeeEmail

INSERT INTO [dbo].[EmployeeRole]
		([EmployeeId],[RoleId])
	VALUES
		(@NewEmployeeId, @HrRoleId)

