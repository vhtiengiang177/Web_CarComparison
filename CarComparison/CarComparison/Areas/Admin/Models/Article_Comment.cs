using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarComparison.Areas.Admin.Models
{
    public class Article_Comment
    {
        public List<Article> Article { get; set; }
        public List<Comment> Comments { get; set; }

    }
}