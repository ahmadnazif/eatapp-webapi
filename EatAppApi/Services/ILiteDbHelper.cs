using EatAppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Services
{
    [Obsolete]
    public interface ILiteDbHelper
    {
        // Fnb
        List<Fnb> ListAllFnb();
        Fnb GetFnbByName(string name);
        Fnb GetFnbById(int id);
        string AddFnb(Fnb fnb);
        string AddBatchFnb(List<Fnb> fnbList);
        string UpdateFnb(int id, Fnb fnb);
        bool IsFnbExist(string name);

        // Fnb comment
        string AddFnbComment(int fnbId, string commenterName, string comment, int rating);
        List<FnbComment> ListAllFnbComment(int fnbId);
        string DeleteAllComment(int fnbId);

        // Picture
        string AddFnbPicture(int fnbId, string addedBy, string picturePath);
        List<FnbPicture> ListAllFnbPicture(int fnbId);

        // User
        UserProfile GetUserById(int id);
        UserProfile GetUserByUsername(string username);
        bool IsUsernameExist(string username);
        string AddUser(string username, string passwordHash, string email);
        List<UserProfile> ListAllUser();
        (bool success, string message) ChangePassword(int userId, string newPasswordHash);
        (bool success, string message) UpdateUser(int userId, string email, string avatar);
    }
}
