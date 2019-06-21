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
    public enum FieldType
    {
        Int,
        String,
        Char,
        Double,
        Binary,
        Bool
    }
    
    public abstract class FieldExpression
    {
        // Each expression shoudl have a return type
        public FieldType FieldType { get; set; }
    }

    public abstract class SelectField
    {
        public string Alias { get; set; }
        public FieldExpression Expression { set; get; }
    }

    public abstract class Field
    {
        public FieldType FieldType { get; set; }
        public string Alias { get; set; }
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
        public Constant(FieldType type, object value)
        {
            this.Value = value;
            this.FieldType = type;
        }
        public object Value { get; set; }
    }

    public enum FieldSpecialType
    {
        All
    }

    // Not a function or aggregate
    // CAST(9.5 AS INT)
    public class FieldCast : Field
    {
        public FieldCast(Field field, FieldType type)
        {
            Field = field;
            ToType = type;
        }

        public Field Field { get; set; }
        public FieldType ToType { get; set; }
    }

    // Represents a * as in count(*)
    public class FieldSpecial : Field
    {
        public FieldSpecial(FieldSpecialType specialType)
        {
            SpecialType = specialType;

        }
        public FieldSpecialType SpecialType { get; set; }
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
        public FieldFunction(FieldFunctionType function, Field source, params Field[] Arguments)
        {
            this.Source = source;
            this.Function = function;
            this.Arguments = new List<Field>(Arguments);

        }

        public Field Source { get; set; }
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
        StringConcat
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