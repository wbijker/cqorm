using System;
using System.Collections.Generic;

namespace cqorm
{
    public class App
    {
        public void Selects()
        {
             // Select all
            // new QuerySource<ScissorsEntry>()
            //     .FetchSingle();

            // // Select all 
            // new QuerySource<ScissorsEntry>()
            //     .Select(s => s)
            //     .FetchSingle();

            // // Select fields
            // new QuerySource<ScissorsEntry>()
            //     .Select(s => new {
            //         Bat = s.Battery,
            //         Lat = s.Lat,
            //         Long = s.Long
            //     })
            //     .FetchSingle();

            // // select fields with math expressions
            // new QuerySource<ScissorsEntry>()
            //     .Select(s => new {
            //         Bat2 = s.Battery * 2,
            //         LongLat = s.Long + s.Lat
            //     })
            //     .FetchSingle();
            
            // s.Battery -> NamedField
            //     .ToString -> MethodCall
            //         .ToString -> MethodCall with arguments (FieldConstant, FieldConstant)

            // substr(cast(s.Battery as text), 0, 2)
            
            // string expression
            new QuerySource<ScissorsEntry>()
                .Select(s => new {
                    Bat2 = s.Battery.ToString().Substring(0, 2),
                    LongLat = s.Long.ToString() + '-' + s.Lat.ToString()
                })
                .FetchSingle();

            // constant selections

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
                })
                .InnerJoin(
                    new QuerySource<ScissorsEntry>()
                    .GroupBy(s => s.StationId)
                    .Select(e => new {
                        StationId = e.Key,
                        Entries = e.Count()
                    }),(scissors, entries) => scissors.StationId == entries.StationId)
                .Select((scissor, entires) => new {
                    Scissors = scissor.Scissors,
                    Entires = entires.Entries
                })
                .FetchSingle();
        }

        public void Run()
        {
            Selects();
            // Simple();
            // ScissorTest();
            return;
        }
    }
}