using Microsoft.Data.SqlClient;

namespace MiniCRMProject.Models
{
    // Proje listesindeki arama, filtre ve sıralama seçimleri.
    // Kullanıcı girdisi yalnızca parametre olarak sorguya girer; sıralama sütunu beyaz listeden seçilir.
    public class ProjectListQuery
    {
        public string? Search { get; set; }
        public ProjectCategory? Category { get; set; }
        public ProjectStage? Stage { get; set; }
        public string Sort { get; set; } = "yeni";
        public int? CustomerId { get; set; }

        public bool HasFilter => !string.IsNullOrWhiteSpace(Search) || Category.HasValue || Stage.HasValue;

        public (string Sql, List<SqlParameter> Parameters) BuildWhereAndOrder()
        {
            List<string> conditions = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                conditions.Add("(Projects.ProjectName LIKE @search OR Users.FirstName + ' ' + Users.LastName LIKE @search OR Projects.District LIKE @search)");
                parameters.Add(new SqlParameter("@search", "%" + Search.Trim() + "%"));
            }
            if (Category.HasValue)
            {
                conditions.Add("Projects.Category = @category");
                parameters.Add(new SqlParameter("@category", (byte)Category.Value));
            }
            if (Stage.HasValue)
            {
                conditions.Add("Projects.Stage = @stage");
                parameters.Add(new SqlParameter("@stage", (byte)Stage.Value));
            }
            if (CustomerId.HasValue)
            {
                conditions.Add("Projects.CustomerId = @customerId");
                parameters.Add(new SqlParameter("@customerId", CustomerId.Value));
            }

            string where = conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";
            string order = Sort switch
            {
                "eski" => " ORDER BY Projects.CreatedAt ASC",
                "bitis" => " ORDER BY CASE WHEN Projects.EndDate IS NULL THEN 1 ELSE 0 END, Projects.EndDate ASC",
                _ => " ORDER BY Projects.CreatedAt DESC"
            };

            return (where + order, parameters);
        }
    }
}
