namespace cqorm
{
    public class ScissorEnty
    {
        public int Id { get; set; }
        public int StationId { get; set; }
        public int ScissorId { get; set; }
    }

    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Scissor
    {
        public int Id { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Desc { get; set; }
    }
}