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

using System.Collections.Generic;

namespace cqorm
{
    public abstract class Field
    {
        // either constant, name, aggregate, distinct, fieldfunction, fieldExpression
        public static Constant Constant(string value)
        {
            return new Constant(value);
        }

        public static FieldName Name(string name, From source)
        {
            return new FieldName(name, source);
        }

        public static FieldAggregate Aggregate(AggregateFunction function, Field argument)
        {
            return new FieldAggregate(function, argument);
        }

        public static FieldRowFunction RowFunction(FieldRowFunctionType function, params Field[] Arguments)
        {
            return new FieldRowFunction(function, Arguments);
        }

        public static FieldFunction Function(string function, params Field[] Arguments)
        {
            return new FieldFunction(function, Arguments);
        }

        public static FieldMath Math(Field a, FieldMathOperator op, Field b)
        {
            return new FieldMath(a, op, b);
        }

    }

    public class Constant : Field
    {
        // For now just use string
        public Constant(string value)
        {
            this.Value = value;

        }
        public string Value { get; set; }
    }

    // a.Name
    public class FieldName : Field
    {
        public FieldName(string name, From source)
        {
            this.Name = name;
            this.Source = source;

        }
        public string Name { get; set; }
        public From Source { get; set; }
    }

    public enum AggregateFunction
    {
        Count,
        Sum,
        Min,
        Max,
        Average
    }

    // count(a.id), sum(b.value)
    public class FieldAggregate : Field
    {
        public FieldAggregate(AggregateFunction function, Field argument)
        {
            this.Function = function;
            this.Argument = argument;

        }
        public AggregateFunction Function { get; set; }
        public Field Argument { get; set; }
    }


    public enum FieldRowFunctionType
    {
        Distinct
        // Don't know of any any row functions yet
    }

    public class FieldRowFunction : Field
    {
        public FieldRowFunction(FieldRowFunctionType function, params Field[] Arguments)
        {
            this.Function = function;
            this.Arguments = new List<Field>(Arguments);

        }
        public FieldRowFunctionType Function { get; set; }
        public List<Field> Arguments { get; set; }
    }

    // Coalesce, string functions, date functions, number functions
    public class FieldFunction : Field
    {
        public FieldFunction(string function, params Field[] Arguments)
        {
            this.Function = function;
            this.Arguments = new List<Field>(Arguments);

        }
        public string Function { get; set; }
        public List<Field> Arguments { get; set; }
    }

    public enum FieldMathOperator
    {
        Plus,
        Minus,
        Multiply,
        Divide,
        Equal,
        BiggerThan,
        LessThan,
        BiggerEquanThan,
        LessEqualThan,
        Or,
        And,
    }

    public class FieldMath : Field
    {
        public FieldMath(Field left, FieldMathOperator op, Field right)
        {
            this.Left = left;
            this.Operator = op;
            this.Right = right;

        }
        public Field Left { get; set; }
        public FieldMathOperator Operator { get; set; }
        public Field Right { get; set; }
    }

}