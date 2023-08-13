using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Services;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Areas.AdminGame.Controllers
{
    [Area("AdminGame")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductsController> _logger;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _env;


        public ProductsController(ApplicationDbContext context, IConfiguration configuration, ILogger<ProductsController> logger, ICommon common, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _iCommon = common;
            _env = env;
        }

        // GET: AdminGame/Products
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                // Lưu lại URL hiện tại để chuyển hướng sau khi đăng nhập thành công
                string returnUrl = Url.Action("Index", "Products");
                return RedirectToAction("Login", "Products", new { returnUrl });
            }

            // Đã đăng nhập và có quyền "Admin", tiếp tục hiển thị trang Index
            ViewBag.checkActive = "trangChu";
            return View();
        }
        [HttpPost]
        public  IActionResult ChangePassword(string pass)
        {
            if (ModelState.IsValid)
            {
                string appSettingsPath = Path.Combine(_env.ContentRootPath, "appsettings.json");

                JObject appSettingsJson = JObject.Parse(System.IO.File.ReadAllText(appSettingsPath));

                appSettingsJson["AdminShopMain"]["Password"] = pass;

                System.IO.File.WriteAllText(appSettingsPath, appSettingsJson.ToString());

                return Ok();
            }

            return Ok();
        }

        [AllowAnonymous]
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
        //Thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product model, ImgViewModel img)
        {
            try
            {
                if (img.PrPath != null)
                {
                    var PrPath = await _iCommon.UploadedFile(img.PrPath);
                    model.MainImg = "/upload/" + PrPath;
                }
                model.CreateDate = DateTime.Now;
                _context.Product.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        public class ImgViewModel
        {
            public IFormFile? PrPath { get; set; }
        }
        public class ImgViewModelBanner
        {
            public IFormFile? PrPath1 { get; set; }
            public IFormFile? PrPath2 { get; set; }
            public IFormFile? PrPath3 { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> getIdProducts(int id)
        {
            try
            {
                Product vm = new Product();
                if (id > 0)
                {
                    try
                    {
                        vm = await _context.Product.FirstOrDefaultAsync(x => x.Id == id);

                        if (vm == null)
                        {
                            return BadRequest("Không tìm thấy đối tượng với ID tương ứng");
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


                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> deleteProducts(int id)
        {
            try
            {
                var existingProduct = await _context.Product.FirstOrDefaultAsync(x => x.Id == id);

                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.Product.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category model, ImgViewModel img)
        {
            try
            {
                if (img.PrPath != null)
                {
                    var PrPath = await _iCommon.UploadedFile(img.PrPath);
                    model.MainImgCate = "/upload/" + PrPath;
                }
                _context.Category.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [AllowAnonymous]
        [HttpGet]

        public IActionResult GetAllCategory()
        {
            var pr = _context.Category.ToList().OrderByDescending(x=>x.id);
            return Ok(pr);
        }
        [AllowAnonymous]
        [HttpGet]

        public IActionResult GetAllNoti()
        {
            var pr = _context.NotiWeb.ToList().OrderByDescending(x => x.id);
            return Ok(pr);
        }
        [HttpGet]
        public async Task<IActionResult> getIdNoti(int id)
        {
            try
            {
                NotiWeb vm = new NotiWeb();
                if (id > 0)
                {
                    try
                    {
                        vm = await _context.NotiWeb.FirstOrDefaultAsync(x => x.id == id);

                        if (vm == null)
                        {
                            return BadRequest("Không tìm thấy đối tượng với ID tương ứng");
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


                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNoti(NotiWeb model)
        {
            try
            {
                model.CreateDate = DateTime.Now;
                _context.NotiWeb.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateNoti(NotiWeb model)
        {
            try
            {
                var existingProduct = await _context.NotiWeb.FindAsync(model.id);

                if (existingProduct == null)
                {
                    return NotFound("Không tìm thấy");
                }

                existingProduct.Noti = model.Noti;
                existingProduct.CreateDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNoti(NotiWeb model)
        {
            try
            {
                var existingProduct = await _context.NotiWeb.FirstOrDefaultAsync(x => x.id == model.id);

                if (existingProduct == null)
                {
                    return NotFound();
                }
                _context.NotiWeb.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);


            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        //sub menu

        [AllowAnonymous]
        [HttpGet]

        public IActionResult GetAllSub()
        {
            var pr = from _pr in _context.SubMenu
                     join _cate in _context.Category on _pr.IdCate equals _cate.id
                     select new
                     {
                         ID = _pr.Id,
                         CateId = _cate.id,
                         CategoryName = _cate.NameCate,
                         NameSub = _pr.NameSub,
                         IdCate = _pr.IdCate,
                     };

            return Ok(pr.ToList().OrderByDescending(x => x.ID));
        }
        [HttpGet]
        public async Task<IActionResult> getIdSubMenu(int id)
        {
            try
            {
                SubMenu vm = new SubMenu();
                if (id > 0)
                {
                    try
                    {
                        vm = await _context.SubMenu.FirstOrDefaultAsync(x => x.Id == id);

                        if (vm == null)
                        {
                            return BadRequest("Không tìm thấy đối tượng với ID tương ứng");
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


                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddSubMenu(SubMenu model)
        {
            try
            {
                _context.SubMenu.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateSubMenu(SubMenu model)
        {
            try
            {
                var existingProduct = await _context.SubMenu.FindAsync(model.Id);

                if (existingProduct == null)
                {
                    return NotFound("Không tìm thấy");
                }

                existingProduct.IdCate = model.IdCate;
                existingProduct.NameSub = model.NameSub;
                await _context.SaveChangesAsync();

                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubMenu(SubMenu model)
        {
            try
            {
                var existingProduct = await _context.SubMenu.FirstOrDefaultAsync(x => x.Id == model.Id);

                if (existingProduct == null)
                {
                    return NotFound();
                }
                _context.SubMenu.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);


            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        //MainView
        [AllowAnonymous]
        [HttpGet]

        public IActionResult GetAllMainView()
        {
            var pr = _context.MainView;
            return Ok(pr.ToList().OrderByDescending(x => x.id));
        }
        [HttpGet]
        public async Task<IActionResult> getMainView(int id)
        {
            try
            {
                MainView vm = new MainView();
                if (id > 0)
                {
                    try
                    {
                        vm = await _context.MainView.FirstOrDefaultAsync(x => x.id == id);

                        if (vm == null)
                        {
                            return BadRequest("Không tìm thấy đối tượng với ID tương ứng");
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


                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
       
        [HttpPost]
        public async Task<IActionResult> UpdateMainView(MainView model, ImgViewModel img, ImgViewModelBanner banner)
        {
            try
            {
                var existingProduct = await _context.MainView.FindAsync(model.id);

                if (existingProduct == null)
                {
                    return NotFound("Không tìm thấy");
                }
                if (img.PrPath != null)
                {
                    var PrPath = await _iCommon.UploadedFile(img.PrPath);
                    existingProduct.ImgMain = "/upload/" + PrPath;
                }
                else
                {
                    existingProduct.ImgMain = existingProduct.ImgMain;
                }
                if(banner.PrPath1 != null)
                {
                    var PrPath1 = await _iCommon.UploadedFile(banner.PrPath1);
                    existingProduct.Banner1 = "/upload/" + PrPath1;
                }
                else
                {
                    existingProduct.Banner1 = existingProduct.Banner1;
                }
                if (banner.PrPath2 != null)
                {
                    var PrPath2 = await _iCommon.UploadedFile(banner.PrPath2);
                    existingProduct.Banner2 = "/upload/" + PrPath2;
                }
                else
                {
                    existingProduct.Banner2 = existingProduct.Banner2;
                }
                if (banner.PrPath3 != null)
                {
                    var PrPath3 = await _iCommon.UploadedFile(banner.PrPath3);
                    existingProduct.Banner3 = "/upload/" + PrPath3;
                }
                else
                {
                    existingProduct.Banner3 = existingProduct.Banner3;
                }
                await _context.SaveChangesAsync();

                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
       
        public IActionResult Categories()
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                // Lưu lại URL hiện tại để chuyển hướng sau khi đăng nhập thành công
                string returnUrl = Url.Action("Index", "Products");
                return RedirectToAction("Login", "Products", new { returnUrl });
            }
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                // Already logged in and is an "Admin," redirect to dashboard
                return RedirectToAction("Index", "Products");
            }

            // Not logged in or not an "Admin," show login view
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, bool RememberMe)
        {

            if (userName == _configuration["AdminShopMain:Username"] && password == _configuration["AdminShopMain:Password"])
            {
                var claims = new List<Claim>
                    {
                     new Claim(ClaimTypes.Name,userName),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = (bool)RememberMe
                };

                await HttpContext.SignInAsync(
                          CookieAuthenticationDefaults.AuthenticationScheme,
                          new ClaimsPrincipal(claimsIdentity),
                          authProperties);
                return Redirect("/Products/Index");
            }
            ModelState.AddModelError(string.Empty, "Sai mật khẩu,admin");
            ViewBag.LoginAdmin = "Sai mật khẩu,admin";
            return View();

        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Products");
        }
        // GET: AdminGame/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: AdminGame/Products/Create
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                // Lưu lại URL hiện tại để chuyển hướng sau khi đăng nhập thành công
                string returnUrl = Url.Action("Index", "Products");
                return RedirectToAction("Login", "Products", new { returnUrl });
            }
            return View();
        }

        // POST: AdminGame/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: AdminGame/Products/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                // Lưu lại URL hiện tại để chuyển hướng sau khi đăng nhập thành công
                string returnUrl = Url.Action("Index", "Products");
                return RedirectToAction("Login", "Products", new { returnUrl });
            }

            if (id > 0)
            {
                try
                {
                    var vm = _context.Product.FirstOrDefault(x => x.Id == id);

                    if (vm == null)
                    {
                        return BadRequest("Không tìm thấy đối tượng với ID tương ứng");
                    }
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
        // POST: AdminGame/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditProduct([FromForm] int id, IFormCollection formData, ImgViewModel img)
        {
            var edit = await _context.Product.FindAsync(id);
            if (id != edit.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (img.PrPath != null)
                    {
                        var PrPath = await _iCommon.UploadedFile(img.PrPath);
                        edit.MainImg = "/upload/" + PrPath;
                    }
                    else
                    {
                        edit.MainImg = edit.MainImg;
                    }

                    edit.Name = formData["Name"];
                    edit.RAM = formData["RAM"];
                    edit.GB = formData["GB"];
                    edit.Language = formData["Language"];
                    edit.CPU = formData["CPU"];
                    edit.LinkDown = formData["LinkDown"];
                    edit.LinkDownDrop = formData["LinkDownDrop"];
                    edit.LinkDownMedia = formData["LinkDownMedia"];
                    edit.DetailsGame = formData["DetailsGame"];
                    edit.DesShort = formData["DesShort"];
                    edit.Part = formData["Part"];
                    edit.CateId =  Convert.ToInt32(formData["CateId"]);
                    edit.CreateDate = DateTime.Now;
                    _context.Update(edit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(edit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok(edit);

            }
            return Ok();
        }

        // GET: AdminGame/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: AdminGame/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       
        [HttpGet]
        public async Task<IActionResult> getIdCategory(int id)
        {
            try
            {
                Category vm = new Category();
                if (id > 0)
                {
                    try
                    {
                        vm = await _context.Category.FirstOrDefaultAsync(x => x.id == id);

                        if (vm == null)
                        {
                            return BadRequest("Không tìm thấy đối tượng với ID tương ứng");
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


                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category model, ImgViewModel img)
        {
            try
            {
                var existingProduct = await _context.Category.FindAsync(model.id);

                if (existingProduct == null)
                {
                    return NotFound("Không tìm thấy");
                }

                if (img.PrPath != null)
                {
                    var PrPath = await _iCommon.UploadedFile(img.PrPath);
                    existingProduct.MainImgCate = "/upload/" + PrPath;
                }

                existingProduct.NameCate = model.NameCate;
                await _context.SaveChangesAsync();

                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Category model)
        {
            try
            {
                var existingProduct = await _context.Category.FirstOrDefaultAsync(x => x.id == model.id);
               

                if (existingProduct == null)
                {
                    return NotFound();

                }
                var subMenuItems = await _context.SubMenu.Where(sub => sub.IdCate == model.id).ToListAsync();
                _context.SubMenu.RemoveRange(subMenuItems);
                _context.Category.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);


            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost]
        public IActionResult UploadLocal(List<IFormFile> files)
        {
            var filePath = "";
            foreach(IFormFile photo in Request.Form.Files)
            {
                string sv = Path.Combine(_env.WebRootPath,"upload", photo.FileName);
                using(var stream = new FileStream(sv, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                filePath = "https://localhost:44328/" + "upload" + photo.FileName;
            }
            return Json(new { url = filePath });

        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult UploadLocalMain(List<IFormFile> files, [FromServices] IUrlHelperFactory urlHelperFactory)
        {
            var filePaths = new List<string>();

            foreach (IFormFile photo in Request.Form.Files)
            {
                string sv = Path.Combine(_env.WebRootPath, "upload", photo.FileName);
                using (var stream = new FileStream(sv, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                string relativePath = $"~/upload/{photo.FileName}";
                string absolutePath = Url.Content(relativePath);

                filePaths.Add(absolutePath);
            }

            return Json(new { urls = filePaths });
        }


        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
