using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace cqorm
{
    public class ExpressionParser
    {
        private SelectQuery _query;

        public ExpressionParser(SelectQuery query)
        {
            _query = query;
        }
        
        private Field ParseLamda(LambdaExpression lambda)
        {
            return ParseField(lambda.Body);
        }

        public Field ParseField(Expression exp)
        {
            // QuoteExpression
            if (exp is UnaryExpression un)
            {
                // un.Method?
                return ParseField(un.Operand);
            }
            // u => u.Name == "Willy"
            if (exp is LambdaExpression lambda)
            {
                return ParseLamda(lambda);
            }
            // new { ... }
            if (exp is NewExpression newx)
            {
                // newx.Constructor
                var list = newx.Arguments.Select(a => ParseField(a)).ToArray();
                return Field.List(list);
            }
            if (exp is ConstantExpression constant)
            {
                return ParseConstant(constant);
            }
            // u.Name
            if (exp is MethodCallExpression call)
            {
                return ParseMethodCall(call);
            }
            if (exp is MemberExpression member)
            {
                // member.Member.Name == "name"
                if (member.Expression is ParameterExpression p)
                {
                    // p.Name == "u"
                }
                // If Group by Field then .Key = grouped field
                if (_query.From is FromGroup group)
                {
                    if (_query.GroupBy.Count() == 1)
                    {
                        return _query.GroupBy.First();
                    }
                    return new FieldList(_query.GroupBy);
                }

            
                // member.Member
                return new FieldName(member.Member.Name, GetSourceFromType(member.Type));
            }
            if (exp is BinaryExpression bin)
            {
                return ParseBinaryExpression(bin);
            }
            throw new NotImplementedException();            
        }

        private From GetSourceFromType(Type type)
        {
            if (type == _query.From.Type)
            {
                return _query.From;
            }
            if (_query.Join != null)
            {
                if (type == _query.Join.Left.Type) 
                {
                    return _query.Join.Left;
                }
                if (type == _query.Join.Right.Type)
                {
                    return _query.Join.Right;
                }
            }
            throw new Exception("Could not determine type");
        }

        private Field ParseConstant(ConstantExpression constant)
        {
            if (constant.Type == typeof(string))
            {
                return new Constant(ConstantType.String, constant.Value);
            }
            if (constant.Type == typeof(int)) 
            {
                return new Constant(ConstantType.Int, (int)constant.Value);
            }
            throw new NotImplementedException();
        }

        private Field ParseMethodCall(MethodCallExpression call)
        {
            // A function on a object
            // Either from a Primitive or Aggregate 
            var param = call.Object as ParameterExpression;
            if (_query.From is FromGroup group)
            {
                // Grouped by _query.GrouBy
                // call.Type should be  AggregateSource<T, Q>. 
                // All calls here access method / members on AggregateSource 
                var args = call.Arguments.Select(a => ParseField(a)).ToList();                
                if (call.Method.Name == "CountDistinct")
                {
                    // Count(distinct ...)
                    return new FieldAggregate(AggregateFunction.Count, new List<Field> {
                        new FieldRowFunction(FieldRowFunctionType.Distinct, args.ToArray())
                    }
);
                }
                if (call.Method.Name == "Count")
                {
                    if (args.Count() == 0)
                    {
                        args = new List<Field> { Field.Special(FieldSpecialType.All) };
                    }
                    // Count cannot have arguments?
                    return new FieldAggregate(AggregateFunction.Count, args);
                }
                throw new NotImplementedException();
            }
            
            // _query.From
            if (call.Type == typeof(string))
            {
                // All string functions here
                // with call.Argu,ents
                // call.Method.Name == "ToLower";
                return new FieldFunction(ToStringFunctionType(call.Method.Name), ParseField(call.Object));
            }
            throw new NotImplementedException();
        }

        private FieldFunctionType ToStringFunctionType(string name)
        {
            switch (name)
            {
                case "ToLower":
                    return FieldFunctionType.Lower;
                case "ToUpper":
                    return FieldFunctionType.Upper;
            }
            throw new NotImplementedException();
        }

        public FieldMath ParseBinaryExpression(BinaryExpression bin)
        {
            return new FieldMath(ParseField(bin.Left), ParseOperator(bin.NodeType), ParseField(bin.Right));
        }

        private FieldMathOperator ParseOperator(ExpressionType nodeType)
        {
            if (nodeType == ExpressionType.Equal)
            {
                return FieldMathOperator.Equal;
            }
            if (nodeType == ExpressionType.OrElse || nodeType == ExpressionType.Or)
            {
                return FieldMathOperator.Or;
            }
            if (nodeType == ExpressionType.Add)
            {
                return FieldMathOperator.And;
            }
            if (nodeType == ExpressionType.GreaterThan)
            {
                return FieldMathOperator.GreaterThan;
            }
            if (nodeType == ExpressionType.GreaterThanOrEqual)
            {
                return FieldMathOperator.GreaterEqualThan;
            }
            if (nodeType == ExpressionType.LessThan)
            {
                return FieldMathOperator.LessThan;
            }
            if (nodeType == ExpressionType.LessThanOrEqual)
            {
                return FieldMathOperator.LessEqualThan;
            }

            if (nodeType == ExpressionType.Multiply)
            {
                return FieldMathOperator.Multiply;
            }
            if (nodeType == ExpressionType.Divide)
            {
                return FieldMathOperator.Divide;
            }
            if (nodeType == ExpressionType.Add)
            {
                return FieldMathOperator.Plus;
            }
            if (nodeType == ExpressionType.Subtract)
            {
                return FieldMathOperator.Minus;
            }

            throw new NotImplementedException();
        }
    }
}