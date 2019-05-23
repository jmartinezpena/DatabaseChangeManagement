namespace EmployeeDirectory.Features.Employee
{
    using System;
    using System.Linq;
    using AutoMapper;
    using MediatR;
    using Model;

    public class EmployeeIndex
    {
        public class Query : IRequest<ViewModel[]> { }

        public class ViewModel
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Title { get; set; }
            public Office Office { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class QueryHandler : RequestHandler<Query, ViewModel[]>
        {
            private readonly DirectoryContext _database;
            private readonly IMapper _mapper;

            public QueryHandler(DirectoryContext database, IMapper mapper)
            {
                _database = database;
                _mapper = mapper;
            }

            protected override ViewModel[] Handle(Query request)
            {
                return _database.Employee
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .Select(_mapper.Map<ViewModel>)
                    .ToArray();
            }
        }
    }
}