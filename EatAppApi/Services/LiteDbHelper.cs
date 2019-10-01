using EatAppApi.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Services
{
    public class LiteDbHelper : ILiteDbHelper
    {
        private readonly string n = Environment.NewLine;
        private static readonly ConnectionString conString;

        static LiteDbHelper()
        {
            var dbName = "appdb.db";
            var dbRootPath = $@"{Directory.GetCurrentDirectory()}";
            var dbPath = $@"{dbRootPath}\{dbName}";

            var rpExist = Directory.Exists(dbRootPath);
            if (!rpExist)
                Directory.CreateDirectory(dbRootPath);

            var dbExist = File.Exists(dbPath);
            if (!dbExist)
            {
                Stopwatch sw = Stopwatch.StartNew();
                File.Create(dbPath);
                sw.Stop();
            }

            conString = new ConnectionString
            {
                Filename = dbName
            };
        }

        // FNB

        public List<Fnb> ListAllFnb()
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<Fnb>();
                    return data.FindAll().ToList();
                }
            }
            catch
            {
                return new List<Fnb>();
            }
        }

        public Fnb GetFnbByName(string name)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<Fnb>();
                    return data.FindOne(d => d.Name.ToLower().Contains(name.ToLower()));
                }
            }
            catch
            {
                return null;
            }
        }

        public Fnb GetFnbById(int id)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<Fnb>();
                    return data.FindOne(d => d.Id == id);
                }
            }
            catch
            {
                return null;
            }
        }

        public string AddFnb(Fnb fnb)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<Fnb>();
                    var id = data.Insert(fnb);
                    return $"'{fnb.Name}' added with ID '{id.AsString}'";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AddBatchFnb(List<Fnb> fnbList)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<Fnb>();
                    var count = data.Insert(fnbList);
                    return $"{count} data inserted";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string UpdateFnb(int id, Fnb fnb)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<Fnb>();
                    var succ = data.Update(id, fnb);
                    return succ ? "Success" : "Failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool IsFnbExist(string name)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var coll = db.GetCollection<Fnb>();
                    return coll.FindOne(d => d.Name.ToLower() == name.ToLower()) != null;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // COMMENT

        public string AddFnbComment(int fnbId, string commenterName, string comment, int rating)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var coll = db.GetCollection<FnbComment>();
                    var id = coll.Insert(new FnbComment
                    {
                        FnbId = fnbId,
                        //CommenterName = commenterName,
                        Comment = comment,
                        Rating = rating,
                        //TimeAdded = DateTime.Now
                    });

                    return $"Comment & rating added. Total comment: {ListAllFnbComment(fnbId).Count}";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<FnbComment> ListAllFnbComment(int fnbId)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<FnbComment>();
                    return data.FindAll().Where(d => d.FnbId == fnbId).ToList();
                }
            }
            catch
            {
                return new List<FnbComment>();
            }
        }

        public string DeleteAllComment(int fnbId)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<FnbComment>();
                    var count = data.Delete(d => d.FnbId == fnbId);

                    if (count == 0)
                        return "No comment to delete";
                    else
                        return $"All {count} comment deleted";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // PICTURE

        public string AddFnbPicture(int fnbId, string addedBy, string picturePath)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var coll = db.GetCollection<FnbPicture>();
                    var id = coll.Insert(new FnbPicture
                    {
                        FnbId = fnbId,
                        AddedBy = addedBy,
                        PicturePath = picturePath,
                        TimeAdded = DateTime.Now
                    });

                    return $"New picture added. Total picture: {ListAllFnbComment(fnbId).Count}";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<FnbPicture> ListAllFnbPicture(int fnbId)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<FnbPicture>();
                    return data.FindAll().Where(d => d.FnbId == fnbId).ToList();
                }
            }
            catch
            {
                return new List<FnbPicture>();
            }
        }

        // User

        public User GetUserById(int id)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var coll = db.GetCollection<User>();
                    return coll.FindById(id);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public User GetUserByUsername(string username)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var coll = db.GetCollection<User>();
                    return coll.FindOne(d => d.Username.ToLower() == username.ToLower());
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool IsUsernameExist(string username)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var coll = db.GetCollection<User>();
                    return coll.FindOne(d => d.Username.ToLower() == username.ToLower()) != null;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string AddUser(string username, string passwordHash, string email)
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<User>();
                    var id = data.Insert(new User
                    {
                        Username = username,
                        PasswordHash = passwordHash,
                        Email = email,
                        CreatedTime = DateTime.Now
                    });
                    return $"'{username}' added with ID '{id.AsString}'";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<User> ListAllUser()
        {
            try
            {
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<User>();
                    return data.FindAll().ToList();
                }
            }
            catch
            {
                return new List<User>();
            }
        }

        public (bool, string) ChangePassword(int userId, string newPasswordHash)
        {
            try
            {
                var user = GetUserById(userId);
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<User>();
                    var succ = data.Update(userId, new User
                    {
                        PasswordHash = newPasswordHash,

                        Username = user.Username,
                        Avatar = user.Avatar,
                        CreatedTime = user.CreatedTime,
                        Email = user.Email
                    });
                    return (true, $"Password for '{user.Username}' changed");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public (bool, string) UpdateUser(int userId, string email, string avatar)
        {
            try
            {
                var user = GetUserById(userId);
                using (var db = new LiteDatabase(conString))
                {
                    var data = db.GetCollection<User>();
                    var succ = data.Update(userId, new User
                    {
                        Avatar = avatar,
                        Email = email,

                        Username = user.Username,
                        PasswordHash = user.PasswordHash,
                        CreatedTime = user.CreatedTime,
                    });
                    return (true, $"User '{user.Username}' successfully updated");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
