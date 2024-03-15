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

		public IDbQueryTextBuilder CloseBlock()
		{
			_stringBuilder.Append(")");
			_state = DbQueryTextBuilderState.CloseBlock;
			return this;
		}

		public IDbQueryTextBuilder Column(string columnName, bool quote = true)
		{
			return InternalColumn($"{columnName.Quote(quote)}");
		}

		public IDbQueryTextBuilder Column(string tableName, string columnName)
		{
			return Column($"{tableName.Quote()}.{columnName.Quote()}", quote: false);
		}

		public IDbQueryTextBuilder From()
		{
			_stringBuilder.AppendLine();
			_stringBuilder.Append("FROM");
			return this;
		}

		public IDbQueryTextBuilder GroupBy()
		{
			_stringBuilder.AppendLine();
			_stringBuilder.AppendLine("GROUP BY ");
			_state = DbQueryTextBuilderState.GroupBy;
			return this;
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

		public IDbQueryTextBuilder OpenBlock()
		{
			_stringBuilder.Append(" (");
			_state = DbQueryTextBuilderState.OpenBlock;
			return this;
		}

		public IDbQueryTextBuilder Select()
		{
			_stringBuilder.AppendLine();
			_stringBuilder.Append("SELECT");
			_state = DbQueryTextBuilderState.Select;
			return this;
		}

		public IDbQueryTextBuilder InternalColumn(string columnName)
		{
			switch (_state)
			{
				case DbQueryTextBuilderState.Select:
				case DbQueryTextBuilderState.GroupBy:
				case DbQueryTextBuilderState.Where:
				case DbQueryTextBuilderState.On:
				case DbQueryTextBuilderState.OrderBy:
					_stringBuilder.AppendLine();
					_stringBuilder.Append('\t');
					break;
				case DbQueryTextBuilderState.Column:
					_stringBuilder.AppendLine(",");
					_stringBuilder.Append('\t');
					break;
				case DbQueryTextBuilderState.And:
				case DbQueryTextBuilderState.Equals:
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

		public IDbQueryTextBuilder End()
		{
			_stringBuilder.Append(';');
			return this;
		}

		protected abstract string MapValueType(Type valueType);
		protected abstract string ValueQueryByType<T>(T value);

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

		public IDbQueryTextBuilder Where()
		{
			_stringBuilder.AppendLine();
			_stringBuilder.Append("WHERE");
			_state = DbQueryTextBuilderState.Where;
			return this;
		}

		public IDbQueryTextBuilder IsNotNull()
		{
			_stringBuilder.Append(" IS NOT NULL");
			return this;
		}

		public IDbQueryTextBuilder Is()
		{
			_stringBuilder.Append(" IS");
			return this;
		}

		public IDbQueryTextBuilder Null()
		{
			_stringBuilder.Append(" NULL");
			return this;
		}

		public IDbQueryTextBuilder In()
		{
			_stringBuilder.Append(" IN");
			return this;
		}

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

		public IDbQueryTextBuilder And()
		{
			_stringBuilder.AppendLine();
			_stringBuilder.Append('\t');
			_stringBuilder.Append("AND");
			_state = DbQueryTextBuilderState.And;
			return this;
		}

		public IDbQueryTextBuilder Or()
		{
			_stringBuilder.AppendLine();
			_stringBuilder.Append('\t');
			_stringBuilder.Append("OR");
			_state = DbQueryTextBuilderState.And;
			return this;
		}

		public IDbQueryTextBuilder And(string tableName, string columnName)
		{
			And();
			Column(tableName, columnName);
			return this;
		}

		public IDbQueryTextBuilder Intersect(Func<bool>? condition = null)
		{
			if (condition is null
				|| condition() == true)
			{
				_stringBuilder.AppendLine();
				_stringBuilder.Append("INTERSECT");
				_state = DbQueryTextBuilderState.Intersect;
			}
			return this;
		}

		public IDbQueryTextBuilder UnionAll(Func<bool>? condition = null)
		{
			if (condition is null
				|| condition() == true)
			{
				_stringBuilder.AppendLine();
				_stringBuilder.Append("UNION ALL");
				_state = DbQueryTextBuilderState.Union;
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

		public IDbQueryTextBuilder On()
		{
			_stringBuilder.Append(" ON");
			_state = DbQueryTextBuilderState.On;
			return this;
		}

		public IDbQueryTextBuilder Equals()
		{
			_stringBuilder.Append(" =");
			_state = DbQueryTextBuilderState.Equals;
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

		public abstract IDbQueryTextBuilder OffsetLimit(int offset, int limit);

		public IDbQueryTextBuilder OrderBy()
		{
			_stringBuilder.AppendLine();
			_stringBuilder.Append("ORDER BY");
			_state = DbQueryTextBuilderState.OrderBy;
			return this;
		}

		public IDbQueryTextBuilder Desc()
		{
			_stringBuilder.Append(" DESC");
			return this;
		}

		public override string ToString()
		{
			return _stringBuilder.ToString();
		}
	}
}
