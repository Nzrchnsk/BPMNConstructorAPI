using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BPMNConstructorAPI.Models
{
    public class AuthOptions
    {  
        public const string ISSUER = "ESportAPI";
        public const string AUDIENCE = "EsportUI"; 
        const string KEY = "dvddfvergertert123123"; 
        public const int LIFETIME = 100; 
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
        
    }
}