namespace DbQueryTextBuilder
{
	public class MSSqlDbQueryTextBuilder : BaseDbQueryTextBuilder
	{
		public override string FormatValue<T>(T value)
		{
			switch (value)
			{
				case int intValue:
					return string.Format("{0}", intValue);
				case bool boolValue:
					var formatValue = boolValue ? "true" : "false";
					return string.Format("'{0}'", formatValue);
				case string stringValue:
					return string.Format("N'{0}'", stringValue.Replace("'", "''"));
				default:
					return string.Format("'{0}'", value);
			}
		}

		protected override string MapValueType(Type valueType)
		{
			switch (valueType)
			{
				case Type type when type == typeof(Guid):
					return "uniqueidentifier";
				case Type type when type == typeof(int):
					return "int";
				default:
					throw new NotImplementedException("Not supported value type");
			}
		}

		public override IDbQueryTextBuilder OffsetLimit(int offset, int limit)
		{
			_stringBuilder.AppendLine();
			_stringBuilder.AppendFormat("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, limit);
			return this;
		}

		public override IDbQueryTextBuilder Coalesce()
		{
			InternalColumn("ISNULL");
			base.Coalesce();
			return this;
		}
	}
}
