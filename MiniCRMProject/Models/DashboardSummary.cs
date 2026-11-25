namespace MiniCRMProject.Models
{
    // Yöneticinin özet sayfası için hesaplar. Veri tabanından bağımsızdır; test edilebilir.
    public class DashboardSummary
    {
        public const int UpcomingDays = 30;

        public int Total { get; private init; }
        public IReadOnlyDictionary<ProjectStage, int> ByStage { get; private init; } = new Dictionary<ProjectStage, int>();
        public IReadOnlyList<ProjectModel> Upcoming { get; private init; } = Array.Empty<ProjectModel>();
        public IReadOnlyList<ProjectModel> Overdue { get; private init; } = Array.Empty<ProjectModel>();

        public static DashboardSummary From(IEnumerable<ProjectModel> projects, DateTime today)
        {
            List<ProjectModel> list = projects.ToList();
            DateTime day = today.Date;
            List<ProjectModel> open = list
                .Where(p => p.Stage != ProjectStage.TeslimEdildi && p.EndDate.HasValue)
                .ToList();

            return new DashboardSummary
            {
                Total = list.Count,
                ByStage = Enum.GetValues<ProjectStage>().ToDictionary(s => s, s => list.Count(p => p.Stage == s)),
                Upcoming = open
                    .Where(p => p.EndDate!.Value.Date >= day && p.EndDate.Value.Date <= day.AddDays(UpcomingDays))
                    .OrderBy(p => p.EndDate)
                    .ToList(),
                Overdue = open
                    .Where(p => p.EndDate!.Value.Date < day)
                    .OrderBy(p => p.EndDate)
                    .ToList()
            };
        }
    }
}
