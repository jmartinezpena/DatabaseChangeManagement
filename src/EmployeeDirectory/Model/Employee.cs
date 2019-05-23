namespace EmployeeDirectory.Model
{
    public class Employee : Entity
    {
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SportTeam { get; set; }
        public string Title { get; set; }
        public Office Office { get; set; }
        public string PhoneNumber { get; set; }
    }
}