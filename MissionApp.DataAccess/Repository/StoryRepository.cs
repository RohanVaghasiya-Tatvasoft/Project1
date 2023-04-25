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
    public class StoryRepository : GenericRepository<Story>, IStoryRepository
    {
        public StoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Story GetStoryAndMedia(Expression<Func<Story, bool>> predicate)
        {
            IQueryable<Story> query = dbSet;
            query = query.Where(predicate).Include(story => story.StoryMedia);
            return query.FirstOrDefault();
        }
        public IEnumerable<Story> GetAllStory()
        {
            IQueryable<Story> query = dbSet;
            query = query.Include(story => story.User).Include(story => story.Mission);
            return query.ToList();
        }
    }
}
