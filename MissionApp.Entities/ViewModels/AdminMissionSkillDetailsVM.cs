﻿
using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class AdminMissionSkillDetailsVM
    {
        public IEnumerable<Skill> SkillLists { get; set; } = new List<Skill>();
    }
}
