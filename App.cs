using System;
using System.Collections.Generic;

namespace cqorm
{
    public class App
    {

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