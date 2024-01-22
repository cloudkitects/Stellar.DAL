namespace Stellar.DAL.Tests
{
    [Collection("Remote Database collection")]
    public class RemoteDatabaseTests
    {
        readonly RemoteDatabaseFixture database;

        public RemoteDatabaseTests(RemoteDatabaseFixture fixture)
        {
            database = fixture;
        }

        #region Execute
        /// <summary>
        /// Execute round-trip DML.
        /// </summary>
        [Fact]
        public void ExecuteToList()
        {
            var sql = "SELECT * FROM cfg.SourceMetadata s WHERE s.Isenabled = 1;";

            var list = database.GetCommand()
                .SetCommandText(sql)
                .ExecuteToDynamicList();
        }
        #endregion

    }
}
