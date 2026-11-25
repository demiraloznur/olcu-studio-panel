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
        private readonly NoteRepository notes;
        private readonly PasswordService passwords;

        public DemoSeeder(UserRepository users, ProjectRepository projects, NoteRepository notes, PasswordService passwords)
        {
            this.users = users;
            this.projects = projects;
            this.notes = notes;
            this.passwords = passwords;
        }

        public void SeedIfEmpty()
        {
            if (users.Any()) return;

            AddUser("Zeynep", "Arslan", AdminEmail, AdminPassword, "Admin");
            int elif = AddUser("Elif", "Koç", "elif@olcustudio.test", CustomerPassword, "Customer");
            int mert = AddUser("Mert", "Aydın", "mert@olcustudio.test", CustomerPassword, "Customer");
            int selin = AddUser("Selin", "Tunç", "selin@olcustudio.test", CustomerPassword, "Customer");
            int can = AddUser("Can", "Özer", "can@olcustudio.test", CustomerPassword, "Customer");


            // (ad, açıklama, aşama, kategori, ilçe, müşteri, kaç gün önce başladı, bitişe kaç gün var)
            AddProject("Nilüfer daire", "Üç odalı dairenin oturma alanı ve mutfağının yenilenmesi.",
                ProjectStage.Konsept, ProjectCategory.Konut, "Nilüfer", elif, 40, 12);
            AddProject("Osmangazi kafe", "Mahalle kafesi için sıcak, ahşap ağırlıklı iç mekân.",
                ProjectStage.Cizim, ProjectCategory.Ticari, "Osmangazi", mert, 65, 25);
            AddProject("Görükle ofis", "Sekiz kişilik yazılım ofisi; toplantı odası ve sessiz çalışma alanı.",
                ProjectStage.Uygulama, ProjectCategory.Ofis, "Görükle", selin, 120, -5);
            AddProject("Mudanya yazlık", "Bahçe katı yazlığın deniz manzaralı salonu.",
                ProjectStage.TeslimEdildi, ProjectCategory.Konut, "Mudanya", elif, 200, -30);
            AddProject("Yıldırım butik", "Küçük giyim mağazası için vitrin ve raf düzeni.",
                ProjectStage.OlcuAlindi, ProjectCategory.Ticari, "Yıldırım", can, 3, null);
            AddProject("Nilüfer çocuk odası", "İki kardeş için büyüyebilen çocuk odası.",
                ProjectStage.OlcuAlindi, ProjectCategory.Konut, "Nilüfer", mert, 1, null);
            AddProject("Osmangazi muayenehane", "Diş hekimi muayenehanesi bekleme salonu.",
                ProjectStage.Konsept, ProjectCategory.Ofis, "Osmangazi", can, 20, 75);
            AddProject("Mudanya pansiyon", "Altı odalı pansiyonun lobi ve kahvaltı salonu.",
                ProjectStage.TeslimEdildi, ProjectCategory.Ticari, "Mudanya", selin, 300, -90);

            AddNotes();
        }

        // Her projeye aşamasına uygun 1–3 not.
        private void AddNotes()
        {
            string[] steps =
            {
                "Mekânda ölçü alındı, mevcut durum fotoğraflandı.",
                "Konsept sunumu yapıldı; renk ve malzeme yönü onaylandı.",
                "Uygulama çizimleri tamamlandı, metraj çıkarıldı.",
                "Uygulama başladı; ustalarla haftalık şantiye toplantısı yapılıyor.",
                "Proje teslim edildi, son kontroller tamamlandı."
            };

            foreach (ProjectModel project in projects.List(new ProjectListQuery { Sort = "eski" }))
            {
                int count = Math.Min((int)project.Stage, 3);
                int first = (int)project.Stage - count;
                for (int i = 0; i < count; i++)
                {
                    notes.Add(new ProjectNote
                    {
                        ProjectId = project.ProjectId,
                        Body = steps[first + i],
                        CreatedAt = project.CreatedAt.AddDays(7 * (i + 1))
                    });
                }
            }
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

        private void AddProject(string name, string description, ProjectStage stage, ProjectCategory category,
            string district, int customerId, int daysSinceStart, int? daysUntilEnd)
        {
            projects.Add(new ProjectModel
            {
                ProjectName = name,
                Description = description,
                Stage = stage,
                Category = category,
                District = district,
                CustomerId = customerId,
                CreatedAt = DateTime.Today.AddDays(-daysSinceStart),
                EndDate = daysUntilEnd.HasValue ? DateTime.Today.AddDays(daysUntilEnd.Value) : null
            });
        }
    }
}
