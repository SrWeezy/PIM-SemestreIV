using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIMIV.Data;
using PIMIV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIMIV.Pages.Tickets
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) { _db = db; }

        public List<Ticket> ListaTickets { get; set; } = new();
        public int TotalTickets { get; private set; }
        public int TicketsPendentes { get; private set; }
        public int TicketsFechados { get; private set; }
        public int TicketsComResposta { get; private set; }
        public int UsuariosAtendidos { get; private set; }
        public double PercentualRespondidos { get; private set; }

        public async Task OnGetAsync()
        {
            ListaTickets = await _db.Tickets
                .Include(t => t.Usuario)
                .OrderByDescending(t => t.DataAbertura)
                .ToListAsync();

            TotalTickets = ListaTickets.Count;
            TicketsComResposta = ListaTickets.Count(t => !string.IsNullOrWhiteSpace(t.RespostaIA));
            TicketsPendentes = TotalTickets - TicketsComResposta;
            TicketsFechados = ListaTickets.Count(t => t.Status == TicketStatus.Fechado);
            UsuariosAtendidos = ListaTickets
                .Where(t => t.UsuarioId.HasValue)
                .Select(t => t.UsuarioId!.Value)
                .Distinct()
                .Count();
            PercentualRespondidos = TotalTickets == 0
                ? 0
                : Math.Round((double)TicketsComResposta / TotalTickets * 100, 1);
        }
    }
}