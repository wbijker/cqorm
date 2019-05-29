using System;

namespace cqorm
{
    public class ScissorsEntry
    {
        public int StationId { get; set; }
        public int ScissorsId { get; set; }
        public DateTime Entry { get; set; }
    }
    
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}