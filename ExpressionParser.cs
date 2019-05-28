using System;
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
            if (lambda.Body is BinaryExpression exp)
            {
                return ParseBinaryExpression(exp);
            }
            throw new NotImplementedException();
        }

        public Field ParseField(Expression exp)
        {
            // u => u.Name == "Willy"
            if (exp is LambdaExpression lambda)
            {
                return ParseLamda(lambda);
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
                // member.Member
                return new FieldName(member.Member.Name, _query.From);
            }
        
            throw new NotImplementedException();            
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

            throw new NotImplementedException();
        }
    }
}