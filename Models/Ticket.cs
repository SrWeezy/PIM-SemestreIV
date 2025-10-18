using System;
using System.ComponentModel.DataAnnotations;

namespace PIMIV.Models
{
    public enum TicketStatus { Aberto = 0, EmAndamento = 1, Fechado = 2 }
    public enum TicketPrioridade { Baixa = 0, Media = 1, Alta = 2 }

    public class Ticket
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Titulo { get; set; } = string.Empty;

        [Required, StringLength(4000)]
        public string Descricao { get; set; } = string.Empty;

        [Display(Name = "Aberto em")]
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;

        [Required]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.Aberto;

        [Required]
        public TicketPrioridade Prioridade { get; set; } = TicketPrioridade.Media;
    }
}
