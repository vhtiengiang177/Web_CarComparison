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
            
        }

        //public List<SelectListItem> Automaker { get; set; }
        //public List<SelectListItem> Model { get; set; }
        //public List<SelectListItem> Version { get; set; }



       




        public string automakerId { get; set; }
        public string modelId { get; set; }
        public string versionId { get; set; }
        public string CarId { get; set; }

        public string automakerId1 { get; set; }
        public string modelId1 { get; set; }
        public string versionId1 { get; set; }
        public string CarId1 { get; set; }

        public string automakerId2 { get; set; }
        public string modelId2 { get; set; }
        public string versionId2 { get; set; }
        public string CarId2 { get; set; }

        

    }
}