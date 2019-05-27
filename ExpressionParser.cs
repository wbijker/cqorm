using System;
using System.Linq.Expressions;

namespace cqorm
{
    public class ExpressionParser
    {
        public void ProcessExpression(Expression exp)
        {
            Console.WriteLine($"process expression {exp.NodeType}");
            if (exp is ConstantExpression constant)
            {

            }
            if (exp is LambdaExpression lambda)
            {
                ParseLamda(lambda);
            }
        }

        private void ParseLamda(LambdaExpression lambda)
        {
            // lambda.Parameters[0] 
            var nn = (lambda.Body as NewArrayExpression);
        }
    }
}