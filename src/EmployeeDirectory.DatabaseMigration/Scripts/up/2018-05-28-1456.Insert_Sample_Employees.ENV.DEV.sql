DECLARE @Austin INT;
SET @Austin = 1;

DECLARE @Dallas INT;
SET @Dallas = 2;

DECLARE @Houston INT;
SET @Houston = 3;

DECLARE @Monterrey INT;
SET @Monterrey = 4;

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'dustin@example.com',
    'Dustin',
    'Wells',
    'President & CEO',
    @Austin,
    '555-123-0001'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'vasudha@example.com',
    'Vasudha',
    'Prabhala',
    'Vice President of Service Delivery',
    @Austin,
    '555-123-0002'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'jimmy@example.com',
    'Jimmy',
    'Bogard',
    'Chief Architect',
    @Austin,
    '555-123-0003'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'glenn@example.com',
    'Glenn',
    'Burnside',
    'Executive Vice President, Operations and Strategy',
    @Austin,
    '555-123-0004'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'john@example.com',
    'John',
    'Sarantakes',
    'Vice President & General Manager',
    @Austin,
    '555-123-0005'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'kathy@example.com',
    'Kathy',
    'Chesner',
    'Director of Finance',
    @Austin,
    '555-123-0006'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'lola@example.com',
    'Lola',
    'Mullen',
    'Principal Consultant 1 - Client Services',
    @Austin,
    '555-123-0007'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'deran@example.com',
    'Deran',
    'Schilling',
    'Senior Director in Service Delivery',
    @Houston,
    '555-123-0008'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'javier@example.com',
    'Javier',
    'Najera',
    'Senior Consultant 1 - Engineering',
    @Monterrey,
    '555-123-0009'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'erin@example.com',
    'Erin',
    'Knight',
    'Senior Consultant 1 - Engineering',
    @Austin,
    '555-123-0010'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'chad@example.com',
    'Chad',
    'Stever',
    'Senior Consultant 3 - Engineering',
    @Dallas,
    '555-123-0011'
);

INSERT INTO [dbo].[Employee] ([Email], [FirstName], [LastName], [Title], [Office], [PhoneNumber]) VALUES (
    'patrick@example.com',
    'Patrick',
    'Lioi',
    'Principal Consultant 3 - Engineering',
    @Austin,
    '555-123-0012'
);