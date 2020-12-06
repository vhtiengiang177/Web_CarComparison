using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarComparison
{
    public class Global
    {
        public static InfoAccount GlobalUser { get; private set; }

        public static void SetGlobalUser(InfoAccount user)
        {
            GlobalUser = user;
        }
    }
}