using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class AdminBannerDetailsVM
    {
        public IEnumerable<Banner> BannerLists { get; set; } = new List<Banner>();
        public User UserInfo { get; set; }

        public string? BannerText { get; set; }

        public int? BannerNumber { get; set; }

        public int BannerId { get; set; }

        public string? Banner { get; set; }
    }
}
