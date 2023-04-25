﻿using MissionApp.DataAccess.GenericRepository;
using MissionApp.DataAccess.GenericRepository.Interface;
using MissionApp.DataAccess.Repository.Interface;
using MissionApp.Entities.Data;
using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.DataAccess.Repository
{
    public class GoalMissionRepository : GenericRepository<GoalMission>, IGoalMissionRepository
    {
        public GoalMissionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
