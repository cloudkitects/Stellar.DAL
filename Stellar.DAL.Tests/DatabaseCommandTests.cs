using Stellar.Common;

namespace Stellar.DAL.Tests;

[Collection("Local Database collection")]
public class DatabaseCommandTests(LocalSqlDatabaseFixture fixture)
{
    readonly LocalSqlDatabaseFixture database = fixture;

    [Fact]
    public void ExecuteReaderBreaksOnCondition()
    {
        var command = database.GetCommand()
            .SetCommandText("select 1 a, 2 b union select 3 a, 4 b;");

        var i = 0;

        command.ExecuteReader((rec) =>
        {
            var b = ValueConverter.Parse<int>(rec["b"]);

            if ((int)b! == 2)
            {
                return false;
            }

            i++;

            return true;
        });

        Assert.Equal(0, i);
    }

    [Fact]
    public void ExecuteReaderBreaksOnError()
    {
        var command = database.GetCommand()
            .SetCommandText("select 1 a, 2 b union select 3 a, 4 b;");

        var i = 0;

        command.ExecuteReader((rec) =>
        {
            try
            {
                var b = ValueConverter.Parse<int>(rec["c"]);

                i++;

                return true;
            }
            catch (Exception exception)
            {
                Assert.Equal("c", exception.Message);

                return false;
            }
        });

        Assert.Equal(0, i);
    }
}
