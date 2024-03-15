namespace DbQueryTextBuilder
{
	public static class StringDbQueryTextBuilderExtensions
	{
		public static string Quote(this string value, bool quote = true)
		{
			return quote
				? $@"""{value}"""
				: value;
		}

		public static string ToOneLine(this string value)
		{
			return value
				.Replace(Environment.NewLine, " ")
				.Replace("\t", string.Empty);
		}
	}
}
