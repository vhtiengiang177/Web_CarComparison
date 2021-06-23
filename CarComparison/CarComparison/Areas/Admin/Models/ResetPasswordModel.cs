using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarComparison.Areas.Admin.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Bắt buộc nhập mật khẩu mới", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "{0} cần nhập từ {2} đến {1} ký tự.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không trùng khớp")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }
    }
}