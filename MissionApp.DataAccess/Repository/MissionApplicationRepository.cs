using Microsoft.EntityFrameworkCore;
using MissionApp.DataAccess.GenericRepository;
using MissionApp.DataAccess.GenericRepository.Interface;
using MissionApp.DataAccess.Repository.Interface;
using MissionApp.Entities.Data;
using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.DataAccess.Repository
{
    public class MissionApplicationRepository : GenericRepository<MissionApplication>, IMissionApplicationRepository
    {
        public MissionApplicationRepository(ApplicationDbContext context) : base(context)
        {
        }
        public IEnumerable<MissionApplication> GetMissionApplicationList(Expression<Func<MissionApplication, bool>> predicate)
        {
            IQueryable<MissionApplication> query = dbSet;
            query = query.Where(predicate).Include(m => m.Mission).Include(m => m.Mission.City);
            return query.ToList();
        }
        public IEnumerable<MissionApplication> GetAllMissionApplicationList()
        {
            IQueryable<MissionApplication> query = dbSet;
            query = query.Include(m => m.Mission).Include(m => m.User);
            return query.ToList();
        }
    }
}
