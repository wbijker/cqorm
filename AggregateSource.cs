using System;
using System.Collections.Generic;
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

        // .Select(e => e.Count()) || e => e.Key
            // .Select(e => new {
            //     Key = e.Key,
            //     Count = e.Count()
            // })

        public DataSource<P> Select<P>(Expression<Func<AggregateSource<T, Q>, P>> select)
        {
            // Can only select grouped items and / or aggregate functions
            
            var group = _query.From as FromGroup;
            if (group == null)
            {
                throw new Exception("Source is not grouped");
            }

            // When you select group.Key you're refering to _query.GroupBy
            
            var parse = new ExpressionParser(_query);
            var field = parse.ParseField(select);
            if (field is FieldName)
            {
                _query.Fields = new List<Field> { field };
            }
            if (field is FieldList list)
            {
                _query.Fields = list.Fields;
            }
       
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