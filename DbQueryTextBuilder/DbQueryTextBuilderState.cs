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
		Or,
		Intersect,
		Union,
		Equals,
		Coalesce,
		OrderBy
	}
}
