using System;
using System.Linq;

namespace cqorm
{
    public class QueryBuilder
    {
        public string Generate(QuerySelect select)
        {
            string sql = "SELECT ";
            sql += string.Join(", ", select.Fields.Select(f => $"{f.Source.Alias}.{f.Name}"));
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
        public QuerySource Source { get; set; }
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
    }
}