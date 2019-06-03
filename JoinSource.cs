using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace cqorm
{
    // Combined left and right QuerySource
    public class JoinSource<L, R>: BaseSource
    {
        public JoinSource(SelectQuery query) : base(query)
        {
        }

        // The momemtn you select something the source changes
        public QuerySource<C> Select<C>(Expression<Func<L, R, C>> select)
        {
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
            return new QuerySource<C>(_query);
        }
    }
}