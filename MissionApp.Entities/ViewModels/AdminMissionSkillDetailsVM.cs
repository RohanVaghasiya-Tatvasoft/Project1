
using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class AdminMissionSkillDetailsVM
    {
        public IEnumerable<Skill> SkillLists { get; set; } = new List<Skill>();
        public User UserInfo { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? SkillName { get; set; }

        public int SkillIds { get; set; }

        public int Status { get; set; }
    }
}
