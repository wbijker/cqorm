using System;
using System.Linq;

namespace cqorm
{
    public interface ISQLDriver
    {
        string Generate(SelectQuery query);

    }

    public class SQLLiteDriver : ISQLDriver
    {
        public string Generate(SelectQuery query)
        {
            string sql = "SELECT ";
            sql += string.Join(", ", query.Fields.Select(f => GenerateField(f)));
            sql += " FROM ";
            if (query.From is FromTable table) {
                sql += table.Table;
            }
            return sql;
        }

        private string GenerateField(Field f)
        {
            if (f is Constant c)
            {
                return c.Value;
            }
            if (f is FieldName name) 
            {
                return $"{name.Source.Alias}.{name.Name}";
            }
            if (f is FieldAggregate aggregate)
            {
                var arg = GenerateField(aggregate.Argument);
                return $"{GenerateFunction(aggregate.Function)}({arg})";
            }
            if (f is FieldRowFunction row) 
            {
                var args = String.Join(", ", row.Arguments.Select(a => GenerateField(a)));
                return $"{GenerateRowFunction(row.Function)}({args})";
            }
            if (f is FieldFunction func)
            {
                var args = String.Join(", ", func.Arguments.Select(a => GenerateField(a)));
                return $"{GenerateFunction(func.Function)}({args})";
            }
            if (f is FieldMath math)
            {
                return $"{GenerateField(math.Left)} {math.Operator} {GenerateField(math.Right)}";
            }
            throw new NotImplementedException(f.ToString());
        }

        private object GenerateFunction(string function)
        {
            return function;
        }

        private object GenerateRowFunction(FieldRowFunctionType function)
        {
            switch (function)
            {
                case FieldRowFunctionType.Distinct:
                    return "distinct";
            }
            throw new Exception($"Could not map row function '{function.ToString()}'");
        }

        private object GenerateFunction(AggregateFunction function)
        {
            switch (function)
            {
                case AggregateFunction.Average:
                    return "AVERAGE";
            }
            throw new Exception($"Could not map aggregate function '{function.ToString()}'");
        }
    }
}