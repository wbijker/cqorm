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
    public class QueryBuilder
    {
        public T Populate<T>(Func<T> aa)
        {
            return aa.Invoke();
        }

        public string Generate(SelectQuery select)
        {
            Populate(() => new { Name = "Sir" });

            // string sql = "SELECT ";
            // sql += string.Join(", ", select.Fields.Select(f => $"{f.Alias}.{f.Name}"));
            // sql += " FROM ";
            // if (!String.IsNullOrEmpty(select.Source.TableSource))
            // {
            //     sql += $"{select.Source.TableSource} {select.Source.Alias}";
            // }
            // else
            // {
            //     sql += $"({Generate(select.Source.Source)}) {select.Source.Alias}";
            // }
            // return sql;
            return "";
        }
    }

        
    public abstract class From
    {
        public string Alias { get; set; }
    }

    public class FromTable : From
    {
        public string Table { get; set; }
    }

    public class SelectQuery : From
    {
        // SELECT
        public List<Field> Fields { get; set; }
        // JOIN [SOURCE] on [ConditionalExpression...]
        public QueryJoin Join { get; set; }
        public ConditionalExpression Where { get; set; }
        // WHERE [ConditionalExpression...] 
        // ORDER BY [FieldName...] ASC/DESC
        // GROUP BY [FieldName...]
        // HAVING [HavingExpression] 


        // public QueryJoin Join { get; set; }
        // // GROUP BY field1, field2, field3
        // public QueryField[] GroupBy { get; set;}


        // public WhereExpression Where { get; set; }
    }

    public class QueryJoin
    {
        public SelectQuery Source { get; set; }
        public ConditionalExpression On { get; set; }
    }

    public class ConditionalExpression
    {
        public Field Left { get; set; }
        public string Operator { get; set; }
        public Field Right { get; set; }
    }


    // a + b == 10
    // func(a)
    // a.name == 'string'
    // a or b
    // a and b
}