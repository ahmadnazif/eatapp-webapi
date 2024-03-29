﻿using EatAppApi.Common;
using EatAppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Services
{
    public interface IMysqlDbHelper
    {
        // User
        Task<UserProfile> GetUserByIdAsync(int id);
        Task<UserProfile> GetUserByUsernameAsync(string username);
        Task<UserAuth> GetUserAuthAsync(string username);
        Task<DbCommitResponse> UpdateLoginTimeAsync(int userId);
        Task<bool> IsUsernameExistAsync(string username);
        Task<DbCommitResponse> AddUserAsync(string username, string passwordHash, string passwordSalt, string email, UserRole role);
        Task<List<UserProfile>> ListAllUserAsync();
        Task<DbCommitResponse> ChangePasswordAsync(int userId, string newPasswordHash);
        Task<DbCommitResponse> UpdateUserAsync(int userId, string email, string fullname, string avatar);

        // Fnb
        Task<List<Fnb>> ListAllFnbAsync();
        Task<Fnb> GetFnbByNameAsync(string name, bool exact = false);
        Task<Fnb> GetFnbByIdAsync(int id);
        Task<DbCommitResponse> AddFnbAsync(Fnb fnb);
        Task<DbCommitResponse> AddBatchFnbAsync(List<Fnb> fnbList);
        Task<bool> IsFnbExistAsync(string name);

        // Fnb comment
        Task<DbCommitResponse> AddFnbCommentAsync(int fnbId, int commenterId, string comment, int rating, BaseRating baseRating);
        Task<List<FnbComment>> ListAllFnbCommentAsync(int fnbId);
        Task<int> CountAllFnbCommentAsync(int fnbId);
        Task<DbCommitResponse> DeleteAllCommentAsync(int fnbId);


    }
}
