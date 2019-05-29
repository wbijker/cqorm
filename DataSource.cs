using System;
using System.Linq;
using System.Linq.Expressions;

namespace cqorm
{
    public class Join<T, Q>
    {
        public T Left { get; set; }
        public Q Right { get; set; }
    }


    public class DataSource<T>: BaseSource<T>
    {
        public DataSource(SelectQuery query) : base(query)
        {
        }

        public DataSource(): base()
        {
        }

        public AggregateSource<Q, T> GroupBy<Q>(Expression<Func<T, Q>> clause)
        {
            return new AggregateSource<Q, T>(_query);
        }

        public DataSource<T> Union(DataSource<T> source)
        {
            return this;
        }

        public DataSource<T> UnionAll(DataSource<T> source)
        {
            return this;
        }

        public DataSource<Join<T, Q>> InnerJoin<Q>(DataSource<Q> source, Expression<Func<T, Q, bool>> on)
        {
            return new DataSource<Join<T, Q>>();
        }

        public DataSource<T> Where(Expression<Func<T, bool>> clause)
        {
            var parse = new ExpressionParser(_query);
            var ff = parse.ParseField(clause);
            if (ff is FieldMath math)
            {
                _query.Where = math;
                return this;
            }
            throw new Exception("Where should be a binary expression")  ;
            // query.Where = new FieldMath(
            //     Field.Function("lower", Field.Name("Username", query.From)), 
            //     FieldMathOperator.Equal,
            //     Field.ConstantString("Willem"));
        }

        public int Delete(Expression<Func<T, bool>> clause)
        {        
            return 1;
        }

      

        public DataSource<T> Update<Q>(Expression<Func<T, Q>> update, Q newValue)
        {
            return this;
        }

        public DataSource<T> Update<Q>(Expression<Func<T, Q>> update, Expression<Func<T, Q>> newValue)
        {
            return this;
        }

        // The momemtn you select something the source changes
        public DataSource<Q> Select<Q>(Expression<Func<T, Q>> select)
        {
            var parse = new ExpressionParser(_query);
            parse.ParseField(select);
            
            return new DataSource<Q>(_query);
        }
    }
}