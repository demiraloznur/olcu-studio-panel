namespace MiniCRMProject.Models
{
    public enum ProjectCategory : byte
    {
        Konut = 1,
        Ofis = 2,
        Ticari = 3
    }

    public static class CategoryExtensions
    {
        public static string DisplayName(this ProjectCategory category) => category switch
        {
            ProjectCategory.Konut => "Konut",
            ProjectCategory.Ofis => "Ofis",
            ProjectCategory.Ticari => "Ticari",
            _ => category.ToString()
        };
    }
}
