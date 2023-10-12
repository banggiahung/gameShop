using WebApplication1.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebApplication1.Models.HistoryViewModels
{
    public class HistoryCRUD
    {
        public int ID { get; set; }
        public int? PriceUser { get; set; }
        public string? UserId { get; set; }
        public int? BankId { get; set; }
        public string? ContentTransit { get; set; }
        public bool? isDone { get; set; }
        public DateTime? CreateDate { get; set; }


        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? NameAccount { get; set; }
        public string? LoaiBank { get; set; }
        public string? UserName { get; set; }


        public static implicit operator HistoryCRUD(History _Cate)
        {
            return new HistoryCRUD
            {
                ID = _Cate.ID,
                PriceUser = _Cate.PriceUser,
                UserId = _Cate.UserId,
                BankId = _Cate.BankId,
                ContentTransit = _Cate.ContentTransit,
                CreateDate = _Cate.CreateDate,
                isDone = _Cate.isDone,
              
            };
        }

        public static implicit operator History(HistoryCRUD vm)
        {
            return new History
            {
                ID = vm.ID,
                PriceUser = vm.PriceUser,
                UserId = vm.UserId,
                BankId = vm.BankId,
                ContentTransit = vm.ContentTransit,
                CreateDate = vm.CreateDate,
                isDone = vm.isDone,
            };
        }
    }
}
