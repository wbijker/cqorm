// Select coalesce( datetime('now') > null, 0), 'str' == 'aa', (select 10) + 10

//  SELECT
//     [Field...]
// FROM
//     [SOURCE]
// JOIN [SOURCE] on [ConditionalExpression...]
// WHERE [ConditionalExpression...] 
// ORDER BY [FieldName...] ASC/DESC
// GROUP BY [FieldName...]
// HAVING [HavingExpression] 

// ConditionalExpression: [Field] [Operator] [Field]
// Field: FieldConstant, FieldName, FieldAggregate, FieldRowFunction, FieldExpression
// FieldExpression: Func([Field], [Field] [Operator] [Field]
// HavingExpression: [FieldAggregate] [Operator] [Field]

// FieldSelectExpression: FieldConstant, FieldName, FieldAggregate, FieldRowFunction, FieldOperator, FieldBoolean, FieldExpression
// FieldWhere: FieldBoolean 


// FieldMath
// FieldFunction

using System;
using System.Collections.Generic;
using System.Linq;

namespace cqorm
{
    public abstract class From
    {
        public Type Type { get; set; }
        public string Alias { get; set; }

        public From(Type type, string alias)
        {
            Type = type;
            Alias = alias;
        }
    }

    public class FromTable : From
    {
        public FromTable(Type type, string alias, string table) : base(type, alias)
        {
            Table = table;
        }

        public string Table { get; set; }
    }

    public class FromGroup : From
    {
        public FromGroup(Type type, string alias, From from) : base(type, alias)
        {
            From = from;
        }

        public From From { get; set; }
    }

    public class FromSubQuery : From
    {
        public FromSubQuery(Type type, string alias, SelectQuery query) : base(type, alias)
        {
            Query = query;
        }
        public SelectQuery Query { get; set; }
    }

    public class SelectQuery
    {
        public List<Field> Fields { get; set; }
        public QueryJoin Join { get; set; } 
        public FieldMath Where { get; set; }
        public From From { get; set; }
        public List<FieldName> GroupBy { get; set; }
    }

    public class QueryJoin
    {
        public From Left { get; set; }
        public From Right { get; set; }
        public FieldMath On { get; set; }
    }
}