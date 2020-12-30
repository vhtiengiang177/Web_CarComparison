using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarComparison.Areas.Admin.Models
{
    public class Article_Comment
    {
        public Article ArticleModel { get; set; }
        public Comment CommentModel { get; set; }

    }
}