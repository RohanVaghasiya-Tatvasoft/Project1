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
    }
}
