using WebApplication1.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebApplication1.Models.BankViewModels
{
    public class BankCRUD
    {
        public int ID { get; set; }
        public string? LoaiBank { get; set; }
        public string? BankName { get; set; }
        public string? BankAcc { get; set; }
        public string? BankUser { get; set; }
        public string? ImgBank { get; set; }
        public string? QrBank { get; set; }
        public IFormFile? ImgPr { get; set; }
        public IFormFile? ImgPrLogo { get; set; }
        public static implicit operator BankCRUD(BankConfig _Cate)
        {
            return new BankCRUD
            {
                ID = _Cate.ID,
                LoaiBank = _Cate.LoaiBank,
                BankName = _Cate.BankName,
                BankAcc = _Cate.BankAcc,
                BankUser = _Cate.BankUser,
                ImgBank = _Cate.ImgBank,
                QrBank = _Cate.QrBank,

            };
        }

        public static implicit operator BankConfig(BankCRUD vm)
        {
            return new BankConfig
            {
                ID = vm.ID,
                LoaiBank = vm.LoaiBank,
                BankName = vm.BankName,
                BankAcc = vm.BankAcc,
                BankUser = vm.BankUser,
                ImgBank = vm.ImgBank,
                QrBank = vm.QrBank,
            };
        }
    }
}
