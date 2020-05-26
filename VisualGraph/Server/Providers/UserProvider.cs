using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Shared
{
    public class UserProvider
    {
        private static Random random = new Random();
        private static string userdir;
        public UserProvider(string userdir = "users")
        {
            UserProvider.userdir = userdir;
            if (!Directory.Exists(userdir))
            {
                Directory.CreateDirectory(userdir);
            }
        }
        public static string RandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<UserModel> FindUser(string username)
        {
            UserModel user = null;
            foreach (var file in Directory.EnumerateFiles(userdir))
            {
                var json = await File.ReadAllTextAsync(file);
                var _user = JsonSerializer.Deserialize<UserModel>(json); //(typeof(UserModel));
                if (_user.Username == username)
                {
                    user = _user;
                    break;
                }
            }
            return user;
        }

        internal async Task DeleteUser(UserModel userModel)
        {
            var users = await GetUsernames();

            if (users.Contains(userModel.Username))
            {
                string filepath = await FindUserFile(userModel.Username);
                File.Delete(filepath);
            }
        }

        private async Task<string> FindUserFile(string username)
        {
            string user = "";
            foreach (var file in Directory.EnumerateFiles(userdir))
            {
                var json = await File.ReadAllTextAsync(file);
                var _user = JsonSerializer.Deserialize<UserModel>(json); //(typeof(UserModel));
                if (_user.Username == username)
                {
                    user = file;
                    break;
                }
            }
            return user;
        }

        public async Task<UserModel> RegisterUser(UserModel userModel)
        {
            var users = await GetUsernames();

            if (!users.Contains(userModel.Username))
            {
                userModel.Id = RandomString();
                var json = JsonSerializer.Serialize(userModel); //(typeof(UserModel));
                await File.WriteAllTextAsync(userdir + "/" + userModel.Id + ".json", json);

            }
            return userModel;
        }

        internal async Task UpdateUser(UserModel user)
        {
            var json = JsonSerializer.Serialize(user); //(typeof(UserModel));
            await File.WriteAllTextAsync(userdir + "/" + user.Id + ".json", json);
        }

        public async Task<UserModel> ChangePassword(UserModel userModel, string password)
        {
            var users = await GetUsernames();

            if (users.Contains(userModel.Username))
            {
                var file = await FindUserFile(userModel.Username);
                PasswordHasher<UserModel> passwordHasher = new PasswordHasher<UserModel>();
                userModel.Password = passwordHasher.HashPassword(userModel, password);
                var json = JsonSerializer.Serialize(userModel); //(typeof(UserModel));
                await File.WriteAllTextAsync(file + ".json", json);
            }
            return userModel;
        }

        private Task<List<string>> GetUsernames()
        {
            return Task.FromResult(Directory.EnumerateFiles(userdir).ToList());
        }
    }
}
