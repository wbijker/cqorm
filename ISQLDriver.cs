using System;
using System.Collections.Generic;
using System.Linq;

namespace cqorm
{
    public interface ISQLDriver
    {
        string Generate(SelectQuery query);

    }

    public class SQLLiteDriver : ISQLDriver
    {
        private string GenerateFields(IEnumerable<Field> list)
        {
            return String.Join(", ", list.Select(f => GenerateField(f)));
        }

        private string GenerateFrom(From from)
        {
            if (from is FromTable table) 
            {
                return $"{table.Table} {table.Alias}";
            }
            if (from is FromJoin join)
            {
                var left = GenerateFrom(join.Left);
                var right = GenerateFrom(join.Right);
                var on = GenerateField(join.On);
                return $@"{left} 
                    INNER JOIN {right} 
                    ON {on}";
            }
            if (from is FromSubQuery query) 
            {
                return $"({Generate(query.Query)}) {query.Alias}";
            }
            if (from is FromGroup group)
            {
                return GenerateFrom(group.From);
            }
            return "";
        }

        public string Generate(SelectQuery query)
        {
            string sql = "SELECT ";
            if (query.Fields == null)
            {
                throw new Exception("No fields to select");
            }
            sql += GenerateFields(query.Fields);
            sql += $" FROM {GenerateFrom(query.From)}";
            if (query.Where != null)
            {
                sql += " WHERE " + GenerateField(query.Where);
            }
            if (query.GroupBy?.Count() > 0)
            {
                sql += " GROUP BY " + GenerateFields(query.GroupBy);
            }
            return sql;
        }

        private object OperatorStr(FieldMathOperator op)
        {
            switch (op)
            {
                case FieldMathOperator.Plus:
                    return "+";
                case FieldMathOperator.Minus:
                    return "-";
                case FieldMathOperator.Multiply:
                    return "*";
                case FieldMathOperator.Divide:
                    return "/";
                case FieldMathOperator.Equal:
                    return "=";
                case FieldMathOperator.GreaterThan:
                    return ">";
                case FieldMathOperator.LessThan:
                    return "<";
                case FieldMathOperator.GreaterEqualThan:
                    return ">=";
                case FieldMathOperator.LessEqualThan:
                    return "<=";
                case FieldMathOperator.Or:
                    return "OR";
                case FieldMathOperator.And:
                    return "AND";
            }
            throw new NotImplementedException();
        }

        private string GenerateField(Field f)
        {
            if (f is FieldSpecial special)
            {
                switch (special.SpecialType)
                {
                    case FieldSpecialType.All:
                        return "*";
                }
                throw new NotImplementedException();
            }
            if (f is Constant c)
            {
                switch (c.FieldType)
                {
                    case FieldType.Int:
                    case FieldType.Double:
                        return c.Value.ToString();
                    case FieldType.String:
                    case FieldType.Char:
                        return $"'{c.Value.ToString()}'";
                }
                throw new NotImplementedException();
            }
            if (f is FieldName name) 
            {
                return $"{name.Source.Alias}.{name.Name}";
            }
            if (f is FieldAggregate aggregate)
            {
                var args = GenerateFields(aggregate.Arguments);
                return $"{GenerateFunction(aggregate.Function)}({args}) AS {aggregate.Alias}";
            }
            if (f is FieldRowFunction row) 
            {
                var args = GenerateFields(row.Arguments);
                return $"{GenerateRowFunction(row.Function)} {args}";
            }
            if (f is FieldFunction func)
            {
                // Combined source and arguments
                var args = GenerateFields(func.Arguments.Prepend(func.Source));
                return $"{GenerateFunction(func.Function)}({args})";
            }
            if (f is FieldMath math)
            {
                return $"{GenerateField(math.Left)} {OperatorStr(math.Operator)} {GenerateField(math.Right)}";
            }
            if (f is FieldCast cast)
            {
                return $"CAST({GenerateField(cast.Field)} AS {CastType(cast.ToType)})";
            }
            throw new NotImplementedException(f.ToString());
        }

        private string CastType(FieldType toType)
        {
            switch (toType)
            {
                case FieldType.String:
                    return "string";
                case FieldType.Double:
                    return "double";
                case FieldType.Int:
                    return "int";
            }
            throw new NotImplementedException();
        }

        private object GenerateFunction(FieldFunctionType function)
        {
            switch (function)
            {
                case FieldFunctionType.Lower:
                    return "lower";
                case FieldFunctionType.Upper:
                    return "upper";
                case FieldFunctionType.Substring: 
                    return "substr";
            }
            throw new NotImplementedException();
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
            case AggregateFunction.Count:
                    return "COUNT";
            }
            throw new Exception($"Could not map aggregate function '{function.ToString()}'");
        }
    }
}