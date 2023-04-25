using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class AdminMissionDetailsVM
    {
        public IEnumerable<Mission> MissionLists { get; set; } = new List<Mission>();
    }
}
