using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Server.Shared
{
    public static class VGAppSettings
    {
        public static string RemoteRequestProxy { get; set; } = "";
        public static string BaseAddress { get; set; } = "";
        public static string AdminRole { get; private set; } = "Administrator";
        public static string SuperUserRole { get; private set; }
        public static string MemberRole { get; set; } = "Member";

        public static void InitFromConfiguration(IConfiguration configuration)
        {
            VGAppSettings.RemoteRequestProxy = configuration["Hosting:RemoteRequestProxy"];
            VGAppSettings.AdminRole = configuration["Roles:Admin"];
            VGAppSettings.SuperUserRole = configuration["Roles:SuperUser"];
            VGAppSettings.MemberRole = configuration["Roles:Member"];
        }
    }
}
