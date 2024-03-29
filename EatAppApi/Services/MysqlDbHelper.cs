﻿using EatAppApi.Common;
using EatAppApi.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Services
{
    public class MysqlDbHelper : IMysqlDbHelper
    {
        private readonly string dbConString;
        private readonly IConfiguration config;
        private readonly ITimezoneHelper timezoneHelper;

        public MysqlDbHelper(IConfiguration config, ITimezoneHelper timezoneHelper)
        {
            this.config = config;
            this.timezoneHelper = timezoneHelper;
            dbConString = config["Db:MySqlConnectionString_CloudSql"];
        }

        private static object GetObjectValue(object obj)
        {
            if (obj == DBNull.Value) return null;
            else return obj;
        }

        private static string GetStringValue(object obj)
        {
            if (obj == DBNull.Value) return null;
            else return obj.ToString();
        }

        private static byte[] GetByteArrayValue(object obj)
        {
            if (obj == DBNull.Value) return null;
            else return (byte[])obj;
        }

        private static DateTime? GetDateTimeValue(object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToDateTime(obj);
        }

        #region User

        public async Task<UserProfile> GetUserByIdAsync(int id)
        {
            try
            {
                UserProfile user = null;
                string sql = "SELECT * FROM user WHERE id = @id;";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                user = new UserProfile
                                {
                                    Id = int.Parse(GetStringValue(reader["id"])),
                                    Username = GetStringValue(reader["username"]),
                                    Fullname = GetStringValue(reader["fullname"]),
                                    Email = GetStringValue(reader["email"]),
                                    Avatar = GetStringValue(reader["avatar"]),
                                    Role = (UserRole)int.Parse(GetStringValue(reader["role"])),
                                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value.ToLocalTime(),
                                    LastLoginTime = GetLastLoginTime(GetDateTimeValue(reader["last_login_time"]))
                                };
                            }
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                //LOGGER.LogDbException(ex);
                return null;
            }
        }

        public async Task<UserProfile> GetUserByUsernameAsync(string username)
        {
            try
            {
                UserProfile user = null;
                string sql = "SELECT * FROM user WHERE username = @username;";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                user = new UserProfile
                                {
                                    Id = int.Parse(GetStringValue(reader["id"])),
                                    Username = GetStringValue(reader["username"]),
                                    Fullname = GetStringValue(reader["fullname"]),
                                    Email = GetStringValue(reader["email"]),
                                    Avatar = GetStringValue(reader["avatar"]),
                                    Role = (UserRole)int.Parse(GetStringValue(reader["role"])),
                                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value.ToLocalTime(),
                                    LastLoginTime = GetLastLoginTime(GetDateTimeValue(reader["last_login_time"]))
                                };
                            }
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                //LOGGER.LogDbException(ex);
                return null;
            }
        }

        public async Task<UserAuth> GetUserAuthAsync(string username)
        {
            try
            {
                UserAuth user = null;
                string sql = "SELECT id, password_salt, password_hash FROM user WHERE username = @username;";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                user = new UserAuth
                                {
                                    UserId = int.Parse(GetStringValue(reader[0])),
                                    Username = username,
                                    PasswordSalt = GetStringValue(reader[1]),
                                    PasswordHash = GetStringValue(reader[2])
                                };
                            }
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                //LOGGER.LogDbException(ex);
                return null;
            }
        }

        public async Task<DbCommitResponse> UpdateLoginTimeAsync(int userId)
        {
            try
            {
                DbCommitResponse resp = null;
                string query =
                    "UPDATE user SET last_login_time = NOW() WHERE id = @id;";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        await cmd.ExecuteNonQueryAsync();

                        resp = new DbCommitResponse
                        {
                            IsSuccess = true,
                            Message = $"Last login time updated for user ID '{userId}'"
                        };
                    }
                }
                return resp;
            }
            catch (Exception ex) { return new DbCommitResponse { IsSuccess = false, Message = ex.Message }; }
        }

        public async Task<bool> IsUsernameExistAsync(string username)
        {
            try
            {
                string sql = $"SELECT COUNT(*) FROM user WHERE username = '{username}'";
                int count = 0;

                using (MySqlConnection connection = new MySqlConnection(dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                count = int.Parse(reader[0].ToString());
                                break;
                            }
                        }
                    }
                }

                return count > 0 ? true : false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<DbCommitResponse> AddUserAsync(string username, string passwordHash, string passwordSalt, string email, UserRole role)
        {
            try
            {
                DbCommitResponse resp = null;
                string query =
                    "INSERT into user (username, password_hash, password_salt, email, role) VALUES " +
                    "(@a, @b, @c, @d, @e);";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@a", username);
                        cmd.Parameters.AddWithValue("@b", passwordHash);
                        cmd.Parameters.AddWithValue("@c", passwordSalt);
                        cmd.Parameters.AddWithValue("@d", email);
                        cmd.Parameters.AddWithValue("@e", role);

                        resp = new DbCommitResponse
                        {
                            IsSuccess = true,
                            Message = $"{await cmd.ExecuteNonQueryAsync()} record successfully created"
                        };
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                return new DbCommitResponse
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<List<UserProfile>> ListAllUserAsync()
        {
            try
            {
                List<UserProfile> userList = new List<UserProfile>();
                string sql = "SELECT * FROM user;";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var user = new UserProfile
                                {
                                    Id = int.Parse(GetStringValue(reader["id"])),
                                    Username = GetStringValue(reader["username"]),
                                    Fullname = GetStringValue(reader["fullname"]),
                                    Email = GetStringValue(reader["email"]),
                                    Avatar = GetStringValue(reader["avatar"]),
                                    Role = (UserRole)int.Parse(GetStringValue(reader["role"])),
                                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value.ToLocalTime(),
                                    LastLoginTime = GetLastLoginTime(GetDateTimeValue(reader["last_login_time"]))
                                };

                                userList.Add(user);
                            }
                        }
                    }
                }

                return userList;
            }
            catch (Exception ex)
            {
                //LOGGER.LogDbException(ex);
                return new List<UserProfile>();
            }
        }

        public async Task<DbCommitResponse> ChangePasswordAsync(int userId, string newPasswordHash)
        {
            try
            {
                DbCommitResponse resp = null;
                string query =
                    "UPDATE user SET password_hash = @password WHERE id = @id;";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@password", newPasswordHash);
                        cmd.Parameters.AddWithValue("@id", userId);

                        resp = new DbCommitResponse
                        {
                            IsSuccess = true,
                            Message = $"{await cmd.ExecuteNonQueryAsync()} record updated (password)"
                        };
                    }
                }
                return resp;
            }
            catch (Exception ex) { return new DbCommitResponse { IsSuccess = false, Message = ex.Message }; }
        }

        public async Task<DbCommitResponse> UpdateUserAsync(int userId, string email, string fullname, string avatar)
        {
            try
            {
                DbCommitResponse resp = null;
                string query =
                    "UPDATE user SET email = @email, fullname = @fullname, avatar = @avatar WHERE id = @id;";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@fullname", fullname);
                        cmd.Parameters.AddWithValue("@avatar", avatar);
                        cmd.Parameters.AddWithValue("@id", userId);
                        await cmd.ExecuteNonQueryAsync();

                        resp = new DbCommitResponse
                        {
                            IsSuccess = true,
                            Message = "User updated"
                        };
                    }
                }
                return resp;
            }
            catch (Exception ex) { return new DbCommitResponse { IsSuccess = false, Message = ex.Message }; }
        }

        private DateTime? GetLastLoginTime(DateTime? lastLoginTime)
        {
            if (lastLoginTime.HasValue)
                return lastLoginTime.Value.ToLocalTime();
            else return null;
        }

        #endregion

        #region Fnb

        public async Task<List<Fnb>> ListAllFnbAsync()
        {
            try
            {
                List<Fnb> fnbList = new List<Fnb>();
                string sql = "SELECT * FROM fnb;";

                using (MySqlConnection connection = new MySqlConnection(dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var fnb = new Fnb
                                {
                                    Id = int.Parse(GetStringValue(reader["id"])),
                                    Name = GetStringValue(reader["name"]),
                                    FnbType = (FnbType)int.Parse(GetStringValue(reader["fnb_type"])),
                                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value.ToLocalTime(),
                                };

                                fnbList.Add(fnb);
                            }
                        }
                    }
                }

                return fnbList;
            }
            catch (Exception ex)
            {
                //LOGGER.LogDbException(ex);
                return new List<Fnb>();
            }
        }

        public async Task<Fnb> GetFnbByNameAsync(string name, bool exact = false)
        {
            try
            {
                Fnb fnb = null;

                string sql = $"SELECT * FROM fnb WHERE {(exact ? $"name = '{name}'" : $"name LIKE '%{name}%'")};";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        //cmd.Parameters.AddWithValue("@name", name);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                fnb = new Fnb
                                {
                                    Id = int.Parse(GetStringValue(reader["id"])),
                                    Name = GetStringValue(reader["name"]),
                                    FnbType = (FnbType)int.Parse(GetStringValue(reader["fnb_type"])),
                                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value.ToLocalTime(),
                                };
                            }
                        }
                    }
                }

                return fnb;
            }
            catch (Exception ex)
            {
                //LOGGER.LogDbException(ex);
                return null;
            }
        }

        public async Task<Fnb> GetFnbByIdAsync(int id)
        {
            try
            {
                Fnb fnb = null;

                string sql = $"SELECT * FROM fnb WHERE id = @id;";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                fnb = new Fnb
                                {
                                    Id = int.Parse(GetStringValue(reader["id"])),
                                    Name = GetStringValue(reader["name"]),
                                    FnbType = (FnbType)int.Parse(GetStringValue(reader["fnb_type"])),
                                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value.ToLocalTime(),
                                };
                            }
                        }
                    }
                }

                return fnb;
            }
            catch (Exception ex)
            {
                //LOGGER.LogDbException(ex);
                return null;
            }
        }

        public async Task<DbCommitResponse> AddFnbAsync(Fnb fnb)
        {
            try
            {
                DbCommitResponse resp = null;
                string query =
                    "INSERT into fnb (name, fnb_type) VALUES " +
                    "(@a, @b);";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@a", fnb.Name);
                        cmd.Parameters.AddWithValue("@b", fnb.FnbType);


                        resp = new DbCommitResponse
                        {
                            IsSuccess = true,
                            Message = $"{await cmd.ExecuteNonQueryAsync()} record successfully created"
                        };
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                return new DbCommitResponse
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbCommitResponse> AddBatchFnbAsync(List<Fnb> fnbList)
        {
            int succCount = 0;
            foreach (var fnb in fnbList)
            {
                var r = await AddFnbAsync(fnb);
                succCount += r.IsSuccess ? 1 : 0;
            }

            return new DbCommitResponse
            {
                IsSuccess = succCount == fnbList.Count ? true : false,
                Message = $"{succCount} record successfully created"
            };
        }


        public async Task<bool> IsFnbExistAsync(string name)
        {
            try
            {
                string sql = $"SELECT COUNT(*) FROM fnb WHERE name = '{name}'";
                int count = 0;

                using (MySqlConnection connection = new MySqlConnection(dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                count = int.Parse(reader[0].ToString());
                                break;
                            }
                        }
                    }
                }

                return count > 0 ? true : false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region Fnb comment

        public async Task<DbCommitResponse> AddFnbCommentAsync(int fnbId, int commenterId, string comment, int rating, BaseRating baseRating)
        {
            try
            {
                DbCommitResponse resp = null;
                string query =
                    "INSERT into fnb_comment (fnb_id, commenter_id, comment, rating, base_rating) VALUES " +
                    "(@a, @b, @c, @d, @e);";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@a", fnbId);
                        cmd.Parameters.AddWithValue("@b", commenterId);
                        cmd.Parameters.AddWithValue("@c", comment);
                        cmd.Parameters.AddWithValue("@d", rating);
                        cmd.Parameters.AddWithValue("@e", baseRating);
                        await cmd.ExecuteNonQueryAsync();

                        resp = new DbCommitResponse
                        {
                            IsSuccess = true,
                            Message = $"Comment successfully submitted"
                        };
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                return new DbCommitResponse
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<List<FnbComment>> ListAllFnbCommentAsync(int fnbId)
        {
            try
            {
                List<FnbComment> fnbCommentList = new List<FnbComment>();
                string sql = $"SELECT * FROM fnb_comment WHERE fnb_id = '{fnbId}';";

                using (MySqlConnection connection = new MySqlConnection(dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var fnbComment = new FnbComment
                                {
                                    Id = int.Parse(GetStringValue(reader["id"])),
                                    FnbId = int.Parse(GetStringValue(reader["fnb_id"])),
                                    CommenterId = int.Parse(GetStringValue(reader["commenter_id"])),
                                    Comment = GetStringValue(reader["comment"]),
                                    Rating = int.Parse(GetStringValue(reader["rating"])),
                                    BaseRating = (BaseRating)int.Parse(GetStringValue(reader["base_rating"])),
                                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value.ToLocalTime(),
                                };

                                fnbCommentList.Add(fnbComment);
                            }
                        }
                    }
                }

                return fnbCommentList;
            }
            catch (Exception ex)
            {
                //LOGGER.LogDbException(ex);
                return new List<FnbComment>();
            }
        }

        public async Task<int> CountAllFnbCommentAsync(int fnbId)
        {
            try
            {
                int count = 0;
                string sql = $"SELECT COUNT(*) FROM fnb_comment WHERE fnb_id = '{fnbId}';";

                using (MySqlConnection connection = new MySqlConnection(dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                count = int.Parse(GetStringValue(reader[0]));
                            }
                        }
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                //LOGGER.LogDbException(ex);
                return 0;
            }
        }


        public async Task<DbCommitResponse> DeleteAllCommentAsync(int fnbId)
        {
            try
            {
                DbCommitResponse resp = null;
                string query =
                    "DELETE FROM fnb_comment WHERE fnb_id = @id;";

                using (MySqlConnection connection = new MySqlConnection(this.dbConString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", fnbId);
                        await cmd.ExecuteNonQueryAsync();

                        resp = new DbCommitResponse
                        {
                            IsSuccess = true,
                            Message = $"All comment deleted"
                        };
                    }
                }
                return resp;
            }
            catch (Exception ex)
            {
                return new DbCommitResponse
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        #endregion
    }
}
