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
    }
}
