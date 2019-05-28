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


    public class DataSource<T>
    {
        private SelectQuery _query;

        public DataSource()
        {
            _query = new SelectQuery();
            _query.From = new FromTable
            {
                Table = typeof(T).Name,
                Alias = "u"
            };

            // query.Fields = new List<Field> 
            // {
            //     Field.Name("Username", query.From),
            //     Field.Name("Password", query.From),
            //     Field.Name("Email", query.From)
            // };
            
        }

        public AggregateSource<Q, T> GroupBy<Q>(Expression<Func<T, Q>> clause)
        {
            // e => e.Field
            if (clause is LambdaExpression lambda)
            {
                // e
                if (clause.Parameters[0] is ParameterExpression p)
                {
                    // p.name == "E"
                    // p.Type == typeof(T)
                }
                // e => e.Field
                if (lambda.Body is MemberExpression member)
                {
                    // member.Member.Name == "Field"
                    // member.Member
                    // _querySource.Source
                }
                
            }
            // return this;
            return new AggregateSource<Q, T>();
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

        public T FetchSingle()
        {
            // If not select was spesified select all from original source
            if (_query.Fields == null)
            {
                var props = typeof(T).GetProperties();
                _query.Fields = props
                    .Select(p => (Field)new FieldName(p.Name, _query.From))
                    .ToList();
            }
                
                
            ISQLDriver driver = new SQLLiteDriver();
            Console.WriteLine("Fetch Single");
            Console.WriteLine(driver.Generate(_query));
            return default(T);
        }

        public T[] FetchArray()
        {
            // ToList()
            return null;
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
            return new DataSource<Q>();
        }
    }


    public class AggregateSource<T, Q>
    {
        public T Key { get; set; }
        // public Aggregate<Q> Aggregate { get; set; }

        public DataSource<P> Select<P>(Expression<Func<AggregateSource<T, Q>, P>> select)
        {
            // ExpParser.ProcessExpression(select);
            return new DataSource<P>();
        }

        public AggregateSource<T, Q> Distinct()
        {
            return this;
        }

        public int Count()
        {
            return 0;
        }

        public Q Max()
        {
            return default(Q);
        }

        public Q Min()
        {
            return default(Q);
        }
    }
}