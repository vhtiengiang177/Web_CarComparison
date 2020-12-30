using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace CarComparison.Areas.Admin.Models
{
    public class AuthorizeController : ActionFilterAttribute
    {
        CompareCarEntities db = new CompareCarEntities();
        //phương thức thực thi khi action được gọi
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            User_ tbus = HttpContext.Current.Session["user"] as User_;
            //nếu session=null thì trả về trang đăng nhập
            if (tbus == null)
            {
                filterContext.Result = new RedirectResult("~/Account/LoginView");
            }
            //session != null
            else
            {
                //lấy danh sách quyền của user
                var us_idper = (from id_per in db.User_Permission where id_per.id_typeuser == tbus.id_typeuser select id_per).ToList();
                if(us_idper.Count != 0)
                {
                    //string idper = us_idper[0].ToString();
                    //var us_per = db.Permissions.Where(n => n.id_permission == idper).ToList();
                    //(from per in db.Permissions where per.id_permission == idper select per).ToList();
                    //db.Permissions.Where(n => n.id_permission == us_idper[0]).ToList();
                    //đếm số lượng quyền
                    int amount_per = us_idper.Count;
                    //khởi tạo mảng
                    string[] listpermission = new string[amount_per];
                    int i = 0;
                    //lấy danh sách quyền đưa vào mảng
                    foreach (var item in us_idper)
                    {
                        listpermission[i] = item.Permission.name_permission;
                        i++;
                    }
                    //Lấy tên controller và action
                    //string actionName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "-" + filterContext.ActionDescriptor.ActionName;

                    //Lấy tên Controller
                    string ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    //nếu tên controler không có trong mảng quyền của user thì trả về trang chủ
                    if (!listpermission.Contains(ControllerName))
                    {
                        filterContext.Result = new RedirectResult("~/Client/Index");
                    }
                    else
                    {
                        base.OnActionExecuting(filterContext);
                    }
                }
                // Nếu không có quyền gì hết
                else
                {
                    filterContext.Result = new RedirectResult("~/Client/Index");
                }
                
            }
        }
    }
}