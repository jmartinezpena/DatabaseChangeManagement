﻿namespace EmployeeDirectory.Model
{
    public class EmployeeRole : Entity
    {
        public virtual Employee Employee { get; set; }
        public virtual Role Role { get; set; }
    }
}