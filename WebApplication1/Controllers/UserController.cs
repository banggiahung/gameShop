using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.UserConfigViewModels;
using WebApplication1.Services;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using WebApplication1.Models.HistoryViewModels;

namespace WebApplication1.Controllers
{

    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ICommon _icommon;
        private static readonly Random random = new Random();
        private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public UserController(ILogger<UserController> logger, ApplicationDbContext context, ICommon icommon)
        {
            _logger = logger;
            _context = context;
            _icommon = icommon;
        }
        public static string GenerateRandomString(int length)
        {
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }
        public class RedirectIfAuthenticatedAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext contextRe)
            {
                var user = contextRe.HttpContext.User;
                Console.WriteLine($"IsAuthenticated: {user.Identity.IsAuthenticated}");

                if (user.Identity.IsAuthenticated)
                {
                    contextRe.Result = new RedirectToActionResult("Index", "Home", null);
                }

            }
        }

        [RedirectIfAuthenticated]
        [Route("tai-khoan/dang-ky-tai-khoan")]
        [HttpGet]
        public IActionResult Register() { return View(); }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string userName, string password, string rePass)
        {
            var existingUser = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserName == userName);
            if (existingUser != null)
            {
                return Json(new { code = 400, message = "Tài khoản đã tồn tại" });

            }
            if (rePass != password)
            {
                return Json(new { code = 400, message = "Mật khẩu không giống" });

            }
            else
            {
                var passwordHash = _icommon.GetSHA256(password);
                var IdCreate = Guid.NewGuid().ToString();
                UserConfig user = new UserConfig();
                user.UserID = IdCreate;
                user.UserName = userName;
                user.Password = passwordHash;
                user.Total = 0;
                user.CreateDate = DateTime.Now;
                user.IsAcitive = true;

                await _context.UserConfig.AddAsync(user);
                await _context.SaveChangesAsync();

                return Json(new { code = 200, message = "Đăng ký thành công" });



            }

        }

        //[HttpPost]
        //public async Task<IActionResult> Login(UserConfigCRUD login)
        //{
        //    var existingUser = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserName == login.UserName);

        //    if (existingUser == null || _icommon.GetSHA256(login.Password) != existingUser.Password)
        //    {
        //        return Json(new { code = 400, message = "Đăng nhập không thành công,kiểm tra lại tài khoản và mật khẩu" });
        //    }
        //   if(existingUser != null)
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.ASCII.GetBytes("shopgame$#!@ddategame14gamemienphi$#");
        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(new[] { new Claim("id", existingUser.UserID) }),
        //            Expires = DateTime.UtcNow.AddDays(1),
        //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //        };
        //        var token = tokenHandler.CreateToken(tokenDescriptor);
        //        var tokenString = tokenHandler.WriteToken(token);

        //        return Ok(new { Token = tokenString });
        //    }
        //    else
        //    {
        //        return Json(new { code = 400, message = "Đăng nhập không thành công,kiểm tra lại tài khoản và mật khẩu" });
        //    }


        //}
        [HttpPost]
        public async Task<IActionResult> Login(UserConfigCRUD login)
        {
            var existingUser = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserName == login.UserName);
           
            if (existingUser == null || _icommon.GetSHA256(login.Password) != existingUser.Password)
            {
                return Json(new { code = 400, message = "Đăng nhập không thành công,kiểm tra lại tài khoản và mật khẩu" });
            }
           
            if (existingUser != null)
            {
                if (existingUser.IsAcitive == false)
                {
                    return Json(new { code = 400, message = "Tài khoản đã bị khóa, liên hệ với admin" });

                }
                var token = GenerateRandomString(20);
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,existingUser.UserName),
                        new Claim(ClaimTypes.NameIdentifier, existingUser.UserID.ToString()),
                        new Claim("Token", token),
                        new Claim(ClaimTypes.Role, "Memmber")
                    };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return Json(new { code = 200, message = "Đăng nhập thành công," });

            }
            else
            {
                return Json(new { code = 400, message = "Đăng nhập không thành công,kiểm tra lại tài khoản và mật khẩu" });
            }


        }
        [HttpGet("get-id-user")]
        public IActionResult GetIdUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string token = User.FindFirstValue("Token");
                return Ok(token);

            }
            return Json(new { code = 400, message = "Đăng nhập không thành công,kiểm tra lại tài khoản và mật khẩu" });

        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult GetAllBank()
        {
            var pr = _context.BankConfig.ToList().OrderByDescending(x => x.ID);
            return Ok(pr);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRechaUser(HistoryCRUD model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var user = _context.UserConfig.Where(x => x.UserID == userId).FirstOrDefault();
                    if (user != null)
                    {
                        History buy = new History();
                        buy.isDone = false;
                        buy.UserId = user.UserID;
                        buy.PriceUser = model.PriceUser;
                        buy.BankId = model.BankId;
                        buy.ContentTransit = model.ContentTransit;
                        buy.CreateDate = DateTime.Now;

                        await _context.AddAsync(buy);

                        await _context.SaveChangesAsync();
                        _icommon.SendEmail(user, buy.PriceUser??0);
                        

                        return new JsonResult(new
                        {
                            code = 200,
                            status = "Successs",
                            message = "Đã thêm dữ liệu"
                        });
                    }
                    else
                    {

                        return new JsonResult(new
                        {
                            code = 405,
                            status = "error",
                            message = "Bạn chưa đăng nhập"
                        });
                    }
                }
                else
                {
                    return new JsonResult(new
                    {
                        code = 405,
                        status = "error",
                        message = "Bạn chưa đăng nhập"
                    });
                }




            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    code = 400,
                    status = "error",
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(int idProduct)
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.UserConfig.Where(x => x.UserID == userId).FirstOrDefault();
                if (user != null)
                {
                    if (user.Total > 0)
                    {
                        try
                        {
                            var pr = _context.Product.FirstOrDefault(x => x.Id == idProduct);
                            if (pr != null)
                            {
                                if (user.Total - 1000 < 0)
                                {
                                    return new JsonResult(new
                                    {
                                        code = 400,
                                        status = "Error",
                                        message = "Không đủ tiền để mua sản phẩm này."
                                    });
                                }
                                user.Total -= 1000;
                                await _context.SaveChangesAsync();

                                return new JsonResult(new
                                {
                                    code = 200,
                                    status = "Success",
                                    content = pr.LinkMoney,

                                });

                            }


                        }
                        catch
                        {
                            return new JsonResult(new
                            {
                                code = 405,
                                status = "False",

                            });
                        }
                    }
                    return new JsonResult(new
                    {
                        code = 400,
                        status = "Error",
                        message = "Không đủ tiền để mua sản phẩm này."
                    });
                }
                return new JsonResult(new
                {
                    code = 400,
                    status = "Error",
                    message = "Không tìm thấy user."
                });
            }

            return new JsonResult(new
            {
                code = 405,
                status = "False",

            });
        }

       
    }
}
