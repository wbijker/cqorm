using System;
using System.Linq.Expressions;

namespace cqorm
{
    public class AggregateSource<T, Q> : BaseSource<Q>
    {
        public AggregateSource(SelectQuery query) : base(query)
        {
        }

        public T Key { get; set; }
        // public Aggregate<Q> Aggregate { get; set; }

        public DataSource<P> Select<P>(Expression<Func<AggregateSource<T, Q>, P>> select)
        {
            var parse = new ExpressionParser(_query);
            var field = parse.ParseField(select);
            if (field is FieldName)
            {
                // Single
            }
            if (field is FieldList)
            {
                // Multiple
            }

            // .Select(e => new {
            //         StationId = e.Key,
            //         Items = e.Count()
            //     })

            // ExpParser.ProcessExpression(select);
            return new DataSource<P>(_query);
        }

        public AggregateSource<T, Q> Distinct()
        {
            return this;
        }

        public int Count()
        {
            return 0;
        }

        public Q Max()
        {
            return default(Q);
        }

        public Q Min()
        {
            return default(Q);
        }
    }
}