using System;
using System.Collections.Generic;
using System.Linq;

namespace cqorm
{
    public class QueryBuilder : Object
    {
        public string Generate(QuerySelect select)
        {
            string sql = "SELECT ";
            sql += string.Join(", ", select.Fields.Select(f => $"{f.Parent.Source.Alias}.{f.Name}"));
            sql += " FROM ";
            if (!String.IsNullOrEmpty(select.Source.TableSource))
            {
                sql += $"{select.Source.TableSource} {select.Source.Alias}";
            }
            else
            {
                sql += $"({Generate(select.Source.Source)}) {select.Source.Alias}";
            }
            return sql;
        }
    }

    public class QueryField
    {
        public string Name { get; set; }
        // public QuerySelect Parent { get; set; }
        public QuerySelect Source { get; set; }
    }

    public class QuerySource
    {
        // Can either be a table source of a subquery source   
        public QuerySelect Source { get; set; }
        public string TableSource { get; set; }
        public string Alias { get; set; }
    }

    public class QuerySelect
    {
        public QueryField[] Fields { get; set; }
        public QuerySource Source { get; set; }
        public string Alias { get; set; }
        public WhereExpression Where { get; set; }
    }

    public abstract class WhereExpression
    {
        public QueryField Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; } 
    }

    public class WhereBooleanExpression : WhereExpression
    {
        // OR / AND
        public string BooleanOperation { get; set; }
        public WhereExpression A { get; set; }
        public WhereExpression B { get; set; }
    }
}