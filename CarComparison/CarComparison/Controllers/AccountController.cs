using CarComparison.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Controllers
{
    // Chức năng: Sửa thông tin tài khoản sau khi đăng nhập, cả admin và member đều dùng chung
    public class AccountController : Controller
    {
        private CompareCarEntities db = new CompareCarEntities();

        //// GET: Account
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // Hash mật khẩu
        public string GetMD5(string str)
        {
            str = str + "md5";
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {

                sbHash.Append(String.Format("{0:x2}", b));

            }
            return sbHash.ToString();
        }

        // View đăng nhập
        [AllowAnonymous]
        public ActionResult LoginView()
        {
            User_ us = Session["user"] as User_;
            if (us != null)
            {
                return RedirectToAction("Index", "Client");
            }
            return View();
        }

        [HttpPost]
        //xử lý đăng nhập
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection f)
        {
            string userName = f["username"].ToString();
            string passWord = f["password"].ToString();
            string passWordmd5 = GetMD5(passWord);
            User_ us = (from c in db.User_ where c.name_user == userName && c.password_user == passWordmd5 select c).SingleOrDefault();
            //nếu user nhập đúng mật khẩu
            if (us != null)
            {
                if (us.block_state_user == "0" && us.id_typeuser != "TyUs01") // Không phải admin và trạng thái ngừng hoạt động
                {
                    ModelState.AddModelError("LoginError", "Tài khoản đang bị khóa");
                }
                else
                {
                    Session["user"] = us;
                    try
                    { //lấy thời gian đăng nhập lưu vào hệ thống
                        us.lastvisitdate_user = DateTime.Now;
                        db.SaveChanges();
                    }
                    catch { };

                    if (us.id_typeuser == "TyUs01")
                    {
                        return RedirectToAction("Index", "Admin/Home");
                    }
                    if (us.id_typeuser == "TyUs02")
                    {
                        return RedirectToAction("Index", "Client");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("LoginError", "Tài khoản hoặc mật khẩu không đúng, vui lòng nhập lại!");
            }
            return View("LoginView");
        }

        // View đăng ký
        [AllowAnonymous]
        public ActionResult RegisterView()
        {
            User_ us = Session["user"] as User_;
            if (us != null)
            {
                return RedirectToAction("Index", "Client");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection f)
        {
            try
            {
                string userName = f["username"].ToString();
                string passWord = f["password_user"].ToString();
                var checkUserName = (from c in db.User_ where c.name_user == userName select c).ToList();
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasMinimum8Chars = new Regex(@".{8,}");
                var hasCharSpecial = new Regex(@"[#?!@$%^&*-]+");

                var isValidated = hasNumber.IsMatch(passWord) && hasUpperChar.IsMatch(passWord) && hasMinimum8Chars.IsMatch(passWord) && hasCharSpecial.IsMatch(passWord);
                if (checkUserName.Count() != 0 || !isValidated)
                {
                    if (checkUserName.Count() != 0)
                    {
                        ModelState.AddModelError("RegisterError", "Tên tài khoản đã tồn tại!");
                    }
                    if (!isValidated)
                    {
                        ModelState.AddModelError("PassError", "Mật khẩu phải chứa ít nhất một số, một chữ hoa, một ký tự đặc biệt, dài hơn 8 ký tự!");
                    }
                }
                else
                {
                    string passWordmd5 = GetMD5(passWord);
                    string lastName = f["lname"].ToString();
                    string firstName = f["fname"].ToString();
                    string email = f["email"].ToString();
                    string sex = f["sex"];
                    DateTime birthday = Convert.ToDateTime(f["birthday"]);
                    User_ us_new = new User_();
                    // Tạo id user tự động
                    var createID = (from c in db.User_ select c.id_user).ToList();
                    string id = "";
                    if (createID.Count == 0) // nếu danh sách rỗng
                    {
                        id = "Us01";
                    }
                    else
                    {
                        for (int i = 0; i < createID.Count(); i++)
                        {
                            if (int.Parse(createID[i].Substring(2, 2)) != (i + 1))
                            {
                                if (i + 1 >= 0 && i + 1 < 9)
                                    id = "Us0" + (i + 1).ToString();
                                else if (i + 1 > 9)
                                    id = "Us" + (i + 1).ToString();
                                break;
                            }
                        }
                        if (id == "")
                        {
                            id = createID[createID.Count - 1].Substring(2, 2);
                            if (int.Parse(id) >= 0 && int.Parse(id) < 9)
                            {
                                id = "Us0" + (int.Parse(id) + 1).ToString();
                            }
                            else if (int.Parse(id) >= 9)
                            {
                                id = "Us" + (int.Parse(id) + 1).ToString();
                            }
                        }
                    }
                    us_new.id_user = id;
                    us_new.id_typeuser = "TyUs02"; // Member
                    us_new.lname_user = lastName;
                    us_new.fname_user = firstName;
                    us_new.email_user = email;
                    if (sex == "1")
                    {
                        us_new.sex_user = "Nam";
                    }
                    else if (sex == "2")
                    {
                        us_new.sex_user = "Nữ";
                    }
                    else
                    {
                        us_new.sex_user = "Khác";
                    }
                    us_new.birthday_user = birthday;
                    us_new.block_state_user = "1";
                    us_new.name_user = userName;
                    us_new.password_user = passWordmd5;
                    us_new.registerdate_user = DateTime.Now;
                    us_new.lastvisitdate_user = DateTime.Now;
                    us_new.address_user = us_new.phone_user = "";
                    us_new.avt_user = "/Asset/Image/User/Default.jpg";
                    db.User_.Add(us_new);
                    db.SaveChanges();
                    Session["user"] = us_new;
                    return RedirectToAction("Index", "Client");
                }
                return View("RegisterView");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Lỗi: Không thể tạo được tài khoản, vui lòng đăng ký lại!" + ex.Message + "');</script>");
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Remove("user");
            Session.Clear();
            return RedirectToAction("Index", "Client");
        }

        [AuthorizeController]
        public ActionResult AccountInformation(string tab = "")
        {
            User_ us = Session["user"] as User_;
            if (us == null)
            {
                return RedirectToAction("Index", "Client");
            }
            ViewBag.tabactive = tab;
            if (us != null)
            {
                User_ user = db.User_.Find(us.id_user);
                return View(user);
            }
            return View(us);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInfo(FormCollection f, [Bind(Include = "imageFile" )] User_ user)
        {
            try
            {
                string action = f["action"];
                //string formID = f["id_user"];
                User_ us = db.User_.Find((Session["user"] as User_).id_user);
                if (us == null)
                {
                    return HttpNotFound();
                }
                if (action == "Lưu")
                {
                    string lname = f["lname_user"];
                    string fname = f["fname_user"];
                    string email = f["email_user"];
                    string phone = f["phone_user"];
                    string sex = f["sex"];
                    try
                    {
                        DateTime birthday = Convert.ToDateTime(f["birthday_us"]);
                        us.birthday_user = birthday;
                    }
                    catch 
                    {
                        ModelState.AddModelError("BirthdayError", "Không đúng định dạng");
                    }
                    
                    string address = f["address_user"];
                    us.lname_user = lname;
                    us.fname_user = fname;
                    us.email_user = email;
                    us.phone_user = phone;
                    if (sex == "1")
                    {
                        us.sex_user = "Nam";
                    }
                    else if (sex == "2")
                    {
                        us.sex_user = "Nữ";
                    }
                    else
                    {
                        us.sex_user = "Khác";
                    }
                    
                    us.address_user = address;
                    us.imageFile = user.imageFile;
                    if (us.imageFile != null)
                    {
                        if (us.avt_user != null && us.avt_user != "" && us.avt_user != "/Asset/Image/User/Default.jpg")
                        {
                            string fullpath = Server.MapPath(us.avt_user);
                            FileInfo fi = new FileInfo(fullpath);
                            if (fi != null)
                            {
                                System.IO.File.Delete(fullpath);
                                fi.Delete();
                            }
                        }
                        string fileName = Path.GetFileNameWithoutExtension(us.imageFile.FileName);
                        string extension = Path.GetExtension(us.imageFile.FileName);
                        us.avt_user = "/Asset/Image/User/" + us.id_user + extension;
                        fileName = Path.Combine(Server.MapPath("/Asset/Image/User/"), us.id_user + extension);
                        us.imageFile.SaveAs(fileName);
                    }

                    db.Entry(us).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AccountInformation", "Account", new { tab = "edit" });
                }
                else if (action == "Hủy")
                {
                    db.Entry(us).State = EntityState.Unchanged;
                    db.SaveChanges();
                    return RedirectToAction("AccountInformation", "Account", new { tab = "editfalse" });
                }
                return RedirectToAction("AccountInformation", "Account", new { tab = "editfalse" });
            }
            catch(DbEntityValidationException e)
            {
                 foreach (var eve in e.EntityValidationErrors)
                 {
                     Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                         eve.Entry.Entity.GetType().Name, eve.Entry.State);
                     foreach (var ve in eve.ValidationErrors)
                     {
                         Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                     }
                 }
                 throw;
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(FormCollection f)
        {
            //string formID = f["id_user"];
            User_ us = db.User_.Find((Session["user"] as User_).id_user);
            if (us == null)
            {
                return HttpNotFound();
            }
            string passWordNew = f["password_new"];
            string olmd5 = GetMD5(f["password_old"]);
            string nemd5 = GetMD5(passWordNew);
            string re_nemd5 = GetMD5(f["repassword_new"]);
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");
            var hasCharSpecial = new Regex(@"[#?!@$%^&*-]+");

            var isValidated = hasNumber.IsMatch(passWordNew) && hasUpperChar.IsMatch(passWordNew) && hasMinimum8Chars.IsMatch(passWordNew) && hasCharSpecial.IsMatch(passWordNew);
            if (nemd5 != re_nemd5 || olmd5 != us.password_user || !isValidated)
            {
                if (!isValidated)
                {
                    ModelState.AddModelError("errorChange", "Mật khẩu phải chứa ít nhất một số, một chữ hoa, một ký tự đặc biệt, dài hơn 8 ký tự!");
                }
                else if (nemd5 != re_nemd5)
                {
                    ModelState.AddModelError("errorChange", "Mật khẩu mới và nhập lại mật khẩu không trùng nhau!");
                }
                if (olmd5 != us.password_user)
                {
                    ModelState.AddModelError("errorChange2", "Mật khẩu cũ không đúng!");
                }
                return RedirectToAction("AccountInformation", "Account", new { us.id_user, tab = "changepassf" });
            }
            us.password_user = nemd5;
            db.Entry(us).State = EntityState.Modified;
            db.SaveChanges();
            Response.Write("<script>alert('Đổi thành công!');</script>");
            return RedirectToAction("AccountInformation", "Account", new { us.id_user, tab = "changepasst" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DisableAccount(FormCollection f)
        {
            string action = f["action"];
            //string formID = f["id_user"];
            User_ us = db.User_.Find((Session["user"] as User_).id_user);
            if (us == null)
            {
                return HttpNotFound();
            }
            if (action == "Vô hiệu hóa tài khoản")
            {
                us.block_state_user = "0";
                db.Entry(us).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Logout"); // Thoát khỏi trang
            }
            return RedirectToAction("AccountInformation", "Account", new { us.id_user });
        }

        public ActionResult ForgotPassword()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string EmailID)
        {
            string resetCode = Guid.NewGuid().ToString();
            var verifyUrl = "/Account/ResetPassword/" + resetCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var getUser = (from s in db.User_ where s.email_user == EmailID select s).FirstOrDefault();
            if (getUser != null)
            {
                getUser.resetPassCode = resetCode;

                //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 

                db.Entry(getUser).State = EntityState.Modified;
                db.SaveChanges();

                var subject = "[CAR COMPARISON] Yêu cầu đặt lại mật khẩu";
                var body = "Xin chào " + getUser.fname_user + ", <br/> Gần đây bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình. Nhấp vào liên kết dưới đây để đặt lại mật khẩu." +

                     " <br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                     "Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này hoặc trả lời cho chúng tôi biết. Xin cảm ơn. <br/><br/>Trân trọng.<br/> Đội ngũ Car Comparison.";

                SendEmail(getUser.email_user, body, subject);

                ViewBag.Message = "Liên kết đặt lại mật khẩu đã được gửi đến id email của bạn.";
            }
            else
            {
                ViewBag.Message = "Tài khoản không tồn tại.";
                return View();
            }
            return View();
        }

        private void SendEmail(string emailAddress, string body, string subject)
        {
            using (MailMessage mm = new MailMessage("vhtiengiang177@gmail.com", emailAddress))
            {
                mm.Subject = subject;
                mm.Body = body;

                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                NetworkCredential NetworkCred = new NetworkCredential("vhtiengiang177@gmail.com", "ji@n554!");
 
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);

            }
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            var user = (from c in db.User_ where c.resetPassCode == id select c).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = db.User_.Where(a => a.resetPassCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    //you can encrypt password here, we are not doing it
                    user.password_user = GetMD5(model.NewPassword);
                    //make resetpasswordcode empty string now
                    user.resetPassCode = "";
                    //to avoid validation issues, disable it
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "Đã cập nhật mật khẩu mới thành công.";
                    Session["user"] = user;
                    return RedirectToAction("Index", "Client");
                }
            }
            else
            {
                message = "Không hợp lệ.";
            }
            ViewBag.Message = message;
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}