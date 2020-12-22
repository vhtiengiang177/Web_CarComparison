using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarComparison
{
    public class Global
    {
        public static User_ GlobalUser { get; private set; }

        public static void SetGlobalUser(User_ user)
        {
            GlobalUser = user;
        }
    }
}