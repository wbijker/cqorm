using System;
using System.Collections.Generic;
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
        public SelectQuery Query => _query;

        public DataSource(SelectQuery query) : base(query)
        {
        }

        public DataSource(): base()
        {
        }

        public AggregateSource<Q, T> GroupBy<Q>(Expression<Func<T, Q>> clause)
        {
            // Single or Multiple groupbys
            // GroupBy(e => e.field)
            // GroupBy(e => new { e.field1. e.field2 })
            var parse = new ExpressionParser(_query);
            var field = parse.ParseField(clause);
            if (field is FieldName)
            {
                // Single group by 
                // Select default groupby item?
                // _query.Fields = new List<Field> {...}
                _query.GroupBy = new List<FieldName> { field as FieldName };
                // Change the source of the query. Makes it avaialb eofr the ExpressionParser
                _query.From = new FromGroup(_query.From);
                return new AggregateSource<Q, T>(_query);
            }
            // todo: cater for multiples
            throw new Exception("Invalid GroupBy Expression. It can only be a field name or a group object");
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
            var parse = new ExpressionParser(_query);
            var fieldOn = parse.ParseField(on);
            if (fieldOn is FieldMath math)
            {
                var select = new SelectQuery
                {
                    Join = new QueryJoin
                    {
                        Left = new FromSubQuery(_query),
                        Right = new FromSubQuery(source.Query)
                    }
                };

                return new DataSource<Join<T, Q>>(select);
            }

            throw new NotImplementedException();
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
            var field = parse.ParseField(select);
            if (field is FieldName)
            {
                _query.Fields = new List<Field> { field };
            }
            if (field is FieldList list)
            {
                _query.Fields = list.Fields;
            }
            return new DataSource<Q>(_query);
        }
    }
}