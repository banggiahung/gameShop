using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication1.Models.UserConfigViewModels;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.PageCustomViewModels;

namespace ShopBanVe.Areas.Admin.Controllers
{
    [Area("AdminGame")]
    public class UserManagerController : Controller
	{
		private readonly ApplicationDbContext _context;

		public UserManagerController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Customer()
		{
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                // Lưu lại URL hiện tại để chuyển hướng sau khi đăng nhập thành công
                string returnUrl = Url.Action("CommentUser", "Products");
                return RedirectToAction("Login", "Products", new { returnUrl });
            }
            var data = from us in _context.UserConfig
					   select new UserConfigCRUD
                       {
						   UserID = us.UserID,
						   UserName = us.UserName,
						   Total = us.Total,
						   IsAcitive = us.IsAcitive,
						   CreateDate = us.CreateDate,
					   };
			return View(data.ToList());
		}
		public IActionResult PageCustom()
		{
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                // Lưu lại URL hiện tại để chuyển hướng sau khi đăng nhập thành công
                string returnUrl = Url.Action("CommentUser", "Products");
                return RedirectToAction("Login", "Products", new { returnUrl });
            }
            return View();
		}

		[HttpGet]
		public IActionResult GetData()
		{
			JsonResultVM json = new JsonResultVM();
			try
			{
				json.Success = true;
				json.Mesaage = "";
				var data = from us in _context.UserConfig
                           select new
						   {
                               UserID = us.UserID,
                               Total = us.Total,
							   IsActive = us.IsAcitive,
							   UserName = us.UserName,
						   };
				json.Object = data.ToList();
				return Ok(json);
			}
			catch (Exception ex)
			{
				json.Mesaage = ex.Message;
				json.Success = false;
				json.Object = null;
				return Ok(json);
			}
		}
		[HttpGet]
		public async Task<IActionResult> ChangeActive(string id)
		{
			try
			{
				var user = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserID == id);
				if(user != null)
				{
                    user.IsAcitive = !user.IsAcitive;
                    await _context.SaveChangesAsync();
                    return Ok();

                }
				return BadRequest();

            }
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet]
		public async Task<IActionResult> DeleteUser(string id)
		{
			try
			{
                var user = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserID == id);
				if(user != null)
				{
                    _context.UserConfig.Remove(user);
                    await _context.SaveChangesAsync();

                    return Ok();
                }
				return BadRequest();

            }
            catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
        [HttpGet]
        public IActionResult GetAllPage()
        {
            var pr = _context.PageCustom.OrderByDescending(x => x.ID).ToList();
            return Ok(pr);
        }
        [HttpPost]
        public IActionResult AddPage(PageCustom model)
        {

            try
            {
                model.CreateDate = DateTime.Now;
                _context.PageCustom.Add(model);
                _context.SaveChanges();
                return Ok(model);

            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePage(PageCustomCRUD model)
        {

            try
            {
                var existingProduct = await _context.PageCustom.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return BadRequest();

                }
                else
                {
                    existingProduct.SlugCustom = model.SlugCustom;
                    existingProduct.NameCustom = model.NameCustom;
                    existingProduct.Custom = model.Custom;
                    existingProduct.CreateDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                return Ok(existingProduct);


            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> getIdPage(int id)
        {
            try
            {
                PageCustom vm = new PageCustom();
                if (id > 0)
                {
                    try
                    {
                        vm = await _context.PageCustom.FirstOrDefaultAsync(x => x.ID == id);

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
        public async Task<IActionResult> DeletePage(PageCustom model)
        {
            try
            {
                var existingProduct = await _context.PageCustom.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.PageCustom.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
