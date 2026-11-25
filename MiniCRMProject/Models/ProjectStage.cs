namespace MiniCRMProject.Models
{
    // İç mimarlık projesinin aşamaları, sırasıyla.
    public enum ProjectStage : byte
    {
        OlcuAlindi = 1,
        Konsept = 2,
        Cizim = 3,
        Uygulama = 4,
        TeslimEdildi = 5
    }

    public static class StageExtensions
    {
        public static string DisplayName(this ProjectStage stage) => stage switch
        {
            ProjectStage.OlcuAlindi => "Ölçü alındı",
            ProjectStage.Konsept => "Konsept",
            ProjectStage.Cizim => "Çizim",
            ProjectStage.Uygulama => "Uygulama",
            ProjectStage.TeslimEdildi => "Teslim edildi",
            _ => stage.ToString()
        };

        public static int ProgressPercent(this ProjectStage stage) => (byte)stage * 20;
    }
}
