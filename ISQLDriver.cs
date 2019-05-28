namespace cqorm
{
    public interface ISQLDriver
    {
        string Generate(SelectQuery query);

    }

    public class SQLLiteDriver : ISQLDriver
    {
        public string Generate(SelectQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}