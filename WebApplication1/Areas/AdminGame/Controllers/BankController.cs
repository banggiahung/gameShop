using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.BankViewModels;
using WebApplication1.Models.HistoryViewModels;
using WebApplication1.Services;

namespace WebApplication1.Areas.AdminGame.Controllers
{
    [Area("AdminGame")]

    public class BankController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BankController> _logger;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _env;


        public BankController(ApplicationDbContext context, IConfiguration configuration, ILogger<BankController> logger, ICommon common, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _iCommon = common;
            _env = env;
        }
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                // Lưu lại URL hiện tại để chuyển hướng sau khi đăng nhập thành công
                string returnUrl = Url.Action("Index", "Products");
                return RedirectToAction("Login", "Products", new { returnUrl });
            }
            return View();
        } 
        public IActionResult UserNap()
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                // Lưu lại URL hiện tại để chuyển hướng sau khi đăng nhập thành công
                string returnUrl = Url.Action("Index", "Products");
                return RedirectToAction("Login", "Products", new { returnUrl });
            }
            return View();
        }

        //bank
        [AllowAnonymous]
        [HttpGet]

        public IActionResult GetAllBank()
        {
            var pr = _context.BankConfig.ToList().OrderByDescending(x => x.ID);
            return Ok(pr);
        }
        [HttpPost]
        public async Task<IActionResult> AddBankData(BankCRUD model)
        {
            try
            {
                if (model.ImgPr != null)
                {
                    var PrPath = await _iCommon.UploadedFile(model.ImgPr);
                    model.QrBank = "/Upload/" + PrPath;
                }
                if (model.ImgPrLogo != null)
                {
                    var PrPath = await _iCommon.UploadedFile(model.ImgPrLogo);
                    model.ImgBank = "/Upload/" + PrPath;

                }

                _context.BankConfig.Add(model);
                await _context.SaveChangesAsync();

                return Ok(model);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
        [HttpPost]
        public async Task<IActionResult> editBank([FromForm] BankCRUD model)
        {
            var edit = await _context.BankConfig.FindAsync(model.ID);
            if (edit == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.ImgPr != null)
                    {
                        var PrPath = await _iCommon.UploadedFile(model.ImgPr);
                        model.QrBank = "/Upload/" + PrPath;

                        edit.QrBank = model.QrBank;
                    }
                    else
                    {
                        edit.QrBank = edit.QrBank;
                    }


                    edit.LoaiBank = model.LoaiBank;
                    edit.BankName = model.BankName;
                    edit.BankAcc = model.BankAcc;
                    edit.BankUser = model.BankUser;
                    _context.Update(edit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    return NotFound();
                }
                return Ok(edit);

            }
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> getIdBank(int id)
        {
            try
            {
                BankConfig vm = new BankConfig();
                if (id > 0)
                {
                    try
                    {
                        vm = await _context.BankConfig.FirstOrDefaultAsync(x => x.ID == id);

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
        public async Task<IActionResult> deleteBankAdmin(int id)
        {
            try
            {
                var existingProduct = await _context.BankConfig.FirstOrDefaultAsync(x => x.ID == id);

                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.BankConfig.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpGet]
        public IActionResult GetAllListDataRecharage()
        {

            var pr = from _pr in _context.History
                     join _user in _context.UserConfig on _pr.UserId equals _user.UserID
                     join _dt in _context.BankConfig on _pr.BankId equals _dt.ID
                     select new HistoryCRUD
                     {
                         ID = _pr.ID,
                         UserId = _pr.UserId,
                         PriceUser = _pr.PriceUser,
                         BankId = _pr.BankId,
                         ContentTransit = _pr.ContentTransit,
                         isDone = _pr.isDone,
                         BankName = _dt.BankName,
                         BankAccount = _dt.BankAcc,
                         NameAccount = _dt.BankUser,
                         LoaiBank = _dt.LoaiBank,
                         UserName = _user.UserName,
                         CreateDate = _pr.CreateDate,

                     };
            var userList = pr.OrderByDescending(x => x.ID).ToList();
            return new JsonResult(new
            {
                code = 200,
                status = "Success",
                DataUser = userList
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRecharage(int id)
        {
            try
            {
                var data = await _context.History.FindAsync(id);
                if (data != null)
                {
                    data.isDone = true;
                    await _context.SaveChangesAsync();

                    if (data.isDone == true)
                    {
                        var dataUser = await _context.UserConfig.FirstOrDefaultAsync(u => u.UserID == data.UserId);
                        if (dataUser != null)
                        {
                            dataUser.Total = dataUser.Total + data.PriceUser;
                            await _context.SaveChangesAsync();

                            if (data.PriceUser.HasValue)
                            {
                                _iCommon.SendEmailUserNap(dataUser, data.PriceUser.Value);
                            }
                        }
                        await _context.SaveChangesAsync();
                        return Ok(data);
                    }
                    else
                    {
                        return NotFound();
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
