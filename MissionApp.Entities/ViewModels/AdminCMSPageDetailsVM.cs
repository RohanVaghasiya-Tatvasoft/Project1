using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class AdminCMSPageDetailsVM
    {
        public IEnumerable<CmsPage> CMSLists { get; set; } = new List<CmsPage>();

        public User UserInfo { get; set; }

        public string? CMSTitle { get; set; }

        public string? CMSDescription { get; set; }

        public string? CMSSlug { get; set; }

        public int CMSId { get; set; }

        public string? Status { get; set; }
    }
}
