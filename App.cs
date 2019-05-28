using System;
using System.Collections.Generic;

namespace cqorm
{
    public class App
    {
        private void QueryBuilder()
        {

            var query = new SelectQuery();
            query.From = new FromTable
            {
                Table = "User",
                Alias = "u"
            };
            query.Fields = new List<Field> 
            {
                Field.Name("Username", query.From),
                Field.Name("Password", query.From),
                Field.Name("Email", query.From)
            };
            query.Where = new FieldMath(
                Field.Function(FieldFunctionType.Lower, Field.Name("Username", query.From)), 
                FieldMathOperator.Equal,
                Field.ConstantString("Willem"));

            ISQLDriver driver = new SQLLiteDriver();
            Console.WriteLine(driver.Generate(query));

        }
        public void Simple()
        {
            var a = new DataSource<User>()
                .Where(u => u.Name.ToLower() == "willem")
                .FetchSingle();
        }

        public void Run()
        {
            // QueryBuilder();
            Simple();
            // ScissorTest();
            return;
        }
    }
}