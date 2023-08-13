using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var backgroundSettings = _context.MainView.FirstOrDefault();
            ViewData["BackgroundImageUrl"] = backgroundSettings?.ImgMain;
            return View();
        }
        [HttpGet]
        public IActionResult GetProductLast()
        {
            var lastProduct = (from _pr in _context.Product
                               join _cate in _context.Category on _pr.CateId equals _cate.id
                               select new
                               {
                                   ID = _pr.Id,
                                   Name = _pr.Name,
                                   MainImg = _pr.MainImg,
                                   LinkDown = _pr.LinkDown,
                                   DetailsGame = _pr.DetailsGame,
                                   DesShort = _pr.DesShort,
                                   CreateDate = _pr.CreateDate,
                                   CateId = _pr.CateId,
                                   CategoryName = _cate.NameCate,
                               })
                              .OrderByDescending(_pr => _pr.ID) 
                              .FirstOrDefault(); 

            if (lastProduct != null)
            {
                return Ok(lastProduct);
            }
            else
            {
                return NotFound(); 
            }
        }
        [HttpGet]
        public IActionResult GetTop5LatestProducts()
        {
            var latestProducts = (from _pr in _context.Product
                                  join _cate in _context.Category on _pr.CateId equals _cate.id
                                  select new
                                  {
                                      ID = _pr.Id,
                                      Name = _pr.Name,
                                      MainImg = _pr.MainImg,
                                      LinkDown = _pr.LinkDown,
                                      DetailsGame = _pr.DetailsGame,
                                      DesShort = _pr.DesShort,
                                      CreateDate = _pr.CreateDate,
                                      CateId = _pr.CateId,
                                      CategoryName = _cate.NameCate,
                                  })
                                 .OrderByDescending(_pr => _pr.ID) // Sắp xếp theo ngày tạo giảm dần
                                 .Take(5) // Giới hạn số lượng bản ghi
                                 .ToList(); // Chuyển kết quả thành danh sách

            return Ok(latestProducts);
        }
        [HttpGet]
        public IActionResult GetCategoryFirst()
        {
            var latestProducts = (from _pr in _context.Product
                                  join _cate in _context.Category on _pr.CateId equals _cate.id
                                  where _cate.id == 1
                                  select new
                                  {
                                      ID = _pr.Id,
                                      Name = _pr.Name,
                                      MainImg = _pr.MainImg,
                                      LinkDown = _pr.LinkDown,
                                      DetailsGame = _pr.DetailsGame,
                                      DesShort = _pr.DesShort,
                                      CreateDate = _pr.CreateDate,
                                      CateId = _pr.CateId,
                                      CategoryName = _cate.NameCate,
                                  })
                                 .OrderByDescending(_pr => _pr.ID) // Sắp xếp theo ngày tạo giảm dần
                                 
                                 .ToList(); // Chuyển kết quả thành danh sách

            return Ok(latestProducts);
        }
        [HttpGet]
        public IActionResult GetAllProduct()
        {

            var pr = from _pr in _context.Product
                     join _cate in _context.Category on _pr.CateId equals _cate.id
                     select new
                     {
                         ID = _pr.Id,
                         Name = _pr.Name,
                         MainImg = _pr.MainImg,
                         LinkDown = _pr.LinkDown,
                         LinkDownDrop = _pr.LinkDownDrop,
                         LinkDownMedia = _pr.LinkDownMedia,
                         DetailsGame = _pr.DetailsGame,
                         DesShort = _pr.DesShort,
                         CreateDate = _pr.CreateDate,
                         CateId = _pr.CateId,
                         CategoryName = _cate.NameCate,
                         RAM = _pr.RAM,
                         GB = _pr.GB,
                         Language = _pr.Language,
                         CPU = _pr.CPU,
                         Part = _pr.Part,
                     };

            return Ok(pr.ToList().OrderByDescending(x => x.ID));
        }
        public IActionResult ProductBycate(int id)
        {
            var backgroundSettings = _context.MainView.FirstOrDefault();
            ViewData["BackgroundImageUrl"] = backgroundSettings?.ImgMain;

            if (id > 0)
            {
                try
                {
                    var vm = _context.Product.FirstOrDefault(x => x.Id == id);
                    ViewBag.IdCate = id;
                    ViewBag.NameCate = vm?.Name;
                    return View();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult GetAllProductByCate(int id)
        {

            var pr = from _pr in _context.Product
                     join _cate in _context.Category on _pr.CateId equals _cate.id
                     where _pr.CateId == id
                     select new
                     {
                         ID = _pr.Id,
                         Name = _pr.Name,
                         MainImg = _pr.MainImg,
                         LinkDown = _pr.LinkDown,
                         LinkDownDrop = _pr.LinkDownDrop,
                         LinkDownMedia = _pr.LinkDownMedia,
                         DetailsGame = _pr.DetailsGame,
                         DesShort = _pr.DesShort,
                         CreateDate = _pr.CreateDate,
                         CateId = _pr.CateId,
                         CategoryName = _cate.NameCate,
                         RAM = _pr.RAM,
                         GB = _pr.GB,
                         Language = _pr.Language,
                         CPU = _pr.CPU,
                     };

            return Ok(pr.ToList().OrderByDescending(x => x.ID));
        }
        [HttpGet]
        public IActionResult GetAllCategory()
        {
            var pr = from _cate in _context.Category
                     join _sub in _context.SubMenu on _cate.id equals _sub.IdCate into subGroup
                     from _sub in subGroup.DefaultIfEmpty()
                     select new
                     {
                         ID = _cate.id,
                         NameCate = _cate.NameCate,
                         MainImgCate = _cate.MainImgCate,
                         NameSub = _sub != null ? _sub.NameSub : "No SubMenu",
                         IdCate = _sub != null ? _sub.IdCate : 0,
                         idSub = _sub != null ? _sub.Id : 0
                     };

            var prWithSubMenus = pr.AsEnumerable().GroupBy(item => new { item.ID, item.NameCate, item.MainImgCate })
                .Select(group => new
                {
                    group.Key.ID,
                    group.Key.NameCate,
                    group.Key.MainImgCate,
                    SubMenus = group.Where(item => item.IdCate != 0).Select(item => new
                    {
                        item.NameSub,
                        item.IdCate,
                        item.idSub
                    }).ToList()
                });

            return Ok(prWithSubMenus.ToList());
        }

        public IActionResult DetailsProduct(int id)
        {
            var backgroundSettings = _context.MainView.FirstOrDefault();
            ViewData["BackgroundImageUrl"] = backgroundSettings?.ImgMain;
            if (id > 0)
            {
                try
                {
                    var vm = _context.Product.FirstOrDefault(x => x.Id == id);


                    if (vm == null)
                    {
                        return BadRequest("Không tìm thấy đối tượng với ID tương ứng");
                    }
                    var category = _context.Category.FirstOrDefault(c => c.id == vm.CateId);
                    if (category == null)
                    {
                        return BadRequest("Không tìm thấy danh mục tương ứng");
                    }

                    ViewBag.CategoryName = category.NameCate;

                    return View(vm);

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            else
            {
                return NotFound();
            }
        } 
        public IActionResult ProductShop()
        {
            var backgroundSettings = _context.MainView.FirstOrDefault();
            ViewData["BackgroundImageUrl"] = backgroundSettings?.ImgMain;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAllProductLayOut()
        {

            var pr = from _pr in _context.Product
                     join _cate in _context.Category on _pr.CateId equals _cate.id
                     select new
                     {
                         ID = _pr.Id,
                         Name = _pr.Name,
                         MainImg = _pr.MainImg,
                         LinkDown = _pr.LinkDown,
                         DetailsGame = _pr.DetailsGame,
                         DesShort = _pr.DesShort,
                         CreateDate = _pr.CreateDate,
                         CateId = _pr.CateId,
                         CategoryName = _cate.NameCate,
                     };

            return Ok(pr.ToList().OrderByDescending(x => x.ID));
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}