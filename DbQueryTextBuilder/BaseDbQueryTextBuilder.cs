using System.Text;

namespace DbQueryTextBuilder
{
	public abstract class BaseDbQueryTextBuilder : IDbQueryTextBuilder
	{
		public DbQueryTextBuilderState State => _state;

		protected DbQueryTextBuilderState _state;
		protected readonly StringBuilder _stringBuilder;

		public BaseDbQueryTextBuilder()
		{
			_stringBuilder = new StringBuilder();
			_state = DbQueryTextBuilderState.None;
		}

		protected abstract string MapValueType(Type valueType);
		protected abstract string ValueQueryByType<T>(T value);

		public abstract IDbQueryTextBuilder OffsetLimit(int offset, int limit);

		public override string ToString()
		{
			return _stringBuilder.ToString();
		}

		#region Operators

		public IDbQueryTextBuilder OpenBlock() => Operator("(", state: DbQueryTextBuilderState.OpenBlock);

		public IDbQueryTextBuilder CloseBlock() => Operator(")", isSpace: false, state: DbQueryTextBuilderState.CloseBlock);

		public IDbQueryTextBuilder From() => Operator("FROM", isNewLine: true, state: DbQueryTextBuilderState.From);

		public IDbQueryTextBuilder GroupBy() => Operator("GROUP BY", isNewLine: true, state: DbQueryTextBuilderState.GroupBy);

		public IDbQueryTextBuilder Select() => Operator("SELECT", isNewLine: true, state: DbQueryTextBuilderState.Select);

		public IDbQueryTextBuilder End() => Operator(";", isSpace: false);

		public IDbQueryTextBuilder Not() => Operator("NOT");

		public IDbQueryTextBuilder Is() => Operator("IS");

		public IDbQueryTextBuilder Null() => Operator("NULL");

		public IDbQueryTextBuilder In() => Operator("IN");

		public IDbQueryTextBuilder Exists() => Operator("EXISTS");

		public IDbQueryTextBuilder Where() => Operator("WHERE", isNewLine: true, state: DbQueryTextBuilderState.Where);

		public IDbQueryTextBuilder And() => Operator("AND", isNewLine: true, isTab: true, state: DbQueryTextBuilderState.And);

		public IDbQueryTextBuilder Or() => Operator("OR", isNewLine: true, isTab: true, state: DbQueryTextBuilderState.Or);

		public IDbQueryTextBuilder On() => Operator("ON", isNewLine: true, isTab: true, state: DbQueryTextBuilderState.On);

		public IDbQueryTextBuilder Equals() => Operator("=", state: DbQueryTextBuilderState.Equals);

		public IDbQueryTextBuilder OrderBy() => Operator("ORDER BY", isNewLine: true, state: DbQueryTextBuilderState.OrderBy);

		public IDbQueryTextBuilder Desc() => Operator("DESC");

		public IDbQueryTextBuilder Operator(
			string name,
			bool isNewLine = false,
			bool isTab = false,
			bool isSpace = true,
			DbQueryTextBuilderState? state = null)
		{
			if (isNewLine)
			{
				_stringBuilder.AppendLine();
				isSpace = false;
			}
			if (isTab)
			{
				_stringBuilder.Append('\t');
				isSpace = false;
			}
			if (isSpace)
			{
				_stringBuilder.Append(' ');
			}
			_stringBuilder.Append(name);
			if (state.HasValue)
			{
				_state = state.Value;
			}
			return this;
		}

		#endregion

		public IDbQueryTextBuilder Column(string columnName, bool quote = true)
		{
			return InternalColumn($"{columnName.Quote(quote)}");
		}

		public IDbQueryTextBuilder Column(string tableName, string columnName)
		{
			return Column($"{tableName.Quote()}.{columnName.Quote()}", quote: false);
		}

		public IDbQueryTextBuilder InsertInto(string tableName, bool quote = true)
		{
			_stringBuilder.AppendLine();
			_stringBuilder.AppendFormat("INSERT INTO {0}", tableName.Quote(quote));
			return this;
		}

		public IDbQueryTextBuilder InsertInto(string tableSchema, string tableName)
		{
			_stringBuilder.AppendLine();
			_stringBuilder.AppendFormat("INSERT INTO {0}.{1}", tableSchema.Quote(), tableName.Quote());
			return this;
		}

		public IDbQueryTextBuilder Max(string columnName, bool quote = true)
		{
			return InternalColumn($"MAX({columnName.Quote(quote)})");
		}

