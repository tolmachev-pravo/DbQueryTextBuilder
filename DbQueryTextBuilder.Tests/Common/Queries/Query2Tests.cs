namespace DbQueryTextBuilder.Tests.Common.Queries
{
	[TestFixture]
	public class Query2Tests
	{
		[Test]
		public void Query2_Should_BeCorrectMSSQL()
		{
			var builder = new MSSqlDbQueryTextBuilder();
			var tableVariableName = "@entities";
			var query = builder
				.In().OpenBlock()
					.Select()
						.Column("Id")
					.From(tableVariableName, quote: false)
				.CloseBlock()
				.ToString();

			Assert.That(query, Is.EqualTo($@" IN (
SELECT
	""Id""
FROM {tableVariableName})"));
		}

		[Test]
		public void Query2_Should_BeCorrectPSQL()
		{
			var builder = new PostgreSqlDbQueryTextBuilder();
			var tableVariableName = "@entities";
			var query = builder
				.In().OpenBlock()
					.Select()
						.Column("CAST(value as uuid)", quote: false)
					.From($"jsonb_array_elements_text({tableVariableName})", quote: false)
				.CloseBlock()
				.ToString();

			Assert.That(query, Is.EqualTo($@" IN (
SELECT
	CAST(value as uuid)
FROM jsonb_array_elements_text({tableVariableName}))"));
		}
	}
}
