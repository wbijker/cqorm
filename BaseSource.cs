using System;
using System.Linq;

namespace cqorm
{
    public abstract class BaseSource<T>
    {
        protected SelectQuery _query;

        public BaseSource(SelectQuery query)
        {
            _query = query;
        }

        public BaseSource()
        {
            _query = new SelectQuery();
            _query.From = new FromTable
            {
                Table = typeof(T).Name,
                Alias = "u"
            };
        }

        public T FetchSingle()
        {
            // If not select was spesified select all from original source
            if (_query.Fields == null)
            {
                var props = typeof(T).GetProperties();
                _query.Fields = props
                    .Select(p => (Field)new FieldName(p.Name, _query.From))
                    .ToList();
            }
                
                
            ISQLDriver driver = new SQLLiteDriver();
            Console.WriteLine("Fetch Single");
            Console.WriteLine(driver.Generate(_query));
            return default(T);
        }

        // Distinct on all columns
        public BaseSource<T> Distinct()
        {
            return this;
        }

        public T[] FetchArray()
        {
            // ToList()
            return null;
        }
    }
}