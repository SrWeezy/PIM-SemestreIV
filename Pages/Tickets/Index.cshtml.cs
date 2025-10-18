using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIMIV.Data;
using PIMIV.Models;

namespace PIMIV.Pages.Tickets
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Ticket> ListaTickets { get; set; } = new List<Ticket>();

        public async Task OnGetAsync()
        {
            ListaTickets = await _context.Tickets.Include(t => t.Usuario).ToListAsync();
        }
    }
}

