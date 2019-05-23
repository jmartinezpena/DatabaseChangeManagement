﻿namespace EmployeeDirectory.Tests.Features.Employee
{
    using System.Linq;
    using System.Threading.Tasks;
    using EmployeeDirectory.Features.Employee;
    using EmployeeDirectory.Model;
    using Shouldly;
    using static Testing;

    public class EmployeeIndexTests
    {
        public async Task ShouldGetAllEmployeesSortedByName()
        {
            var patrickEmail = SampleEmail();
            var patrick = await Register(x =>
            {
                x.Email = patrickEmail;
                x.FirstName = "Patrick";
                x.LastName = "Zed";
                x.SportTeam = "America";
                x.Title = "Principal Consultant";
                x.Office = Office.Austin;
                x.PhoneNumber = "555-123-0001";
            });

            var alonsoEmail = SampleEmail();
            var alonso = await Register(x =>
            {
                x.Email = alonsoEmail;
                x.FirstName = "Alonso";
                x.LastName = "Smith";
                x.SportTeam = "Chivas";
                x.Title = "Senior Consultant";
                x.Office = Office.Austin;
                x.PhoneNumber = "555-123-0002";
            });

            var sharonEmail = SampleEmail();
            var sharon = await Register(x =>
            {
                x.Email = sharonEmail;
                x.FirstName = "Sharon";
                x.LastName = "Smith";
                x.SportTeam = "Pumas";
                x.Title = "Principal Consultant";
                x.Office = Office.Dallas;
                x.PhoneNumber = "555-123-0003";
            });

            var expectedIds = new[] { patrick.Id, alonso.Id, sharon.Id };

            var query = new EmployeeIndex.Query();

            var result = await Send(query);

            result.Length.ShouldBe(Count<Employee>());

            result
                .Where(x => expectedIds.Contains(x.Id))
                .ShouldMatch(
                    new EmployeeIndex.ViewModel
                    {
                        Id = alonso.Id,
                        FirstName = "Alonso",
                        LastName = "Smith",
                        SportTeam = "Chivas",
                        Title = "Senior Consultant",
                        Office = Office.Austin,
                        Email = alonsoEmail,
                        PhoneNumber = "555-123-0002"
                    },
                    new EmployeeIndex.ViewModel
                    {
                        Id = sharon.Id,
                        FirstName = "Sharon",
                        LastName = "Smith",
                        SportTeam = "Pumas",
                        Title = "Principal Consultant",
                        Office = Office.Dallas,
                        Email = sharonEmail,
                        PhoneNumber = "555-123-0003"
                    },
                    new EmployeeIndex.ViewModel
                    {
                        Id = patrick.Id,
                        FirstName = "Patrick",
                        LastName = "Zed",
                        SportTeam = "America",
                        Title = "Principal Consultant",
                        Office = Office.Austin,
                        Email = patrickEmail,
                        PhoneNumber = "555-123-0001"
                    }
                );
        }
    }
}