using SQLite;

namespace IntegraCamp.Models
{
	[SQLite.Table("Counselors")]
	public class Counselor
	{
		[SQLite.PrimaryKey, SQLite.AutoIncrement]
		public int Id { get; set; }

		public string FullName { get; set; } = string.Empty;

		public string PhoneHash { get; set; } = string.Empty;

		public int? SquadNumber { get; set; }

		public string Building { get; set; } = string.Empty;

		public int ChildrenCount { get; set; }

		public string RoomHash { get; set; } = string.Empty;

		public string Status { get; set; } = string.Empty;
	}
}