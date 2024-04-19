using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots;
public interface IUser
{
    public string FullName { get; }
    public string FullPhoneNumber { get; }
    public string UserID { get; set; }
    public ImageSource ProfilePictureSource { get; set; }
    public string Email {  get; set; }
}
