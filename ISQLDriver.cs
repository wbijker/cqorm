namespace cqorm
{
    public interface ISQLDriver
    {
        string GenerateName(FieldName Name);
        string GenerateAggregate(FieldAggregate Aggregate);
        string GenerateRowFunction(FieldRowFunction RowFunction);
        string GenerateFunction(FieldFunction Function);
        string GenerateMath(FieldMath Math);

    }

    public class SQLLiteDriver : ISQLDriver
    {
        public string GenerateAggregate(FieldAggregate Aggregate)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateFunction(FieldFunction Function)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateMath(FieldMath Math)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateName(FieldName Name)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateRowFunction(FieldRowFunction RowFunction)
        {
            throw new System.NotImplementedException();
        }
    }
}