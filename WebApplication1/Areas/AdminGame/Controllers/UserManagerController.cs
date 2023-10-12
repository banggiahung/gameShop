using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication1.Models.UserConfigViewModels;
using Microsoft.EntityFrameworkCore;

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
	}
}
