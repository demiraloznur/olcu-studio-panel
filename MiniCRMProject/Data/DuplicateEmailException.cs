namespace MiniCRMProject.Data
{
    // Aynı e-posta ile ikinci bir kullanıcı eklenmek istendiğinde fırlatılır.
    // EmailExists kontrolünden sonra, aynı anda gelen bir başka kayıt araya girebilir;
    // son güvence veri tabanındaki benzersizlik kuralıdır.
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(Exception inner)
            : base("Bu e-posta ile zaten bir hesap var.", inner)
        {
        }
    }
}
