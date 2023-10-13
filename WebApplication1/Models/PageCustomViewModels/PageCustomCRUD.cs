using WebApplication1.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebApplication1.Models.PageCustomViewModels
{
    public class PageCustomCRUD
    {
        public int ID { get; set; }
        public string? Custom { get; set; }
        public string? SlugCustom { get; set; }
        public string? NameCustom { get; set; }
        public DateTime? CreateDate { get; set; }

        public static implicit operator PageCustomCRUD(PageCustom _Cate)
        {
            return new PageCustomCRUD
            {
                ID = _Cate.ID,
                Custom = _Cate.Custom,
                SlugCustom = _Cate.SlugCustom,
                NameCustom = _Cate.NameCustom,
                CreateDate = _Cate.CreateDate,
                
            };
        }

        public static implicit operator PageCustom(PageCustomCRUD vm)
        {
            return new PageCustom
            {
                ID = vm.ID,
                Custom = vm.Custom,
                SlugCustom = vm.SlugCustom,
                NameCustom = vm.NameCustom,
                CreateDate = vm.CreateDate,
            };
        }
    }
}
