namespace DbQueryTextBuilder.Tests.MSSql
{
	[TestFixture]
	public class InTests
	{
		private IDbQueryTextBuilder _builder;

		[SetUp]
		public void Setup()
		{
			_builder = new MSSqlDbQueryTextBuilder();
		}

		[Test]
		public void InGuidValues_Should_BeCorrect()
		{
			var values = new List<Guid>
			{
				Guid.NewGuid(),
				Guid.NewGuid()
			};

			var query = _builder
				.Column("table1", "column1")
				.In(values)
				.ToString();

			Assert.That(query?.ToOneLine(), Is.EqualTo($"{"table1".Quote()}.{"column1".Quote()} IN ('{values[0]}', '{values[1]}')"));
		}
	}
}
