namespace MiniCRMProject.Security
{
    // Oturumdaki kullanıcı. Anahtarlar okul sürümündekiyle aynıdır.
    public record SessionUser(int Id, string FullName, string Role)
    {
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        private const string IdKey = "UserId";
        private const string NameKey = "UserFullName";
        private const string RoleKey = "UserRole";

        public bool IsAdmin => Role == AdminRole;

        public static SessionUser? From(ISession session)
        {
            int? id = session.GetInt32(IdKey);
            string? role = session.GetString(RoleKey);
            if (id == null || string.IsNullOrEmpty(role)) return null;

            return new SessionUser(id.Value, session.GetString(NameKey) ?? "", role);
        }

        public void WriteTo(ISession session)
        {
            session.SetInt32(IdKey, Id);
            session.SetString(NameKey, FullName);
            session.SetString(RoleKey, Role);
        }
    }
}
