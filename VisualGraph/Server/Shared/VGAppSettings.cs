using Microsoft.Extensions.Configuration;

namespace VisualGraph.Server.Shared
{
    /// <summary>
    /// VisualGraphAppSettings Klasse. Diese kapselt alle relevanten Einstllung und 
    /// wird beim Start der Anwendung Initialisiert
    /// </summary>
    public static class VGAppSettings
    {
        /// <summary>
        /// HttpsPort auf den der Server hört
        /// </summary>
        public static int HttpsPort { get; private set; }
        /// <summary>
        /// HttpPort auf den der Server hört
        /// </summary>
        public static int HttpPort { get; private set; }
        /// <summary>
        /// Proxy falls der Server diese zum Laden von Websites benötigt 
        /// </summary>
        public static string RemoteRequestProxy { get; private set; } = "";
        /// <summary>
        /// Anwendungs- Url
        /// </summary>
        public static string BaseAddress { get; private set; } = "";
        /// <summary>
        /// Name für die Admin- Rolle
        /// </summary>
        public static string AdminRole { get; private set; } = "Administrator";
        /// <summary>
        /// Name für die SuperUser- Rolle
        /// </summary>
        public static string SuperUserRole { get; private set; } = "SuperUser";
        /// <summary>
        /// Name für die Member- Rolle
        /// </summary>
        public static string MemberRole { get; private set; } = "Member";
        /// <summary>
        /// Initialisiert die Klasse mit Werten aus der ASP .Net Konfiguration
        /// </summary>
        /// <param name="configuration"></param>
        public static void InitFromConfiguration(IConfiguration configuration)
        {            
            VGAppSettings.HttpsPort = configuration.GetValue<int>("Hosting:HttpsPort");
            VGAppSettings.HttpPort = configuration.GetValue<int>("Hosting:HttpPort");
            VGAppSettings.BaseAddress = configuration["Hosting:BaseAddress"];
            VGAppSettings.RemoteRequestProxy = configuration["Hosting:RemoteRequestProxy"];
            VGAppSettings.AdminRole = configuration["Roles:Admin"];
            VGAppSettings.SuperUserRole = configuration["Roles:SuperUser"];
            VGAppSettings.MemberRole = configuration["Roles:Member"];

        }
    }
}
