using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class AdminMissionThemeDetailsVM
    {
        public IEnumerable<MissionTheme> MissionThemeLists { get; set; } = new List<MissionTheme>();
    }
}
