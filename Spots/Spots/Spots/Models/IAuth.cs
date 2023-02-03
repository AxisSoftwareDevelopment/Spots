using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Models
{
    public interface IAuth
    {
        Task<string> LogInWithEmailAndPasswordAsync(string email, string password);
        Task<string> RegisterWithEmailAndPasswordAsync(string email, string password);
    }
}
