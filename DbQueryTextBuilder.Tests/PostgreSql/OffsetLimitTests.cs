namespace DbQueryTextBuilder.Tests.PostgreSql
{
	[TestFixture]
	public class OffsetLimitTests
	{
		private IDbQueryTextBuilder _builder;

		[SetUp]
		public void Setup()
		{
			_builder = new PostgreSqlDbQueryTextBuilder();
		}

		[Test]
		public void OffsetLimit_Should_Return()
		{
			var query = _builder
				.OffsetLimit(offset: 0, limit: 10)
				.ToString();

			Assert.That(query?.ToOneLine(), Is.EqualTo(" OFFSET 0 LIMIT 10"));
		}

		[Test]
		public void OffsetLimitWithSelect_Should_BeCorrect()
		{
			var query = _builder
				.Select()
					.Column("column1")
				.From("table1")
				.OrderBy()
					.Column("column2")
				.OffsetLimit(offset: 0, limit: 10)
				.ToString();

			Assert.That(
				query?.ToOneLine(),
				Is.EqualTo($" SELECT {"column1".Quote()} FROM {"table1".Quote()} ORDER BY {"column2".Quote()} OFFSET 0 LIMIT 10"));
		}
	}
}
