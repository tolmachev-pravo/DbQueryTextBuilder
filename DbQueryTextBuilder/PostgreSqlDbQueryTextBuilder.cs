namespace DbQueryTextBuilder
{
	public class PostgreSqlDbQueryTextBuilder : BaseDbQueryTextBuilder
	{
		protected override string ValueQueryByType<T>(T value)
		{
			switch (value)
			{
				case int intValue:
					return string.Format("{0}", intValue);
				case bool boolValue:
					var formatValue = boolValue ? "true" : "false";
					return string.Format("'{0}'", formatValue);
				case string stringValue:
					return string.Format("E'{0}'", stringValue.Replace(@"\", @"\\").Replace("'", @"\'"));
				default:
					var type = MapValueType(typeof(T));
					return string.Format("CAST('{0}' AS {1})", value, type);
			}
		}

		protected override string MapValueType(Type valueType)
		{
			switch (valueType)
			{
				case Type type when type == typeof(Guid):
					return "uuid";
				case Type type when type == typeof(int):
					return "int";
				default:
					throw new NotImplementedException("Not supported value type");
			}
		}

		public override IDbQueryTextBuilder OffsetLimit(int offset, int limit)
		{
			_stringBuilder.AppendLine();
			_stringBuilder.AppendFormat("OFFSET {0} LIMIT {1}", offset, limit);
			return this;
		}

		public override IDbQueryTextBuilder Coalesce()
		{
			InternalColumn("COALESCE");
			base.Coalesce();
			return this;
		}
	}
}
