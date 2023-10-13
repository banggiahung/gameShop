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
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{

    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ICommon _icommon;
        private static readonly Random random = new Random();
        private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        public UserController(ILogger<UserController> logger, ApplicationDbContext context, ICommon icommon, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _icommon = icommon;
            _clientFactory = clientFactory;
            _configuration = configuration;
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
        public class Transaction
        {
            public string TransactionID { get; set; }
            public string Amount { get; set; }
            public string Description { get; set; }
            public string TransactionDate { get; set; }
            public string Type { get; set; }
        }
        public class BankApiResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public List<Transaction> Transactions { get; set; }
        }

        public async Task<List<Transaction>> GetBankTransactions()
        {
            var stk = _configuration.GetSection("SoTkNganHang").Value;
            var password = _configuration.GetSection("MatKhauNganHang").Value;
            var Token = _configuration.GetSection("Token").Value;

            var httpClient = _clientFactory.CreateClient();
            var response = await httpClient.GetAsync($"https://api.web2m.com/historyapivcbv3/{password}/{stk}/{Token}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bankApiResponse = JsonConvert.DeserializeObject<BankApiResponse>(json);

                if (bankApiResponse.Status && bankApiResponse.Transactions != null)
                {
                    // Lấy giao dịch có Type là "IN" đầu tiên
                    var latestInTransaction = bankApiResponse.Transactions.FirstOrDefault(t => t.Type == "IN");

                    // Nếu có giao dịch, trả về danh sách chứa giao dịch đó
                    return latestInTransaction != null ? new List<Transaction> { latestInTransaction } : new List<Transaction>();
                }
                else
                {
                    throw new Exception("API response status is false or transactions are null");
                }
            }
            else
            {
                throw new Exception("Failed to retrieve bank transactions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestApiResult(string GiaoDich)
        {
            var maxAttempts = 3; // Số lần kiểm tra lại tối đa
            var delayBetweenAttempts = TimeSpan.FromSeconds(3); // Khoảng thời gian giữa các lần kiểm tra

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var bankTransactions = await GetBankTransactions();
                var matchingTransaction = bankTransactions
     .FirstOrDefault(t =>
     {
         var splitDescription = t.Description.Split('.');

         return splitDescription.Any(part => part.Trim() == GiaoDich.Trim());
     });
                if (matchingTransaction != null)
                {
                    decimal amount = Convert.ToDecimal(matchingTransaction.Amount);
                    return new JsonResult(new
                    {
                        code = 200,
                        status = "Success",
                        message = amount
                    });
                }

                // Nếu không tìm thấy giao dịch và còn dưới số lần kiểm tra tối đa, đợi 5s rồi thử lại
                if (attempt < maxAttempts - 1)
                {
                    await Task.Delay(delayBetweenAttempts);
                }
            }

            return new JsonResult(new
            {
                code = 400,
                status = "Failed",
                message = "No matching transaction found after " + maxAttempts + " attempts."
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRechaUser(HistoryCRUD model)
        {
            try
            {
                if (model.PriceUser < 10000)
                {
                    return new JsonResult(new
                    {
                        code = 405,
                        status = "error",
                        message = "Số tiền phải lớn hơn 10.000đ"
                    });
                }
                if (User.Identity.IsAuthenticated)
                {
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var user = _context.UserConfig.Where(x => x.UserID == userId).FirstOrDefault();
                    
                    if (user != null)
                    {
                        History buy = new History();
                        buy.UserId = user.UserID;
                        buy.PriceUser = model.PriceUser;
                        buy.BankId = model.BankId;
                        buy.ContentTransit = model.ContentTransit;
                        buy.CreateDate = DateTime.Now;

                        await _context.AddAsync(buy);
                        var maxAttempts = 3; 
                        var delayBetweenAttempts = TimeSpan.FromSeconds(10); 

                        for (int attempt = 0; attempt < maxAttempts; attempt++)
                        {
                            var bankTransactions = await GetBankTransactions();
                            var matchingTransaction = bankTransactions
      .FirstOrDefault(t =>
      {
          var splitDescription = t.Description.Split('.');

          return splitDescription.Any(part => part.Trim() == model.ContentTransit.Trim());
      });

                            if (matchingTransaction != null)
                            {
                                if (int.TryParse(matchingTransaction.Amount, out int transactionAmount) && transactionAmount == model.PriceUser)
                                {
                                    if (transactionAmount == model.PriceUser)
                                    {
                                        buy.isDone = true;
                                        user.Total += model.PriceUser;
                                        await _context.SaveChangesAsync();
                                        _icommon.SendEmailUserNap(user, buy.PriceUser ?? 0);
                                        return new JsonResult(new
                                        {
                                            code = 200,
                                            status = "Success",
                                            message = "Đã nạp tiền thành công"
                                        });
                                    }
                                    else
                                    {
                                        buy.isDone = false;
                                        await _context.SaveChangesAsync();
                                        _icommon.SendEmail(user, buy.PriceUser ?? 0);
                                        return new JsonResult(new
                                        {
                                            code = 200,
                                            status = "Success",
                                            message = "Số tiền của bạn chọn không đúng với số tiền chuyển, tiền sẽ được cộng theo số tiền admin nhận"
                                        });
                                    }
                                }
                               
                            }

                            if (attempt < maxAttempts - 1)
                            {
                                await Task.Delay(delayBetweenAttempts);
                            }
                        }
                        buy.isDone = false;
                        await _context.SaveChangesAsync();

                        _icommon.SendEmail(user, buy.PriceUser ?? 0);

                        return new JsonResult(new
                        {
                            code = 200,
                            status = "Success",
                            message = "Chúng tôi chưa nhận được tiền, vui lòng kiểm tra lại hoặc liên hệ admin"
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


                                if (!string.IsNullOrEmpty(pr.LinkMoney))
                                {
                                    user.Total -= 1000;
                                    await _context.SaveChangesAsync();
                                    return new JsonResult(new
                                    {
                                        code = 200,
                                        status = "Success",
                                        content = pr.LinkMoney,

                                    });
                                }
                                else
                                {
                                    return new JsonResult(new
                                    {
                                        code = 200,
                                        status = "Success",
                                        content = "<p>Hiện tại chưa có link</p><br /><p>Số tiền của bạn đã được hoàn</p>",

                                    });

                                }
                               

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
        [HttpGet]
        public IActionResult GetAllTitile()
        {
            var pr = from _page in _context.PageCustom
                     select new PageCustom
                     {
                         ID = _page.ID,
                         NameCustom = _page.NameCustom,
                         SlugCustom = _page.SlugCustom
                     };
            var item = pr.Select(ToDictionaryWithNonNullProperties).ToList();
            return new JsonResult(new
            {
                code = 200,
                status = "Success",
                Page = item,

            });

        }
        [Route("su-kien/{slug}")]
        public IActionResult PageCustom(string slug)
        {
            var backgroundSettings = _context.MainView.FirstOrDefault();
            ViewData["BackgroundImageUrl"] = backgroundSettings?.ImgMain;
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
            var pr = _context.PageCustom.FirstOrDefault(x => x.SlugCustom == slug);
            if (pr != null)
            {
                ViewBag.SlugPage = pr.NameCustom;
                return View(pr);

            }
            return NotFound();


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

    }
}
