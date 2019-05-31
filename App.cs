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

            // select 
            //     s.Id, s.UID,  a.Scissors, b.Entries 
            // from 
            // (
            //     select StationId, count (distinct ScissorsId) as Scissors from ScissorsEntries
            //     group by StationId
            //     ) as a
            
            // inner join 
            // (
            //     select StationId, count(*) as Entries from ScissorsEntries
            //     group by StationId
            // ) as b
            
            // on a.StationId = b.StationId
            // inner join Stations s 
            // on s.Id = b.StationId
            // where s.Active = true
    
            new DataSource<ScissorsEntry>()
                .GroupBy(s => s.StationId)
                .Select(g => new {
                    StationId = g.Key,
                    Scissors = g.CountDistinct(f => f.StationId)
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