using System;
using System.Collections.Generic;

namespace cqorm
{
    public class App
    {
        private void QueryBuilder()
        {

            var query = new SelectQuery();
            query.From = new FromTable
            {
                Table = "User",
                Alias = "u"
            };
            query.Fields = new List<Field> 
            {
                Field.Name("Username", query.From),
                Field.Name("Password", query.From),
                Field.Name("Email", query.From)
            };
            query.Where = new FieldMath(
                Field.Function(FieldFunctionType.Lower, Field.Name("Username", query.From)), 
                FieldMathOperator.Equal,
                Field.ConstantString("Willem"));

            ISQLDriver driver = new SQLLiteDriver();
            Console.WriteLine(driver.Generate(query));

        }
        public void Simple()
        {            
            new DataSource<User>()
                .Where(u => u.Name.ToLower() == "Jabo" || u.Age > 28)
                .Select(u => new
                {
                    Id = u.Id,
                    Age = u.Age
                })
                .FetchSingle();

            new DataSource<ScissorsEntry>()
                .GroupBy(e => e.StationId)
                .Select(e => new {
                    StationId = e.Key,
                    Stations = e.Count(),
                    Scissors = e.Count(a => a.ScissorsId)
                })
                .FetchSingle();
        }

        public void Run()
        {
            // QueryBuilder();
            Simple();
            // ScissorTest();
            return;
        }
    }
}