using Microsoft.AspNetCore.Mvc;
using MissionApp.DataAccess.GenericRepository.Interface;
using MissionApp.Entities.Models;
using MissionApp.Entities.ViewModels;

namespace MissionApp.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class AdminController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult AdminUserDetails()
        {
            IEnumerable<User> userLists = _unitOfWork.User.GetAll();
            AdminUserDetailsVM obj = new();
            obj.UserLists = userLists;
            return View(obj);
        }

        //CMS lists
        public IActionResult AdminCMSPage()
        {
            IEnumerable<CmsPage> cmsLists = _unitOfWork.CMSPage.GetAll();
            AdminCMSPageDetailsVM obj = new()
            {
                CMSLists = cmsLists
            };
            return View(obj);
        }


        //Mission lists
        public IActionResult AdminMissionDetails()
        {
            IEnumerable<Mission> missionLists = _unitOfWork.Mission.GetAll();
            AdminMissionDetailsVM obj = new()
            {
                MissionLists = missionLists
            };
            return View(obj);
        }


        //MissionTheme lists
        public IActionResult AdminMissionThemeDetails()
        {
            IEnumerable<MissionTheme> missionThemeLists = _unitOfWork.MissionTheme.GetAll();
            AdminMissionThemeDetailsVM obj = new()
            {
                MissionThemeLists = missionThemeLists,
            };
            return View(obj);
        }


        //Skill Lists
        public IActionResult AdminMissionSkillDetails()
        {
            IEnumerable<Skill> SkillLists = _unitOfWork.Skill.GetAll();
            AdminMissionSkillDetailsVM obj = new()
            {
                SkillLists = SkillLists,
            };
            return View(obj);
        }


        //MissionApplication Lists
        public IActionResult AdminMissionApplicationDetails()
        {
            IEnumerable<MissionApplication> missionAppLists = _unitOfWork.MissionApplication.GetAllMissionApplicationList();
            AdminMissionApplicationDetailsVM obj = new()
            {
                MissionApplicationLists = missionAppLists,
            };
            return View(obj);
        }


        //Story lists
        public IActionResult AdminStoryDetails()
        {
            IEnumerable<Story> storyLists = _unitOfWork.Story.GetAllStory();
            AdminStoryDetailsVM obj = new()
            {
                StoryLists = storyLists,
            };
            return View(obj);
        }


        //Banner Lists
        public IActionResult AdminBannerDetails()
        {
            IEnumerable<Banner> bannerLists = _unitOfWork.Banner.GetAll();
            AdminBannerDetailsVM obj = new()
            {
                BannerLists = bannerLists,
            };
            return View(obj);
        }
    }
}