		public IDbQueryTextBuilder Max(string tableName, string columnName)
		{
			return Max($"{tableName.Quote()}.{columnName.Quote()}", quote: false);
		}

		public IDbQueryTextBuilder InternalColumn(string columnName)
		{
			switch (_state)
			{
				case DbQueryTextBuilderState.Select:
				case DbQueryTextBuilderState.GroupBy:
				case DbQueryTextBuilderState.Where:
				case DbQueryTextBuilderState.OrderBy:
					_stringBuilder.AppendLine();
					_stringBuilder.Append('\t');
					break;
				case DbQueryTextBuilderState.Column:
					_stringBuilder.AppendLine(",");
					_stringBuilder.Append('\t');
					break;
				case DbQueryTextBuilderState.And:
				case DbQueryTextBuilderState.Or:
				case DbQueryTextBuilderState.Equals:
				case DbQueryTextBuilderState.On:
					_stringBuilder.Append(' ');
					break;
			}
			_stringBuilder.Append(columnName);
			_state = DbQueryTextBuilderState.Column;
			return this;
		}

		public IDbQueryTextBuilder As(string alias, bool quote = true)
		{
			_stringBuilder.AppendFormat(" AS {0}", alias.Quote(quote));
			return this;
		}

		public IDbQueryTextBuilder ColumnValue<T>(T value)
		{
			var valueQuery = ValueQueryByType(value);
			return Column(valueQuery, quote: false);
		}

		public IDbQueryTextBuilder From(string tableSchema, string tableName)
		{
			From();
			_stringBuilder.AppendFormat(" {0}.{1}", tableSchema.Quote(), tableName.Quote());
			return this;
		}

		public IDbQueryTextBuilder IsNotNull() => Is().Not().Null();

		public IDbQueryTextBuilder In<T>(IEnumerable<T> values)
		{
			In();
			OpenBlock();
			var valueQueries = values.Select(ValueQueryByType);
			AppendJoin(", ", valueQueries);
			CloseBlock();
			return this;
		}

		private void AppendJoin(string separator, IEnumerable<string> values)
		{
			foreach (string value in values)
			{
				_stringBuilder.Append(value);
				_stringBuilder.Append(separator);
			}

			if (_stringBuilder.Length > 0)
			{
				_stringBuilder.Length -= separator.Length;
			}
		}

		public IDbQueryTextBuilder And(string tableName, string columnName) => And().Column(tableName, columnName);

		public IDbQueryTextBuilder Intersect(Func<bool>? condition = null)
		{
			if (condition is null
				|| condition() == true)
			{
				return Operator("INTERSECT", isNewLine: true, state: DbQueryTextBuilderState.Intersect);
			}
			return this;
		}

		public IDbQueryTextBuilder UnionAll(Func<bool>? condition = null)
		{
			if (condition is null
				|| condition() == true)
			{
				return Operator("UNION ALL", isNewLine: true, state: DbQueryTextBuilderState.Union);
			}
			return this;
		}

		public IDbQueryTextBuilder Append(string value)
		{
			_stringBuilder.Append(value);
			return this;
		}

		public IDbQueryTextBuilder AppendLine(string value)
		{
			_stringBuilder.AppendLine(value);
			return this;
		}

		public IDbQueryTextBuilder InnerJoin(string tableSchema, string tableName)
		{
			return Join(tableSchema, tableName, "INNER");
		}

		public IDbQueryTextBuilder LeftJoin(string tableSchema, string tableName)
		{
			return Join(tableSchema, tableName, "LEFT");
		}

		private IDbQueryTextBuilder Join(string tableSchema, string tableName, string joinType)
		{
			_stringBuilder.AppendLine();
			_stringBuilder.AppendFormat("{0} JOIN", joinType);
			_stringBuilder.AppendFormat(" {0}.{1}", tableSchema.Quote(), tableName.Quote());
			return this;
		}

		public IDbQueryTextBuilder Append(Action<IDbQueryTextBuilder> action, Func<bool>? condition = null)
		{
			if (condition is null
				|| condition() == true)
			{
				action.Invoke(this);
			}
			return this;
		}

		public IDbQueryTextBuilder From(string tableName, bool quote = true)
		{
			From();
			_stringBuilder.AppendFormat(" {0}", tableName.Quote(quote));
			return this;
		}

		public virtual IDbQueryTextBuilder Coalesce()
		{
			_state = DbQueryTextBuilderState.Coalesce;
			return this;
		}

		public IDbQueryTextBuilder Parameter(string name)
		{
			return Column($"@{name}", quote: false);
		}
	}
}
