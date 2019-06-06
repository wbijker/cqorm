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
        public string Alias { get; set; }
        
        // either constant, name, aggregate, distinct, fieldfunction, fieldExpression
        public static Constant ConstantString(string value)
        {
            return new Constant(ConstantType.String, value);
        }

        public static Constant ConstantInt(string value)
        {
            return new Constant(ConstantType.Int, value);
        }

        public static FieldName Name(string name, From source)
        {
            return new FieldName(name, source);
        }

        public static FieldAggregate Aggregate(AggregateFunction function, params Field[] arguments)
        {
            return new FieldAggregate(function, new List<Field>(arguments));
        }

        public static FieldRowFunction RowFunction(FieldRowFunctionType function, params Field[] Arguments)
        {
            return new FieldRowFunction(function, Arguments);
        }

        public static FieldFunction Function(FieldFunctionType function, params Field[] Arguments)
        {
            return new FieldFunction(function, Arguments);
        }

        public static FieldMath Math(Field a, FieldMathOperator op, Field b)
        {
            return new FieldMath(a, op, b);
        }

        public static FieldParameter Param(string name)
        {
            return new FieldParameter(name);
        }

        public static FieldList List(params Field[] fields)
        {
            return new FieldList(fields);
        }

        public static FieldSpecial Special(FieldSpecialType fieldType)
        {
            return new FieldSpecial(fieldType);
        }

    }

    public enum ConstantType
    {
        Int,
        String,
        Double,
        Binary,
        Bool
    }

    public class FieldList : Field
    {
        public List<Field> Fields { get; set; }

        public FieldList(IEnumerable<Field> fields)
        {
            Fields = new List<Field>(fields);
        }
    }

    public class Constant : Field
    {
        public Constant(ConstantType type, object value)
        {
            this.Value = value;
            this.Type = type;

        }
        public ConstantType Type { get; set; }
        public object Value { get; set; }
    }

    public enum FieldSpecialType
    {
        All
    }

    // Represents a * as in count(*)
    public class FieldSpecial : Field
    {
        public FieldSpecial(FieldSpecialType fieldType)
        {
            this.FieldType = fieldType;

        }
        public FieldSpecialType FieldType { get; set; }


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
        public FieldAggregate(AggregateFunction function, List<Field> arguments)
        {
            this.Function = function;
            this.Arguments = arguments;

        }
        public AggregateFunction Function { get; set; }
        public List<Field> Arguments { get; set; }
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

    public enum FieldFunctionType
    {
        Lower,
        Upper,
        Substring,
        LeftTrim,
        RightTrim,
        Trim,
        Length,
        Replace,
        Contains
    }

    // Coalesce, string functions, date functions, number functions
    public class FieldFunction : Field
    {
        public FieldFunction(FieldFunctionType function, params Field[] Arguments)
        {
            this.Function = function;
            this.Arguments = new List<Field>(Arguments);

        }
        public FieldFunctionType Function { get; set; }
        public List<Field> Arguments { get; set; }
    }

    public enum FieldMathOperator
    {
        Plus,
        Minus,
        Multiply,
        Divide,
        Equal,
        GreaterThan,
        LessThan,
        GreaterEqualThan,
        LessEqualThan,
        Or,
        And,
    }

    public class FieldParameter : Field
    {
        public string Name { get; set; }

        public FieldParameter(string name)
        {
            Name = name;
        }
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