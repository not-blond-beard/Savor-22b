namespace GQL {
	public static class Queries
	{
		public static string GetVillages => @"
			query {
			villages {
				id
				name
				width
				height
				worldX
				worldY
			}
			}";
	}
}
