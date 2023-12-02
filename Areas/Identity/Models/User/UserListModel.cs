using System.Collections.Generic;
using MobileWeb.Models.Entities;

namespace MobileWeb.Areas.Identity.Models.UserViewModels
{
    public class UserListModel
    {
        public int TotalUsers { get; set; }
        public int CountPages { get; set; }

        public int ITEMS_PER_PAGE { get; set; } = 10;

        public int CurrentPage { get; set; }

        public List<UserAndRole>? Users { get; set; }

    }

    public class UserAndRole : User
    {
        public string? RoleNames { get; set; }
    }


}