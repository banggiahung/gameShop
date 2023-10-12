using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Pagination;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static WebApplication1.Areas.AdminGame.Controllers.ProductsController;

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
        public static IDictionary<string, object> ToDictionaryWithNonNullProperties<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var dictionary = new Dictionary<string, object>();
            foreach (var property in typeof(T).GetProperties())
            {
                var value = property.GetValue(obj);
                if (value != null)
                {
                    dictionary.Add(property.Name, value);
                }
            }
            return dictionary;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.UserConfig.Where(x => x.UserID == userId).FirstOrDefault();
                if (user != null)
                {
                    ViewBag.TotalPay = user.Total;
                    ViewBag.userName = user.UserName;

                }

            }
            var backgroundSettings = _context.MainView.FirstOrDefault();
            ViewData["BackgroundImageUrl"] = backgroundSettings?.ImgMain;
            return View();
        }

        [Route("keywords/{key}")]

        public IActionResult SearchWithKey(string key)
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.UserConfig.Where(x=>x.UserID == userId).FirstOrDefault();
                if (user != null)
                {
                    ViewBag.TotalPay = user.Total;
                    ViewBag.userName = user.UserName;

                }

            }
            if (string.IsNullOrEmpty(key))
            {
                return NotFound();
            }
            var backgroundSettings = _context.MainView.FirstOrDefault();
            ViewData["BackgroundImageUrl"] = backgroundSettings?.ImgMain;
            ViewBag.Res = key;
            var re = (from _pr in _context.Product
                      join _cate in _context.Category on _pr.CateId equals _cate.id
                      where _pr.Name.Contains(key)
                      select new Product
                      {
                          Id = _pr.Id,
                          Name = _pr.Name,
                          MainImg = _pr.MainImg,
                          LinkDown = _pr.LinkDown,
                          LinkDownDrop = _pr.LinkDownDrop,
                          LinkDownMedia = _pr.LinkDownMedia,
                          DetailsGame = _pr.DetailsGame,
                          DesShort = _pr.DesShort,
                          CreateDate = _pr.CreateDate,
                          CateId = _pr.CateId,
                          RAM = _pr.RAM,
                          GB = _pr.GB,
                          Language = _pr.Language,
                          CPU = _pr.CPU,
                          Part = _pr.Part,
                          CateJson = _pr.CateJson
                      });

            var productList = re.ToList();
            return View(productList);
        }
        public IActionResult KeywordsKey()
        {
            return View();

        }

        [HttpPost]
        [Route("search/{key}")]
        public IActionResult SearchKey(string key)

        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.UserConfig.Where(x => x.UserID == userId).FirstOrDefault();
                if (user != null)
                {
                    ViewBag.TotalPay = user.Total;
                    ViewBag.userName = user.UserName;

                }

            }
            var re = (from _pr in _context.Product
                      join _cate in _context.Category on _pr.CateId equals _cate.id
                      where _pr.Name.Contains(key)
                      select new ProductViewModel
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
                          CateJson = _pr.CateJson
                      });

            var productList = re.ToList();

            foreach (var product in productList)
            {
                var cateIds = JsonConvert.DeserializeObject<List<int>>(product.CateJson);
                var categoryNames = new List<string>();

                foreach (var cateId in cateIds)
                {
                    var category = _context.Category.FirstOrDefault(c => c.id == cateId);
                    if (category != null)
                    {
                        categoryNames.Add(category.NameCate);
                    }
                }

                product.CateNames = categoryNames;
            }
            if (re != null)
            {
                return new JsonResult(new
                {
                    code = 200,
                    msg = "Thành công",
                    Products = productList.ToList().OrderByDescending(x=>x.ID)

                });
            }
            else
            {
                return new JsonResult(new
                {
                    code = 404,
                    msg = "Không tìm thấy kết quả"

                });
            }
            
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
                                  where _cate.id == _context.Category.Max(c => c.id)
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
        //[HttpGet]
        //public IActionResult GetAllProduct()
        //{

        //    var pr = from _pr in _context.Product
        //             join _cate in _context.Category on _pr.CateId equals _cate.id
        //             select new
        //             {
        //                 ID = _pr.Id,
        //                 Name = _pr.Name,
        //                 MainImg = _pr.MainImg,
        //                 LinkDown = _pr.LinkDown,
        //                 LinkDownDrop = _pr.LinkDownDrop,
        //                 LinkDownMedia = _pr.LinkDownMedia,
        //                 DetailsGame = _pr.DetailsGame,
        //                 DesShort = _pr.DesShort,
        //                 CreateDate = _pr.CreateDate,
        //                 CateId = _pr.CateId,
        //                 CategoryName = _cate.NameCate,
        //                 RAM = _pr.RAM,
        //                 GB = _pr.GB,
        //                 Language = _pr.Language,
        //                 CPU = _pr.CPU,
        //                 Part = _pr.Part,
        //                 PlayWith = _pr.PlayWith,
        //                 AmountPlayer = _pr.AmountPlayer,
        //                 CateJson = _pr.CateJson
        //             };

        //    return Ok(pr.ToList().OrderByDescending(x => x.ID));
        //}
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAllProduct()
        {
            var pr = from _pr in _context.Product
                     join _cate in _context.Category on _pr.CateId equals _cate.id
                     select new ProductViewModel
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
                         CateJson = _pr.CateJson
                     };

            var productList = pr.ToList();

            foreach (var product in productList)
            {
                var cateIds = JsonConvert.DeserializeObject<List<int>>(product.CateJson);
                var categoryNames = new List<string>();

                foreach (var cateId in cateIds)
                {
                    var category = _context.Category.FirstOrDefault(c => c.id == cateId);
                    if (category != null)
                    {
                        categoryNames.Add(category.NameCate);
                    }
                }

                product.CateNames = categoryNames;
            }

            return Ok(productList.OrderByDescending(x => x.ID));
        }
        public IActionResult ProductBycate(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.UserConfig.Where(x => x.UserID == userId).FirstOrDefault();
                if (user != null)
                {
                    ViewBag.TotalPay = user.Total;
                    ViewBag.userName = user.UserName;

                }

            }
            var backgroundSettings = _context.MainView.FirstOrDefault();
            ViewData["BackgroundImageUrl"] = backgroundSettings?.ImgMain;

            if (id > 0)
            {
                try
                {
                    var productsInCategory = _context.Product.Where(p => p.CateJson.Contains(id.ToString())).ToList();

                    ViewBag.IdCate = id;
                    ViewBag.NameCate = _context.Category.FirstOrDefault(x => x.id == id)?.NameCate;

                    return View(productsInCategory);
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
            var productsInCategory = _context.Product
                .AsEnumerable()
                .Where(p => JsonConvert.DeserializeObject<List<int>>(p.CateJson).Contains(id))
                .ToList();

            var joinedProducts = (
                from _pr in productsInCategory
                join _cate in _context.Category on id equals _cate.id
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
                    CateId = id,
                    CategoryName = _cate.NameCate,
                    RAM = _pr.RAM,
                    GB = _pr.GB,
                    Language = _pr.Language,
                    CPU = _pr.CPU,
                }
            ).ToList();

            var orderedProducts = joinedProducts.OrderByDescending(x => x.ID).ToList();

            return Ok(orderedProducts);
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
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.UserConfig.Where(x => x.UserID == userId).FirstOrDefault();
                if (user != null)
                {
                    ViewBag.TotalPay = user.Total;
                    ViewBag.userName = user.UserName;

                }

            }
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
                    var cateIds = JsonConvert.DeserializeObject<List<int>>(vm.CateJson);
                    var categoryNames = new List<string>();
                    var categoryIds = new List<int>();
                    foreach (var cateId in cateIds)
                    {
                        var category = _context.Category.FirstOrDefault(c => c.id == cateId);
                        if (category != null)
                        {
                            categoryNames.Add(category.NameCate);
                            ViewBag.CategoryName = category.NameCate;

                            categoryIds.Add(category.id);
                        }
                        if (category == null)
                        {
                            return BadRequest("Không tìm thấy danh mục tương ứng");
                        }
                    }

                    ViewBag.CategoryIds = categoryIds;
                    ViewBag.CategoryName = categoryNames;

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
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.UserConfig.Where(x => x.UserID == userId).FirstOrDefault();
                if (user != null)
                {
                    ViewBag.TotalPay = user.Total;
                    ViewBag.userName = user.UserName;

                }

            }
            var backgroundSettings = _context.MainView.FirstOrDefault();
            ViewData["BackgroundImageUrl"] = backgroundSettings?.ImgMain;
            return View();
        }

        public IActionResult Privacy()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.UserConfig.Where(x => x.UserID == userId).FirstOrDefault();
                if (user != null)
                {
                    ViewBag.TotalPay = user.Total;
                    ViewBag.userName = user.UserName;
                }

            }
            return View();
        }
        [HttpGet]
        public IActionResult GetAllProductLayOut()
        {
            var pr = from _pr in _context.Product
                     select new ProductViewModel
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
                         RAM = _pr.RAM,
                         GB = _pr.GB,
                         Language = _pr.Language,
                         CPU = _pr.CPU,
                         Part = _pr.Part,
                         CateJson = _pr.CateJson
                     };

            var productList = pr.Take(20).OrderByDescending(x => x.ID).ToList();

            foreach (var product in productList)
            {
                var cateIds = JsonConvert.DeserializeObject<List<int>>(product.CateJson);
                var categoryInfos = new List<CategoryInfo>(); // Tạo một danh sách CategoryInfo mới

                foreach (var cateId in cateIds)
                {
                    var category = _context.Category.FirstOrDefault(c => c.id == cateId);
                    if (category != null)
                    {
                        categoryInfos.Add(new CategoryInfo
                        {
                            Id = category.id,
                            Name = category.NameCate
                        });
                    }
                }

                product.CateJsonApi = categoryInfos;
            }

            return Ok(productList);
        }

        [HttpGet]
        public IActionResult GetAllProductLayOutTest([FromQuery] PaginationParams @params)
        {
            var pr = from _pr in _context.Product
                     select new ProductViewModel
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
                         RAM = _pr.RAM,
                         GB = _pr.GB,
                         Language = _pr.Language,
                         CPU = _pr.CPU,
                         Part = _pr.Part,
                         CateJson = _pr.CateJson
                     };

            var productList = pr.ToList();

            foreach (var product in productList)
            {
                var cateIds = JsonConvert.DeserializeObject<List<int>>(product.CateJson);
                var categoryInfos = new List<CategoryInfo>(); // Tạo một danh sách CategoryInfo mới

                foreach (var cateId in cateIds)
                {
                    var category = _context.Category.FirstOrDefault(c => c.id == cateId);
                    if (category != null)
                    {
                        categoryInfos.Add(new CategoryInfo
                        {
                            Id = category.id,
                            Name = category.NameCate
                        });
                    }
                }

                product.CateJsonApi = categoryInfos;
            }

            return Ok(productList.OrderByDescending(x => x.ID));
        }
        public class CategoryInfo
        {
            public int? Id { get; set; }
            public string? Name { get; set; }

        }

        //[HttpGet]
        //public IActionResult GetAllProductLayOut()
        //{

        //    var pr = from _pr in _context.Product
        //             join _cate in _context.Category on _pr.CateId equals _cate.id
        //             select new
        //             {
        //                 ID = _pr.Id,
        //                 Name = _pr.Name,
        //                 MainImg = _pr.MainImg,
        //                 LinkDown = _pr.LinkDown,
        //                 DetailsGame = _pr.DetailsGame,
        //                 DesShort = _pr.DesShort,
        //                 CreateDate = _pr.CreateDate,
        //                 CateId = _pr.CateId,
        //                 CategoryName = _cate.NameCate,
        //             };

        //    return Ok(pr.ToList().OrderByDescending(x => x.ID));
        //}
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult PostComment(Comment model)
        {
            if (ModelState.IsValid)
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var productId = model.productId;

                var currentDate = DateTime.Now;
                var threeDaysAgo = currentDate.AddDays(-3);

                var commentsByIp = _context.Comment
                    .Where(c => c.IpAddress == ipAddress && c.productId == productId && c.CreateDate >= threeDaysAgo)
                    .ToList();

                if (commentsByIp.Count >= 5)
                {
                    var errorResponse = new
                    {
                        Code = "error",
                        Status = 400,
                        Message = "Bạn không thể thêm bình luận cho Game này, Vui lòng quay lại sau 3 ngày"
                    };
                    return Json(errorResponse);
                }

                model.CreateDate = currentDate;
                model.IpAddress = ipAddress;
                model.isHeartAdmin = false;

                _context.Comment.Add(model);
                _context.SaveChanges();

                var response = new
                {
                    Code = "success",
                    Status = 200,
                    Data = model
                };

                return Json(response);
            }

            var invalidResponse = new
            {
                Code = "error",
                Status = 400,
                Message = "Invalid data"
            };

            return BadRequest(Json(invalidResponse));
        }
        
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAllComment([FromQuery] PaginationParams @params,  int id)
        {
            var pr = _context.Comment.Where(x => x.productId == id).ToList();
            var totalComment = pr.Count();
            var totalPages = (int)Math.Ceiling((double)totalComment / @params.ItemsPerPage);

            if (@params.Page < 1)
            {
                @params.Page = 1;
            }
            else if (@params.Page > totalPages)
            {
                @params.Page = totalPages;
            }
            var CommentList = pr.OrderByDescending(x => x.Id).Skip((@params.Page - 1) * @params.ItemsPerPage).Take(@params.ItemsPerPage).ToList();
            var paginationMetadata = new PaginationMetadata(totalComment, @params.Page, @params.ItemsPerPage);

            return new JsonResult(new
            {
                Pagination = new
                {
                    firstPage = Url.Action("GetAllComment", new {  page = 1, itemsPerPage = @params.ItemsPerPage }),
                    lastPage = Url.Action("GetAllComment", new {  page = totalPages, itemsPerPage = @params.ItemsPerPage }),
                    nextPage = @params.Page < totalPages ? Url.Action("GetAllComment", new { page = @params.Page + 1, itemsPerPage = @params.ItemsPerPage }) : null,
                    previousPage = @params.Page > 1 ? Url.Action("GetAllComment", new { page = @params.Page - 1, itemsPerPage = @params.ItemsPerPage }) : null,
                    currentPage = @params.Page,
                    totalCount = totalComment,
                    totalPages,
                    hasPrevious = @params.Page > 1,
                    hasNext = @params.Page < totalPages
                },
                Data = CommentList
            });
        }
        [HttpGet]
        public async Task<IActionResult> getIdProducts(int id)
        {
            try
            {
                if (id > 0)
                {
                    try
                    {
                      var vm = await _context.Product.FirstOrDefaultAsync(x => x.Id == id);

                        if (vm == null)
                        {
                            return BadRequest("Không tìm thấy đối tượng với ID tương ứng");
                        }
                        else
                        {
                            Product list = new Product();
                            list.Id = vm.Id;
                            list.Name = vm.Name;
                            list.MainImg = vm.MainImg;
                            list.LinkDown = vm.LinkDown;
                            list.LinkDownDrop = vm.LinkDownDrop;
                            list.LinkDownMedia = vm.LinkDownMedia;
                            list.DetailsGame = vm.DetailsGame;
                            list.DesShort = vm.DesShort;
                            list.RAM = vm.RAM;
                            list.GB = vm.GB;
                            list.Language = vm.Language;
                            list.CPU = vm.CPU;
                            list.GB = vm.GB;
                            list.Part = vm.Part;
                            list.PlayWith = vm.PlayWith;
                            list.CateJson = vm.CateJson;
                            list.CateJson = vm.CateJson;
                            list.MoreLink = vm.MoreLink;
                            list.MoreLink = vm.MoreLink;
                            list.CateId = vm.CateId;
                            list.AmountPlayer = vm.AmountPlayer;
                            list.CreateDate = vm.CreateDate;

                            return Ok(list);

                        }
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }

}