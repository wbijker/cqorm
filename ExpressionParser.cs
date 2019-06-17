using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace cqorm
{
    public class ExpressionParser
    {

        public static Field ParseSelectField(Type type, From from, Expression exp, Field carry = null)
        {   
            if (exp is LambdaExpression lambda)
            {
                // Options allowed:
                // 1. Whole object: .Select(s => s)
                if (lambda.Body is ParameterExpression param)
                {
                    if (param.Type != type)
                    {
                        throw new Exception($"Select source must be of type {type.ToString()}");
                    }

                    // Select all fields
                    return Field.List(
                        type.GetProperties()
                        .Select(p => (Field)new FieldName(p.Name, from))
                        .ToArray()
                    );
                }
                
                // 2. Selection: .Select(s => new { ... })
                if (lambda.Body is NewExpression newx)
                {
                    
                }

                // 3. Single Constant: .Select(_ => 10)
                if (lambda.Body is ConstantExpression constant)
                {
                    // todo: need to cater for different types here
                    return Field.ConstantString(constant.ToString());
                }

                // 4. Single epxressino: .Select(s => s.Field * 2)
                if (lambda.Body is UnaryExpression unary)
                {

                }

                if (lambda.Body is BinaryExpression bi)
                {

                }

            }
            throw new Exception("Only all fields, selection of fields or constant allowd in select statement");
        }

        private SelectQuery _query;

        public ExpressionParser(SelectQuery query)
        {
            _query = query;
        }

        private Field ParseLamda(LambdaExpression lambda)
        {
            return ParseField(lambda.Body);
        }

        public Field ParseFieldAlias(Expression exp, string alias)
        {
            var field = ParseField(exp);
            field.Alias = alias;
            return field;
        }

        public Field ParseField(Expression exp)
        {
            // .Select(s => s)
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
                var list = newx.Arguments.Select((a,i) => ParseFieldAlias(a, newx.Members[i].Name)).ToArray();
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
                    return new FieldName(member.Member.Name, GetSourceFromType(p.Type));
                }
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
            if (_query.From is FromJoin join)
            {
                if (type == join.Left.Type)
                {
                    return join.Left;
                }
                if (type == join.Right.Type)
                {
                    return join.Right;
                }
            }
            throw new Exception("Could not determine type");
        }

        private Field ParseConstant(ConstantExpression constant)
        {
            switch (constant.Value)
            {
                case char ch:
                    return new Constant(ConstantType.Char, ch);
                case string str:
                    return new Constant(ConstantType.String, str);
                case int i:
                    return new Constant(ConstantType.Int, i);
                case double d:
                    return new Constant(ConstantType.Double, d);
                case bool b:
                    return new Constant(ConstantType.Bool, b);
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
                // First check casts
                if (call.Method.Name == "ToString")
                {
                    return new FieldCast(ParseField(call.Object), FieldCastType.String);
                }
                // All string functions here
                // with call.Argu,ents
                // call.Method.Name == "ToLower";
                var args = call.Arguments.Select(a => ParseField(a)).ToList();
                return new FieldFunction(ToStringFunctionType(call.Method.Name), ParseField(call.Object), args.ToArray());
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
                case "Substring":
                    return FieldFunctionType.Substring;
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
                return FieldMathOperator.Plus;
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