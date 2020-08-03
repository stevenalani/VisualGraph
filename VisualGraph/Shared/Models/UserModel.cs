using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VisualGraph.Shared.Models
{
    /// <summary>
    /// Kapselt alle Informationen/Daten zum Benutzer
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Random zeichenkette. Wird erstellt, wenn Nuterregistriert wird
        /// </summary>
        public string Id { get; set; } = "";
        /// <summary>
        /// Vorname
        /// </summary>
        public string Firstname { get; set; } = "";
        /// <summary>
        /// Fabilienname
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// Benutzername
        /// </summary>
        public string Username { get; set; } = "";
        /// <summary>
        /// Passwort. (Controller sollen immer Models ohne Passwort zurückgeben)
        /// </summary>
        public string Password { get; set; } = "";
        /// <summary>
        /// Fehler im zusammenhang mit Benutzerprofilen. Z.B. "falsches Passwort" bei Login oder "User nicht gefunden"
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
        /// <summary>
        /// Gibt an, ob ein Fehler Vorlag
        /// </summary>
        [JsonIgnore]
        public bool HasError => ErrorMessage != string.Empty;
        /// <summary>
        /// Benutzerrollen
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
        /// <summary>
        /// gibt an ob der Benutzer angemeldet ist
        /// </summary>
        [JsonIgnore]
        public bool IsAuth { get; set; } = false;


    }
    /// <summary>
    /// Kapselt Informationen/ Daten zum bearbeiteten User. 
    /// </summary>
    public class UserUpdateModel : UserModel
    {
        /// <summary>
        /// Erstellt Instanz aus UserModel
        /// </summary>
        /// <param name="usermodel"></param>
        /// <returns></returns>
        public static UserUpdateModel FromUserModel(UserModel usermodel) => new UserUpdateModel()
        {
            ErrorMessage = usermodel.ErrorMessage,
            Firstname = usermodel.Firstname,
            Id = usermodel.Id,
            IsAuth = usermodel.IsAuth,
            Name = usermodel.Name,
            NewPassword = "",
            Password = "",
            RetypedPassword = "",
            Roles = usermodel.Roles,
            Username = usermodel.Username,
        };
        /// <summary>
        /// Neues Passwort
        /// </summary>
        public string NewPassword { get; set; } = "";
        /// <summary>
        /// Wiederhoung des neuen Passwortes
        /// </summary>
        public string RetypedPassword { get; set; } = "";
    }
}
