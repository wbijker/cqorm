using System;
using System.Collections.Generic;

namespace cqorm
{
    public class App
    {

        private void QueryBuilder()
        {
            // Select * from users u
            // inner join Procuts on p.user_id = u.id
            

            var query = new SelectQuery();
            query.From = new FromTable(typeof(ScissorsEntry), "s", "ScissorsEntry");
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
            // query.Join = new QueryJoin
            // {
            //     Source = new FromTable(typeof(Scissor), "c", "Scissor"),
            //     On = null
            // };

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
    
            var a = new QuerySource<ScissorsEntry>()
                .GroupBy(s => s.StationId)
                .Select(g => new {
                    StationId = g.Key,
                    Scissors = g.CountDistinct(f => f.StationId)
                });

            var b = new QuerySource<ScissorsEntry>()
                .GroupBy(s => s.StationId)
                .Select(e => new {
                    StationId = e.Key,
                    Entries = e.Count()
                });

            var join = a.InnerJoin(b, (scissors, entries) => scissors.StationId == entries.StationId)
                .Select((scissor, entires) => new {
                    Scissors = scissor.Scissors,
                    Entires = entires.Entries
                })
                .FetchSingle();
        }

        public void Run()
        {
            Simple();
            // ScissorTest();
            return;
        }
    }
}