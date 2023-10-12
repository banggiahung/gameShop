using Microsoft.AspNetCore.Identity;
using System.Drawing;
using System.Collections;
using WebApplication1.Models;
namespace WebApplication1.Services
{
    public interface ICommon
    {
        Task<string> UploadedFile(IFormFile ProfilePicture);
        string GetSHA256(string str);
		string GetMD5(string str);

        void SendEmail(UserConfig user, int PayMoney);
        void SendEmailUserNap(UserConfig user, int payMoney);

        //void SendEmail(DataUser request);
        //string GenerateToken();
    }
}
