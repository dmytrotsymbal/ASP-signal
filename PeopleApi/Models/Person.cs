﻿namespace PeopleApi.Models
{
    public class Person
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
