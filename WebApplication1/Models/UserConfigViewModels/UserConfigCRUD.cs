using WebApplication1.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebApplication1.Models.UserConfigViewModels
{
    public class UserConfigCRUD
    {
        public int ID { get; set; }
        public string? UserID { get; set; }
        [Display(Name = "Tài khoản")]
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        public string UserName { get; set; }

        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public string? ConfirmPassword { get; set; }
        public int? Total { get; set; }
        public bool IsAcitive { get; set; }

        public DateTime? CreateDate { get; set; }
        public static implicit operator UserConfigCRUD(UserConfig _Cate)
        {
            return new UserConfigCRUD
            {
                ID = _Cate.ID,
                UserID = _Cate.UserID,
                UserName = _Cate.UserName,
                Password = _Cate.Password,
                Total = _Cate.Total,
                CreateDate = _Cate.CreateDate,
                IsAcitive = _Cate.IsAcitive,

            };
        }

        public static implicit operator UserConfig(UserConfigCRUD vm)
        {
            return new UserConfig
            {
                ID = vm.ID,
                UserID = vm.UserID,
                UserName = vm.UserName,
                Password = vm.Password,
                Total = vm.Total,
                CreateDate = vm.CreateDate,
                IsAcitive = vm.IsAcitive,
            };
        }
    }
}
