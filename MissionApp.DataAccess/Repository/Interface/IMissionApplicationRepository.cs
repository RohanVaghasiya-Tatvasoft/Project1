﻿using MissionApp.DataAccess.GenericRepository.Interface;
using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.DataAccess.Repository.Interface
{
    public interface IMissionApplicationRepository : IGenericRepository<MissionApplication>
    {
        IEnumerable<MissionApplication> GetMissionApplicationList(Expression<Func<MissionApplication, bool>> filter);
        IEnumerable<MissionApplication> GetAllMissionApplicationList();
    }
}
