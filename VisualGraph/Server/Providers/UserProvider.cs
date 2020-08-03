using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VisualGraph.Server.Shared;
using VisualGraph.Shared.Models;

namespace VisualGraph.Server.Providers
{
    /// <summary>
    /// Verwaltet xml- Benutzerdateien
    /// </summary>
    public class UserProvider
    {
        private static Random random = new Random();
        private static string userdir;
        /// <summary>
        /// Erstellt Instanz der Klasse und das Benutzer- Verzeichnis
        /// </summary>
        /// <param name="userdir">Verzeichnis in welchem die Anwender gespeichert werden sollen</param>
        public UserProvider(string userdir = "users")
        {
            UserProvider.userdir = userdir;
            if (!Directory.Exists(userdir))
            {
                Directory.CreateDirectory(userdir);
            }
        }
        /// <summary>
        /// Erstellt eine Zufällige String ID
        /// </summary>
        /// <param name="length">Länge der Zeichenfolge</param>
        /// <returns>Zufälliger String</returns>
        public static string RandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        /// <summary>
        /// Sucht nach Benutzern
        /// </summary>
        /// <param name="username">zu findender Benutzername</param>
        /// <returns>gefundenen User</returns>
        public async Task<UserModel> FindUser(string username)
        {
            UserModel user = null;
            foreach (var file in Directory.EnumerateFiles(userdir))
            {
                var json = await File.ReadAllTextAsync(file);
                var _user = JsonSerializer.Deserialize<UserModel>(json); 
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
            var users = await GetUserFilenames();

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
                var _user = JsonSerializer.Deserialize<UserModel>(json);
                if (_user.Username == username)
                {
                    user = file;
                    break;
                }
            }
            return user;
        }
        /// <summary>
        /// Erstellt neue Benutzerdatei
        /// </summary>
        /// <param name="userModel">Model des zuerstellenden Users</param>
        /// <returns>Model des erstellten Users</returns>
        public async Task<UserModel> RegisterUser(UserModel userModel)
        {
            var users = await GetUserFilenames();

            if (!users.Contains(userModel.Username))
            {
                userModel.Roles.Add(VGAppSettings.MemberRole);
                userModel.Id = RandomString();
                var json = JsonSerializer.Serialize(userModel);
                await File.WriteAllTextAsync(userdir + "/" + userModel.Id + ".json", json);

            }
            return userModel;
        }

        internal async Task UpdateUser(UserModel user)
        {
            user.Roles = user.Roles.Distinct().ToList();
            var json = JsonSerializer.Serialize(user);
            await File.WriteAllTextAsync(userdir + "/" + user.Id + ".json", json);
        }
        /// <summary>
        /// Ändert das Passwort
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="password"></param>
        /// <returns>Eingegebener User</returns>
        public async Task<UserModel> ChangePassword(UserModel userModel, string password)
        {
            var users = await GetUserFilenames();

            if (users.Contains(userModel.Username))
            {
                var file = await FindUserFile(userModel.Username);
                PasswordHasher<UserModel> passwordHasher = new PasswordHasher<UserModel>();
                userModel.Password = passwordHasher.HashPassword(userModel, password);
                var json = JsonSerializer.Serialize(userModel);
                await File.WriteAllTextAsync(file + ".json", json);
            }
            return userModel;
        }

        private Task<List<string>> GetUserFilenames()
        {
            return Task.FromResult(Directory.EnumerateFiles(userdir).ToList());
        }
        /// <summary>
        /// Frägt alle Usernamen ab
        /// </summary>
        /// <returns>Liste aller Usernames</returns>
        public async Task<List<string>> GetUsernames()
        {
            List<string> usernames = new List<string>();
            foreach (var file in Directory.EnumerateFiles(userdir))
            {
                var json = await File.ReadAllTextAsync(file);
                var _user = JsonSerializer.Deserialize<UserModel>(json);
                usernames.Add(_user.Username);
            }
            return usernames;
        }
    }
}
