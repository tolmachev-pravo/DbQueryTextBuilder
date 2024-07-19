namespace DbQueryTextBuilder
{
	public interface IDbQueryTextBuilder
	{
		DbQueryTextBuilderState State { get; }

		string FormatValue<T>(T value);
		IDbQueryTextBuilder InsertInto(string tableName, bool quote = true);
		IDbQueryTextBuilder InsertInto(string tableSchema, string tableName);
		IDbQueryTextBuilder Select();
		IDbQueryTextBuilder Column(string columnName, bool quote = true);
		IDbQueryTextBuilder Column(string tableName, string columnName);
		IDbQueryTextBuilder Max(string columnName, bool quote = true);
		IDbQueryTextBuilder Max(string tableName, string columnName);
		IDbQueryTextBuilder From();
		IDbQueryTextBuilder From(string tableName, bool quote = true);
		IDbQueryTextBuilder From(string tableSchema, string tableName);
		IDbQueryTextBuilder OpenBlock();
		IDbQueryTextBuilder CloseBlock();
		IDbQueryTextBuilder GroupBy();
		IDbQueryTextBuilder As(string alias, bool quote = true);
		IDbQueryTextBuilder End();
		IDbQueryTextBuilder ColumnValue<T>(T value);
		IDbQueryTextBuilder Where();
		IDbQueryTextBuilder IsNotNull();
		IDbQueryTextBuilder In<T>(IEnumerable<T> values);
		IDbQueryTextBuilder In();
		IDbQueryTextBuilder And(Func<bool>? condition = null);
		IDbQueryTextBuilder Or(Func<bool>? condition = null);
		IDbQueryTextBuilder And(string tableName, string columnName);
		IDbQueryTextBuilder Intersect(Func<bool>? condition = null);
		IDbQueryTextBuilder UnionAll(Func<bool>? condition = null);
		IDbQueryTextBuilder Append(string value);
		IDbQueryTextBuilder AppendLine(string value);
		IDbQueryTextBuilder AppendFormat(string format, params object?[] arguments);
		IDbQueryTextBuilder InnerJoin(string tableSchema, string tableName);
		IDbQueryTextBuilder LeftJoin(string tableSchema, string tableName);
		IDbQueryTextBuilder On();
		IDbQueryTextBuilder Equals();
		IDbQueryTextBuilder Append(Action<IDbQueryTextBuilder> action, Func<bool>? condition = null);
		IDbQueryTextBuilder Coalesce();
		IDbQueryTextBuilder Is();
		IDbQueryTextBuilder Null();
		IDbQueryTextBuilder OffsetLimit(int offset, int limit);
		IDbQueryTextBuilder OrderBy();
		IDbQueryTextBuilder Desc();
		IDbQueryTextBuilder Parameter(string name);
		IDbQueryTextBuilder Not();
		IDbQueryTextBuilder Exists();
		IDbQueryTextBuilder Operator(
			string name,
			bool isNewLine = false,
			bool isTab = false,
			bool isSpace = true,
			DbQueryTextBuilderState? state = null);
	}
}
