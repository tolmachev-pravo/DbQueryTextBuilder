namespace DbQueryTextBuilder.Tests.Common.Queries
{
	[TestFixture]
	public class Query3Tests
	{
		[Test]
		public void Query3_Should_BeCorrectMSSQL()
		{
			var builder = new MSSqlDbQueryTextBuilder();
			var query = builder
				.Select().Append(" COUNT(1)").From("schema1", "table1")
				.Where().Column("Id").Equals().ColumnValue("value1")
					.And().Column("IsDeleted").Equals().ColumnValue(false)
				.ToString();

			Assert.That(query, Is.EqualTo($@"
SELECT COUNT(1)
FROM ""schema1"".""table1""
WHERE
	""Id"" = N'value1'
	AND ""IsDeleted"" = 'false'"));
		}

		[Test]
		public void Query3_Should_BeCorrectPSQL()
		{
			var builder = new PostgreSqlDbQueryTextBuilder();
			var query = builder
				.Select().Append(" COUNT(1)").From("schema1", "table1")
				.Where().Column("Id").Equals().ColumnValue("value1")
					.And().Column("IsDeleted").Equals().ColumnValue(false)
				.ToString();

			Assert.That(query, Is.EqualTo($@"
SELECT COUNT(1)
FROM ""schema1"".""table1""
WHERE
	""Id"" = E'value1'
	AND ""IsDeleted"" = 'false'"));
		}
	}
}
