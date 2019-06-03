using System;
using System.Linq;

namespace cqorm
{
    public interface ISource<T>
    {
        T FetchSingle();
        T[] FetchMultiple();
    }

    public abstract class BaseSource
    {
        protected SelectQuery _query;

        public BaseSource(SelectQuery query)
        {
            _query = query;
        }

        public BaseSource()
        {
            _query = new SelectQuery();
        }
    }
}