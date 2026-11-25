using MiniCRMProject.Models;

namespace MiniCRMProject.Security
{
    public static class ProjectAccess
    {
        // Yönetici her projeyi, müşteri yalnızca kendi projesini görebilir.
        public static bool CanView(ProjectModel project, SessionUser user)
        {
            return user.IsAdmin || project.CustomerId == user.Id;
        }

        public static bool CanEdit(SessionUser user)
        {
            return user.IsAdmin;
        }
    }
}
