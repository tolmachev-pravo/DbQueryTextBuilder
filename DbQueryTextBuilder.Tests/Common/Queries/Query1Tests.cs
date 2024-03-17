namespace DbQueryTextBuilder.Tests.Common.Queries
{
	[TestFixture]
	public class Query1Tests
	{
		private IDbQueryTextBuilder _builder;

		[SetUp]
		public void Setup()
		{
			_builder = new MSSqlDbQueryTextBuilder();
		}

		[Test]
		public void Query1_Should_BeCorrect()
		{
			var query = _builder
				.InsertInto("CaseMap", "ProjectLines")
				.OpenBlock()
					.Column("BlockLineId")
					.Column("Order")
					.Column("ProjectVisualBlockId")
				.CloseBlock()
				.Select()
					.Parameter("targetBlockLineId")
					.Column("SourceProjectLines", "Order")
					.Column("TargetProjectBlocks", "Id")
				.From("CaseMap", "ProjectVisualBlocks").As("TargetProjectBlocks")
				.InnerJoin("CaseMap", "ProjectVisualBlocks").As("SourceProjectBlocks")
					.On().Column("SourceProjectBlocks", "VisualBlockId").Equals().Parameter("sourceVisualBlockId")
					.And().Column("TargetProjectBlocks", "ProjectId").Equals().Column("SourceProjectBlocks", "ProjectId")
					.And().Column("TargetProjectBlocks", "Order").Equals().Column("SourceProjectBlocks", "Order")
				.InnerJoin("CaseMap", "ProjectLines").As("SourceProjectLines")
					.On().Column("SourceProjectBlocks", "Id").Equals().Column("SourceProjectLines", "ProjectVisualBlockId")
					.And().Column("SourceProjectLines", "BlockLineId").Equals().Parameter("sourceBlockLineId")
				.InnerJoin("CaseMap", "Projects").As("Entities")
					.On().Column("Entities", "Id").Equals().Column("TargetProjectBlocks", "ProjectId")
				.Where()
					.Column("TargetProjectBlocks", "VisualBlockId").Equals().Parameter("targetVisualBlockId")
					.And().Column("Entities", "ProjectTypeId").In()
					.OpenBlock()
						.Select()
							.Column("Id")
						.From("@entityTypeIds", quote: false)
					.CloseBlock()
					.And().Not().Exists()
						.OpenBlock()
							.Select()
								.ColumnValue(1)
							.From("CaseMap", "ProjectLines").As("TargetProjectLines")
							.Where().Column("TargetProjectLines", "ProjectVisualBlockId").Equals().Column("TargetProjectBlocks", "Id")
								.And().Column("TargetProjectLines", "Order").Equals().Column("SourceProjectLines", "Order")
								.And().Column("TargetProjectLines", "BlockLineId").Equals().Parameter("targetBlockLineId")
						.CloseBlock()
				.End()
				.ToString();

			Assert.That(query, Is.EqualTo(@$"
INSERT INTO ""CaseMap"".""ProjectLines"" (""BlockLineId"",
	""Order"",
	""ProjectVisualBlockId"")
SELECT
	@targetBlockLineId,
	""SourceProjectLines"".""Order"",
	""TargetProjectBlocks"".""Id""
FROM ""CaseMap"".""ProjectVisualBlocks"" AS ""TargetProjectBlocks""
INNER JOIN ""CaseMap"".""ProjectVisualBlocks"" AS ""SourceProjectBlocks""
	ON ""SourceProjectBlocks"".""VisualBlockId"" = @sourceVisualBlockId
	AND ""TargetProjectBlocks"".""ProjectId"" = ""SourceProjectBlocks"".""ProjectId""
	AND ""TargetProjectBlocks"".""Order"" = ""SourceProjectBlocks"".""Order""
INNER JOIN ""CaseMap"".""ProjectLines"" AS ""SourceProjectLines""
	ON ""SourceProjectBlocks"".""Id"" = ""SourceProjectLines"".""ProjectVisualBlockId""
	AND ""SourceProjectLines"".""BlockLineId"" = @sourceBlockLineId
INNER JOIN ""CaseMap"".""Projects"" AS ""Entities""
	ON ""Entities"".""Id"" = ""TargetProjectBlocks"".""ProjectId""
WHERE
	""TargetProjectBlocks"".""VisualBlockId"" = @targetVisualBlockId
	AND ""Entities"".""ProjectTypeId"" IN (
SELECT
	""Id""
FROM @entityTypeIds)
	AND NOT EXISTS (
SELECT
	1
FROM ""CaseMap"".""ProjectLines"" AS ""TargetProjectLines""
WHERE
	""TargetProjectLines"".""ProjectVisualBlockId"" = ""TargetProjectBlocks"".""Id""
	AND ""TargetProjectLines"".""Order"" = ""SourceProjectLines"".""Order""
	AND ""TargetProjectLines"".""BlockLineId"" = @targetBlockLineId);"));
		}
	}
}
