namespace DbQueryTextBuilder
{
	public enum DbQueryTextBuilderState
	{
		None,
		Select,
		From,
		Column,
		On,
		Where,
		CloseBlock,
		OpenBlock,
		As,
		GroupBy,
		And,
		Intersect,
		Union,
		Equals,
		Coalesce,
		OrderBy
	}
}
