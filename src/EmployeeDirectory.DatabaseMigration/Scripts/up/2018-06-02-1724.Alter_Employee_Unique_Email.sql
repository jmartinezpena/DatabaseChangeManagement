ALTER TABLE [dbo].[Employee] ADD
    CONSTRAINT [UQ_Employee_Email] UNIQUE (Email);