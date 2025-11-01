using System.Text.Json.Serialization;

namespace PIMIV.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";

        [JsonIgnore]
        public string Senha { get; set; } = "";
    }
}
