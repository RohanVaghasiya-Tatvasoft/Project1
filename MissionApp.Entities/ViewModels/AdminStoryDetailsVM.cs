﻿using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class AdminStoryDetailsVM
    {
        public IEnumerable<Story> StoryLists { get; set; } = new List<Story>();
        public User UserInfo { get; set; }

        public User? UserOfStory { get; set; }
    }
}
