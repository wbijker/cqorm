using System;
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
        private string _tableName;

        public DataSource(string tableName)
        {
            _tableName = tableName;
        }

        public DataSource()
        {
        }

        public AggregateSource<Q, T> GroupBy<Q>(Expression<Func<T, Q>> clause)
        {
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
            return this;
        }

        public int Delete(Expression<Func<T, bool>> clause)
        {
            return 1;
        }

        public T FetchSingle()
        {
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