using SQLite;
using IntegraCamp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace IntegraCamp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        private readonly string[] Buildings = { "Солнечная", "Морская", "Жемчужная", "Звездная", "Изумрудная" };
        private readonly string[] Rooms =
        {
            "101","102","103","104","105","106","107","108",
            "201","202","203","204","205","206","207","208","209","210","211","212",
            "301","302","303","304","305","306","307","308","309","310","311","312"
        };

        public async Task Init()
        {
            if (_database != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "camp.db");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<Counselor>();
            await _database.CreateTableAsync<UserProfile>();

            var counselors = await _database.Table<Counselor>().CountAsync();
            var profile = await _database.Table<UserProfile>().CountAsync();

            if (counselors == 0)
                await SeedTestData();

            if (profile == 0)
                await SeedUserProfile();
        }

        private async Task SeedTestData()
        {
            var counselors = new List<Counselor>();
            int id = 1;
            var random = new Random();

            foreach (var building in Buildings)
            {
                for (int squad = 1; squad <= 8; squad++)
                {
                    for (int w = 1; w <= 2; w++)
                    {
                        string room = Rooms[random.Next(Rooms.Length)];
                        counselors.Add(new Counselor
                        {
                            Id = id++,
                            FullName = $"{building} {squad} отряд Вожатый {w}",
                            PhoneHash = SecurityService.HashString($"+7{900 + id}000{id}"),
                            SquadNumber = squad,
                            Building = building,
                            ChildrenCount = random.Next(15, 26),
                            RoomHash = SecurityService.HashString(room),
                            Status = "Закреплен"
                        });
                    }
                }
            }

            for (int i = 1; i <= 5; i++)
            {
                string building = Buildings[random.Next(Buildings.Length)];
                counselors.Add(new Counselor
                {
                    Id = id++,
                    FullName = $"Свободный вожатый {i}",
                    PhoneHash = SecurityService.HashString($"+7{900 + id}000{id}"),
                    SquadNumber = null,
                    Building = building,
                    ChildrenCount = 0,
                    RoomHash = SecurityService.HashString(Rooms[random.Next(Rooms.Length)]),
                    Status = "Свободен"
                });
            }

            await _database.InsertAllAsync(counselors);
        }

        private async Task SeedUserProfile()
        {
            var profile = new UserProfile
            {
                Id = 1,
                FullName = "Иванова Анна Петровна",
                Position = "Старший вожатый",
                Email = "anna@camp.ru",
                Phone = "+7 999 123-45-67"
            };
            await _database.InsertAsync(profile);
        }

        public Task<List<Counselor>> GetAllCounselors() => _database.Table<Counselor>().ToListAsync();
        public Task<Counselor> GetCounselor(int id) => _database.Table<Counselor>().FirstOrDefaultAsync(c => c.Id == id);
        public Task<int> SaveCounselor(Counselor counselor) => _database.InsertAsync(counselor);
        public Task<int> UpdateCounselor(Counselor counselor) => _database.UpdateAsync(counselor);
        public Task<int> DeleteCounselor(Counselor counselor) => _database.DeleteAsync(counselor);

        public Task<List<Counselor>> GetFreeCounselors() =>
            _database.Table<Counselor>().Where(c => c.Status == "Свободен").ToListAsync();

        public Task<int> GetFreeCount() =>
            _database.Table<Counselor>().CountAsync(c => c.Status == "Свободен");

        public Task<int> GetAssignedCount() =>
            _database.Table<Counselor>().CountAsync(c => c.Status == "Закреплен");

        public Task<List<Counselor>> GetCounselorsByBuilding(string building)
        {
            if (building == "Все")
                return _database.Table<Counselor>().ToListAsync();
            return _database.Table<Counselor>().Where(c => c.Building == building).ToListAsync();
        }

        public Task<UserProfile> GetUserProfile() => _database.Table<UserProfile>().FirstOrDefaultAsync();
        public Task<int> UpdateUserProfile(UserProfile profile) => _database.UpdateAsync(profile);
    }
}