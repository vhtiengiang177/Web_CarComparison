using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Areas.Admin.Models
{
    public class cascadingmodel
    {

        public cascadingmodel()
        {
            this.Automaker = new List<SelectListItem>();
            this.Model = new List<SelectListItem>();
            this.Version = new List<SelectListItem>();
        }

        public List<SelectListItem> Automaker { get; set; }
        public List<SelectListItem> Model { get; set; }
        public List<SelectListItem> Version { get; set; }

        public string automakerId { get; set; }
        public string modelId { get; set; }
        public string versionId { get; set; }

    }
}