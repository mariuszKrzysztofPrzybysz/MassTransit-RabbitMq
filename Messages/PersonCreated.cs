using System;

namespace Messages
{
    public class PersonCreated
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
