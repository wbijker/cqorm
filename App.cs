using System;

namespace cqorm
{
    public class App
    {
        private void QueryBuilder()
        {
            var q = new QueryBuilder();
            var s = new QuerySource { TableSource = "Entries", Alias = "s" };

            var sel = new QuerySelect();

            sel.Fields = new QueryField[2]
            {
                new QueryField { Name = "StationId", Parent = sel },
                new QueryField { Name = "EntryDate", Parent = sel },
            };
            sel.Source = s;

            string sql = q.Generate(sel);
            Console.WriteLine(sql);
        }

        private void ScissorTest()
        {
            var ScissorEntries = new DataSource<ScissorEnty>("ScissorEntry");
            var Stations = new DataSource<Station>("Station");

            var a = ScissorEntries
                .GroupBy(e => e.StationId)
                .Select(e => new
                {
                    StationId = e.Key,
                    Scissors = e.Distinct().Count()
                });

            // var b = ScissorEntries
            //     .GroupBy(s => s.StationId)
            //     .Select(s=> new {
            //         StationId = s.Key,
            //         Entries = s.Count()
            //     });

            // var res = a
            // .InnerJoin(b, (s,e) => s.StationId == e.StationId)
            // .InnerJoin(Stations, (s, u) => s.Right.StationId == u.Id)
            // .Select(n => new {
            //     UId = n.Right.Id,
            //     Scissors = n.Left.Left.Scissors,
            //     Entries = n.Left.Right.Entries
            // })
            // .FetchArray();
        }

        public void Run()
        {
            // QueryBuilder();
            ScissorTest();
            return;

            // var db = new Database();

            // var res = db.Users
            //     .Where(u => u.Id == 3)
            //     .Select(u => u.Name)
            //     .FetchSingle();

            // // SubQuery
            // // Select id * 2 from (select top 1 id from users where id == 3)
            //  var bb = (db.Users
            //     .Where(u => u.Id == 3)
            //     .Select(u => u.Id))
            //     .Select(a => a * 2)
            //     .FetchSingle();

            // // Updates
            // db.Users
            //     .Update(u => u.Age, 11)
            //     .Update(u => u.Name, "NewName")
            //     .Where(u => u.Id == 10);

            // db.Users
            //     .Update(u => u.Age, u => u.Age + 1)
            //     .Where(u => u.Id == 10);

            // // Delete
            // db.Users.Delete(u => u.Age < 10);

            // // inner join
            // db.Users
            //     .InnerJoin(db.Products, (u,p) => u.Id == p.UserId)
            //     .Select(j => j.Right)
            //     .FetchArray();

            // // group by
            // // Select user_id, count(p.id) from usres inner join products on user.id == products.user_id group by user_id
            // var qq = db.Users
            //     .InnerJoin(db.Products, (u,p) => u.Id == p.UserId)
            //     .GroupBy(j => j.Left.Id)
            //     .Select(s => new {
            //         UserId = s.Key,
            //         Count = s.Count()
            //     }).FetchArray();



            // .InnerJoin(db.Products, (a, p) => a.Count == p.Id)
            // .Select(j => new {
            //     Count = j.Left.Count,
            //     Pid = j.Right.Desc
            // });
        }
    }
}