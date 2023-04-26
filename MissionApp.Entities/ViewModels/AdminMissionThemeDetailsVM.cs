using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class AdminMissionThemeDetailsVM
    {
        public IEnumerable<MissionTheme> MissionThemeLists { get; set; } = new List<MissionTheme>();
        public User UserInfo { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? MissionThemeTitle { get; set; }

        public int MissionThemeId { get; set; }

        public int Status { get; set; }
    }
}
