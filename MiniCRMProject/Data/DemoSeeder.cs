using MiniCRMProject.Models;
using MiniCRMProject.Services;

namespace MiniCRMProject.Data
{
    // Boş bir veri tabanına uydurma demo hesapları ve projeleri ekler.
    // Şifreler düz metin olarak değil, PasswordService ile karıştırılarak kaydedilir.
    public class DemoSeeder
    {
        public const string AdminEmail = "yonetici@olcustudio.test";
        public const string AdminPassword = "Olcu2026!";
        public const string CustomerPassword = "Musteri2026!";

        private readonly UserRepository users;
        private readonly ProjectRepository projects;
        private readonly PasswordService passwords;

        public DemoSeeder(UserRepository users, ProjectRepository projects, PasswordService passwords)
        {
            this.users = users;
            this.projects = projects;
            this.passwords = passwords;
        }

        public void SeedIfEmpty()
        {
            if (users.Any()) return;

            AddUser("Zeynep", "Arslan", AdminEmail, AdminPassword, "Admin");
            int elif = AddUser("Elif", "Koç", "elif@olcustudio.test", CustomerPassword, "Customer");
            int mert = AddUser("Mert", "Aydın", "mert@olcustudio.test", CustomerPassword, "Customer");
            int selin = AddUser("Selin", "Tunç", "selin@olcustudio.test", CustomerPassword, "Customer");
            AddUser("Can", "Özer", "can@olcustudio.test", CustomerPassword, "Customer");

            AddProject("Nilüfer daire", "Üç odalı dairenin yenilenmesi", "Tasarımda", elif, 45);
            AddProject("Osmangazi kafe", "Mahalle kafesi iç mekânı", "Analiz Ediliyor", mert, 60);
            AddProject("Görükle ofis", "Küçük yazılım ofisi", "Kodlanıyor", selin, 20);
            AddProject("Mudanya yazlık", "Bahçe katı yenileme", "Yayında", elif, null);
        }

        private int AddUser(string firstName, string lastName, string email, string password, string role)
        {
            return users.Add(new UserModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = passwords.Hash(password),
                Role = role
            });
        }

        private void AddProject(string name, string description, string status, int customerId, int? daysUntilEnd)
        {
            projects.Add(new ProjectModel
            {
                ProjectName = name,
                Description = description,
                Status = status,
                CustomerId = customerId,
                EndDate = daysUntilEnd.HasValue ? DateTime.Today.AddDays(daysUntilEnd.Value) : null
            });
        }
    }
}
