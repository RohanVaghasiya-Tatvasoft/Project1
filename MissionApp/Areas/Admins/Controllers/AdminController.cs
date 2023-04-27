using Microsoft.AspNetCore.Mvc;
using MissionApp.DataAccess.GenericRepository.Interface;
using MissionApp.DataAccess.MethodRepository.Interface;
using MissionApp.Entities.Models;
using MissionApp.Entities.ViewModels;
using System.Security.Claims;

namespace MissionApp.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class AdminController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoryMethodRepository _repo;
        private readonly IWebHostEnvironment _iweb;
        public AdminController(IUnitOfWork unitOfWork, IStoryMethodRepository storyMethodRepository, IWebHostEnvironment iweb)
        {
            _unitOfWork = unitOfWork;
            _repo = storyMethodRepository;
            _iweb = iweb;
        }

        public User GetThisUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var user = _unitOfWork.User.GetFirstOrDefault(m => m.Email == email);
            return user;
        }

        public IActionResult AdminUserDetails()
        {
            User user = GetThisUser();
            IEnumerable<User> userLists = _unitOfWork.User.GetAccToFilter(m => m.DeletedAt == null);
            AdminUserDetailsVM obj = new();
            obj.UserLists = userLists;
            obj.UserInfo = user;
            return View(obj);
        }

        [HttpPost]
        public JsonResult CascadeCity(int countryId)
        {
            IEnumerable<City> cityList = _unitOfWork.City.GetAccToFilter(cities => cities.CountryId == countryId);
            return new JsonResult(cityList);
        }

        [HttpGet]
        public IActionResult AdminAddUser()
        {
            User user = GetThisUser();
            IEnumerable<Country> Countries = _unitOfWork.Country.GetAll();
            AdminUserDetailsVM obj = new()
            {
                UserInfo = user,
                CountryList = Countries
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult AdminAddUser(AdminUserDetailsVM obj, IFormFile userAvatar)
        {
            User? status = _unitOfWork.User.GetFirstOrDefault(m => m.Email.ToLower() == obj.Email.Trim().ToLower());

            if (status == null)
            {
                User user = new()
                {
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    PhoneNumber = (long)obj.PhoneNumber,
                    Email = obj.Email,
                    Password = obj.Password,
                    CountryId = obj.CountryId,
                    CityId = obj.CityId,
                    EmployeeId = obj.EmployeeId,
                    Department = obj.Department
                };
                _unitOfWork.User.Add(user);
                _unitOfWork.Save();

                if (userAvatar != null)
                {
                    User userForAvatar = _unitOfWork.User.GetFirstOrDefault(m => m.Email.Trim().ToLower() == obj.Email.Trim().ToLower());
                    string imgExt = Path.GetExtension(userAvatar.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        string ImageName = user.UserId + Path.GetFileName(userAvatar.FileName);
                        var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Avatars/", ImageName);
                        string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                        using (FileStream stream = new(finalPath, FileMode.Create))
                        {
                            userAvatar.CopyTo(stream);
                        }
                        user.Avatar = imgSaveTo;
                        user.UpdatedAt = DateTime.Now;
                        _unitOfWork.User.Update(user);
                        _unitOfWork.Save();
                    }
                }
                TempData["success"] = "User added successfully!";
                return RedirectToAction("AdminUserDetails");

            }
            TempData["error"] = "Email Already Exists!";
            IEnumerable<Country> countryList = _unitOfWork.Country.GetAll();
            obj.CountryList = countryList;
            return View(obj);
        }

        [HttpPost]
        public IActionResult DeleteUserData(int userId)
        {
            if (userId > 0)
            {
                User deletingData = _unitOfWork.User.GetFirstOrDefault(m => m.UserId == userId);
                string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + deletingData.Avatar);
                System.IO.File.Delete(alrExists);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.User.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminUserDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminUserDetails");
        }

        [HttpPost]
        public JsonResult CascadeCityForEdit(int countryId, int userId)
        {
            User user = new();
            int cityId = 0;
            if (userId > 0)
            {
                user = _unitOfWork.User.GetFirstOrDefault(user => user.UserId == userId);
                if (user != null)
                {
                    cityId = user.CityId;
                }
            }
            IEnumerable<City> cityList = _unitOfWork.City.GetAccToFilter(city => city.CountryId == countryId);
            return new JsonResult(new { CityId = cityId, Cities = cityList });
        }

        [HttpGet]
        public IActionResult AdminEditUser(int userId)
        {
            User userInfo = GetThisUser();
            IEnumerable<Country> countryList = _unitOfWork.Country.GetAll();
            AdminUserDetailsVM obj = new();
            if (userId > 0)
            {
                User user = _unitOfWork.User.GetFirstOrDefault(m => m.UserId == userId);
                if (User != null)
                {
                    obj.UserId = user.UserId;
                    obj.Avatar = user.Avatar;
                    obj.FirstName = user.FirstName;
                    obj.LastName = user.LastName;
                    obj.PhoneNumber = user.PhoneNumber;
                    obj.Email = user.Email;
                    obj.EmployeeId = user.EmployeeId;
                    obj.Department = user.Department;
                    obj.CityId = user.CityId;
                    obj.CountryId = user.CountryId;
                    obj.UserInfo = userInfo;
                }
            }
            obj.CountryList = countryList;
            return View(obj);
        }

        [HttpPost]
        public IActionResult AdminEditUser(AdminUserDetailsVM obj, IFormFile userAvatar)
        {
            if (obj.UserId > 0)
            {
                User user = _unitOfWork.User.GetFirstOrDefault(m => m.UserId == obj.UserId);
                user.FirstName = obj.FirstName;
                user.LastName = obj.LastName;
                user.PhoneNumber = (long)obj.PhoneNumber;
                user.Email = obj.Email;
                user.EmployeeId = obj.EmployeeId;
                user.Department = obj.Department;
                user.CityId = obj.CityId;
                user.CountryId = obj.CountryId;

                if (user != null && userAvatar != null)
                {
                    string imgExt = Path.GetExtension(userAvatar.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        if (user.Avatar != null)
                        {
                            string ImageName = user.UserId + Path.GetFileName(userAvatar.FileName);
                            var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Avatars/", ImageName);
                            string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                            if (!imgSaveTo.Equals(user.Avatar))
                            {
                                string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + user.Avatar);
                                if (System.IO.File.Exists(alrExists))
                                {
                                    System.IO.File.Delete(alrExists);
                                }

                                using (FileStream stream = new(finalPath, FileMode.Create))
                                {
                                    userAvatar.CopyTo(stream);
                                }
                                user.Avatar = imgSaveTo;
                                user.UpdatedAt = DateTime.Now;
                                _unitOfWork.User.Update(user);
                            }
                        }
                        else
                        {
                            string ImageName = user.UserId + Path.GetFileName(userAvatar.FileName);
                            var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Avatars/", ImageName);
                            string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                            using (FileStream stream = new(finalPath, FileMode.Create))
                            {
                                userAvatar.CopyTo(stream);
                            }
                            user.Avatar = imgSaveTo;
                            user.UpdatedAt = DateTime.Now;
                            _unitOfWork.User.Update(user);
                        }
                    }
                }


                _unitOfWork.User.Update(user);
                _unitOfWork.Save();
                TempData["success"] = "User edited successfully!";
                return RedirectToAction("AdminUserDetails");
            }
            TempData["error"] = "Something went wrong";
            return View(obj);
        }


        //CMS lists
        public IActionResult AdminCMSPage()
        {
            User user = GetThisUser();
            IEnumerable<CmsPage> cmsLists = _unitOfWork.CMSPage.GetAccToFilter(m => m.DeletedAt == null);
            AdminCMSPageDetailsVM obj = new()
            {
                CMSLists = cmsLists,
                UserInfo = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult GetCMSData(int CMSId)
        {
            CmsPage cmsPageData = _unitOfWork.CMSPage.GetFirstOrDefault(m => m.CmsPageId == CMSId);
            return Json(cmsPageData);
        }

        [HttpGet]
        public IActionResult AdminAddMission()
        {
            User user = GetThisUser();
            IEnumerable<Country> Countries = _unitOfWork.Country.GetAll();
            IEnumerable<MissionTheme> themeList = _unitOfWork.MissionTheme.GetAll();
            IEnumerable<Skill> skillList = _unitOfWork.Skill.GetAll();
            AdminMissionDetailsVM obj = new()
            {
                UserInfo = user,
                CountryList = Countries,
                ThemeList = themeList,
                SkillList = skillList,
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult DeleteCMSData(int CMSId)
        {
            if (CMSId > 0)
            {
                CmsPage deletingData = _unitOfWork.CMSPage.GetFirstOrDefault(m => m.CmsPageId == CMSId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.CMSPage.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminCMSPage");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminCMSPage");
        }

        [HttpPost]
        public IActionResult AddAndUpdateCMSPage(AdminCMSPageDetailsVM obj)
        {
            if (obj.CMSId == 0)
            {
                CmsPage data = new()
                {
                    Title = obj.CMSTitle,
                    Description = obj.CMSDescription,
                    Slug = obj.CMSSlug,
                    Status = "1"
                };

                _unitOfWork.CMSPage.Add(data);
                _unitOfWork.Save();

                TempData["success"] = "Data added successfully!";
                return RedirectToAction("AdminCMSPage");
            }
            else
            {
                CmsPage updatedData = _unitOfWork.CMSPage.GetFirstOrDefault(m => m.CmsPageId == obj.CMSId);
                if (updatedData != null)
                {
                    updatedData.CmsPageId = obj.CMSId;
                    updatedData.Title = obj.CMSTitle;
                    updatedData.Description = obj.CMSDescription;
                    updatedData.Slug = obj.CMSSlug;
                    updatedData.Status = obj.Status;

                    _unitOfWork.CMSPage.Update(updatedData);
                    _unitOfWork.Save();

                    TempData["success"] = "Data updated successfully!";
                    return RedirectToAction("AdminCMSPage");
                }
            }

            TempData["error"] = "Something went wrong!";
            return RedirectToAction("AdminCMSPage");
        }


        //Mission lists
        public IActionResult AdminMissionDetails()
        {
            User user = GetThisUser();
            IEnumerable<Mission> missionLists = _unitOfWork.Mission.GetAccToFilter(m => m.DeletedAt == null);
            AdminMissionDetailsVM obj = new()
            {
                MissionLists = missionLists,
                UserInfo = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult DeleteMissionData(int missionId)
        {
            if (missionId > 0)
            {
                Mission deletingData = _unitOfWork.Mission.GetFirstOrDefault(m => m.MissionId == missionId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.Mission.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminMissionDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminMissionDetails");
        }

        //MissionTheme lists
        public IActionResult AdminMissionThemeDetails()
        {
            User user = GetThisUser();
            IEnumerable<MissionTheme> missionThemeLists = _unitOfWork.MissionTheme.GetAccToFilter(m => m.DeletedAt == null);
            AdminMissionThemeDetailsVM obj = new()
            {
                MissionThemeLists = missionThemeLists,
                UserInfo = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult AddAndUpdateMissionTheme(AdminMissionThemeDetailsVM obj)
        {
            var Edata1 = _unitOfWork.MissionTheme.GetAccToFilter(m => m.Title == obj.MissionThemeTitle && m.DeletedAt == null);
            var Edata2 = _unitOfWork.MissionTheme.GetFirstOrDefault(m => m.Title == obj.MissionThemeTitle && m.DeletedAt == null && m.MissionThemeId != obj.MissionThemeId);

            if (obj.MissionThemeId == 0)
            {
                if (Edata1 == null)
                {
                    MissionTheme data = new()
                    {
                        Title = obj.MissionThemeTitle,
                        Status = 1
                    };

                    _unitOfWork.MissionTheme.Add(data);
                    _unitOfWork.Save();

                    TempData["success"] = "Data added successfully!";
                    return RedirectToAction("AdminMissionThemeDetails");
                }
            }
            else
            {
                if (Edata2 == null)
                {
                    MissionTheme updatedData = _unitOfWork.MissionTheme.GetFirstOrDefault(m => m.MissionThemeId == obj.MissionThemeId);
                    if (updatedData != null)
                    {
                        updatedData.MissionThemeId = obj.MissionThemeId;
                        updatedData.Title = obj.MissionThemeTitle;
                        updatedData.Status = (byte)obj.Status;

                        _unitOfWork.MissionTheme.Update(updatedData);
                        _unitOfWork.Save();

                        TempData["success"] = "Data updated successfully!";
                        return RedirectToAction("AdminMissionThemeDetails");
                    }
                }
            }

            TempData["error"] = "Something went wrong!";
            return RedirectToAction("AdminMissionThemeDetails");
        }

        [HttpPost]
        public IActionResult GetMissionThemeData(int missionThemeId)
        {
            MissionTheme missionThemeData = _unitOfWork.MissionTheme.GetFirstOrDefault(m => m.MissionThemeId == missionThemeId);
            return Json(missionThemeData);
        }

        [HttpPost]
        public IActionResult DeleteMissionThemeData(int missionThemeId)
        {
            if (missionThemeId > 0)
            {
                MissionTheme deletingData = _unitOfWork.MissionTheme.GetFirstOrDefault(m => m.MissionThemeId == missionThemeId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.MissionTheme.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminMissionThemeDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminMissionThemeDetails");
        }


        //Skill Lists
        public IActionResult AdminMissionSkillDetails()
        {
            User user = GetThisUser();
            IEnumerable<Skill> SkillLists = _unitOfWork.Skill.GetAccToFilter(m => m.DeletedAt == null);
            AdminMissionSkillDetailsVM obj = new()
            {
                SkillLists = SkillLists,
                UserInfo = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult AddAndUpdateSkill(AdminMissionSkillDetailsVM obj)
        {
            var Edata1 = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillName == obj.SkillName && m.DeletedAt == null);
            var Edata2 = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillName == obj.SkillName && m.DeletedAt == null && m.SkillId != obj.SkillIds);

            if (obj.SkillIds == 0)
            {
                if (Edata1 == null)
                {
                    Skill data = new()
                    {
                        SkillName = obj.SkillName,
                        Status = 1
                    };

                    _unitOfWork.Skill.Add(data);
                    _unitOfWork.Save();

                    TempData["success"] = "Data added successfully!";
                    return RedirectToAction("AdminMissionSkillDetails");
                }
            }
            else
            {
                if (Edata2 == null)
                {
                    Skill updatedData = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillId == obj.SkillIds);
                    if (updatedData != null)
                    {
                        updatedData.SkillId = obj.SkillIds;
                        updatedData.SkillName = obj.SkillName;
                        updatedData.Status = (byte)obj.Status;

                        _unitOfWork.Skill.Update(updatedData);
                        _unitOfWork.Save();

                        TempData["success"] = "Data updated successfully!";
                        return RedirectToAction("AdminMissionSkillDetails");
                    }
                }
            }

            TempData["error"] = "Something went wrong!";
            return RedirectToAction("AdminMissionSkillDetails");
        }

        [HttpPost]
        public IActionResult GetSkillData(int skillId)
        {
            Skill skillData = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillId == skillId);
            return Json(skillData);
        }

        [HttpPost]
        public IActionResult DeleteSkillData(int skillId)
        {
            if (skillId > 0)
            {
                Skill deletingData = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillId == skillId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.Skill.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminMissionSkillDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminMissionSkillDetails");
        }


        //MissionApplication Lists
        public IActionResult AdminMissionApplicationDetails()
        {
            IEnumerable<MissionApplication> missionAppLists = _unitOfWork.MissionApplication.GetAllMissionApplicationList();
            IEnumerable<MissionApplication> missionAppListsByFilter = missionAppLists.Where(m => m.ApprovalStatus == "PENDING").ToList();
            User user = GetThisUser();
            AdminMissionApplicationDetailsVM obj = new()
            {
                MissionApplicationLists = missionAppListsByFilter,
                UserInfo = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult ApproveAndDeclineMissionApplication(int missionApplicationId, int flag)
        {
            if (missionApplicationId > 0)
            {
                MissionApplication missionData = _unitOfWork.MissionApplication.GetFirstOrDefault(m => m.MissionApplicationId == missionApplicationId);

                if (missionData != null)
                {
                    if (flag == 1)
                    {
                        missionData.ApprovalStatus = "APPROVE";
                        TempData["success"] = "Data Approved successfully!";
                    }
                    else
                    {
                        missionData.ApprovalStatus = "DECLINE";
                        TempData["success"] = "Data Declined successfully!";
                    }
                    _unitOfWork.MissionApplication.Update(missionData);
                    _unitOfWork.Save();
                    return RedirectToAction("AdminMissionSkillDetails");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminMissionApplicationDetails");
        }

        //Story lists
        public IActionResult AdminStoryDetails()
        {
            IEnumerable<Story> storyLists = _unitOfWork.Story.GetAllStory();
            IEnumerable<Story> storyListsByFilter = storyLists.Where(m => m.Status == "PENDING").ToList();
            User user = GetThisUser();
            AdminStoryDetailsVM obj = new()
            {
                StoryLists = storyListsByFilter,
                UserInfo = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult ApproveAndDeclineStory(int storyId, int flag)
        {
            if (storyId > 0)
            {
                Story storyData = _unitOfWork.Story.GetFirstOrDefault(m => m.StoryId == storyId);

                if (storyData != null)
                {
                    if (flag == 1)
                    {
                        storyData.Status = "PUBLISHED";
                        TempData["success"] = "Story published successfully!";
                    }
                    else
                    {
                        storyData.Status = "DECLINED";
                        TempData["success"] = "Data Declined successfully!";
                    }
                    _unitOfWork.Story.Update(storyData);
                    _unitOfWork.Save();
                    return RedirectToAction("AdminStoryDetails");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminStoryDetails");
        }

        [HttpPost]
        public IActionResult GetStoryDetails(int storyId)
        {
            User? FindingStoryCreator = _repo.UserOfStory(storyId);

            AdminStoryDetailsVM storyDetails = new()
            {
                UserOfStory = FindingStoryCreator,
            };

            return Json(storyDetails);
        }

        //Banner Lists
        public IActionResult AdminBannerDetails()
        {
            User user = GetThisUser();
            IEnumerable<Banner> bannerLists = _unitOfWork.Banner.GetAccToFilter(m => m.DeletedAt == null);
            AdminBannerDetailsVM obj = new()
            {
                BannerLists = bannerLists,
                UserInfo = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult AddAndUpdateBanner(AdminBannerDetailsVM obj, IFormFile banner)
        {
            var Bdata = _unitOfWork.Banner.GetFirstOrDefault(m => m.DeletedAt == null && m.BannerId != obj.BannerId);

            if (obj.BannerId == 0)
            {
                Banner data = new()
                {
                    Text = obj.BannerText,
                    SortOrder = (int)obj.BannerNumber
                };

                if (banner != null)
                {
                    string imgExt = Path.GetExtension(banner.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        string ImageName = Path.GetFileName(banner.FileName);
                        var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Banners/", ImageName);
                        string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                        using (FileStream stream = new(finalPath, FileMode.Create))
                        {
                            banner.CopyTo(stream);
                        }
                        data.Image = imgSaveTo;
                    }
                }

                _unitOfWork.Banner.Add(data);
                _unitOfWork.Save();

                TempData["success"] = "Data added successfully!";
                return RedirectToAction("AdminBannerDetails");

            }
            else
            {
                if (Bdata == null)
                {
                    Banner updatedData = _unitOfWork.Banner.GetFirstOrDefault(m => m.BannerId == obj.BannerId);
                    if (updatedData != null)
                    {
                        updatedData.BannerId = obj.BannerId;
                        updatedData.Text = obj.BannerText;
                        updatedData.SortOrder = (int)obj.BannerNumber;

                        if (updatedData != null && banner != null)
                        {
                            string imgExt = Path.GetExtension(banner.FileName);
                            if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                            {
                                if (updatedData.Image != null)
                                {
                                    string ImageName = Path.GetFileName(banner.FileName);
                                    var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Banners/", ImageName);
                                    string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                                    if (!imgSaveTo.Equals(updatedData.Image))
                                    {
                                        string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + updatedData.Image);
                                        if (System.IO.File.Exists(alrExists))
                                        {
                                            System.IO.File.Delete(alrExists);
                                        }

                                        using (FileStream stream = new(finalPath, FileMode.Create))
                                        {
                                            banner.CopyTo(stream);
                                        }
                                        updatedData.Image = imgSaveTo;
                                        updatedData.UpdatedAt = DateTime.Now;
                                    }
                                }
                                else
                                {
                                    string ImageName = Path.GetFileName(banner.FileName);
                                    var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Banners/", ImageName);
                                    string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                                    using (FileStream stream = new(finalPath, FileMode.Create))
                                    {
                                        banner.CopyTo(stream);
                                    }
                                    updatedData.Image = imgSaveTo;
                                    updatedData.UpdatedAt = DateTime.Now;
                                }
                            }
                        }

                        _unitOfWork.Banner.Update(updatedData);
                        _unitOfWork.Save();

                        TempData["success"] = "Data updated successfully!";
                        return RedirectToAction("AdminBannerDetails");
                    }
                }
            }
            TempData["success"] = "Data added successfully!";
            return RedirectToAction("AdminBannerDetails");
        }

        [HttpPost]
        public IActionResult GetBannerData(int bannerId)
        {
            Banner bannerData = _unitOfWork.Banner.GetFirstOrDefault(m => m.BannerId == bannerId);
            return Json(bannerData);
        }


        [HttpPost]
        public IActionResult DeleteBannerData(int bannerId)
        {
            if (bannerId > 0)
            {
                Banner deletingData = _unitOfWork.Banner.GetFirstOrDefault(m => m.BannerId == bannerId);
                deletingData.DeletedAt = DateTime.Now;
                string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + deletingData.Image);
                System.IO.File.Delete(alrExists);
                _unitOfWork.Banner.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminBannerDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminBannerDetails");
        }
    }
}
