using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RoslynAnalyzerDemo;

public static class Program
{
    public static void Main()
    {

    }

    public static void Test()
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuer = false,
            RequireExpirationTime = false,
            ValidateAudience = false
        };
    }

    public static void Test2(){
        using var md5 = MD5.Create();
        md5.ComputeHash(Encoding.UTF8.GetBytes("Test"));
    }
}