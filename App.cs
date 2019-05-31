using System;
using System.Collections.Generic;

namespace cqorm
{
    public class App
    {
        private void QueryBuilder()
        {

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
    
            var a = new DataSource<ScissorsEntry>()
                .GroupBy(s => s.StationId)
                .Select(g => new {
                    StationId = g.Key,
                    Scissors = g.CountDistinct(f => f.StationId)
                });

            var b = new DataSource<ScissorsEntry>()
                .GroupBy(s => s.StationId)
                .Select(e => new {
                    StationId = e.Key,
                    Entries = e.Count()
                });

            a.InnerJoin(b, (l,r) => l.StationId == r.StationId)
                .Select(l => new {
                    Scissors = l.Left.Scissors,
                    Entries = l.Right.Entries
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