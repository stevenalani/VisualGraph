using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class UserModel
    {
        public string Id { get; set; } = "";
        public string Firstname { get; set; } = "";
        public string Name { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        [JsonIgnore]
        public string ErrorMessage { get; set; } = string.Empty;
        [JsonIgnore]
        public bool HasError => ErrorMessage != string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsAuth { get; set; } = false;

    }
}
