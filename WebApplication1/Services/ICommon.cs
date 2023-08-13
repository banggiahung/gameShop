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


		//void SendEmail(DataUser request);
		//string GenerateToken();
	}
}
