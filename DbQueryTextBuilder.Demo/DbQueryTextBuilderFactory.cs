namespace DbQueryTextBuilder.Demo
{
    public class DbQueryTextBuilderFactory
    {
        public IDbQueryTextBuilder Create()
        {
#if POSTGRES
			return new PostgreSqlDbQueryTextBuilder();
#else
            return new MSSqlDbQueryTextBuilder();
#endif
        }
    }
}
