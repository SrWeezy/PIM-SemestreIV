﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIMIV.Data;
using PIMIV.Models;
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

        public async Task OnGetAsync()
        {
            ListaTickets = await _db.Tickets
                .Include(t => t.Usuario)
                .OrderByDescending(t => t.DataAbertura)
                .ToListAsync();
        }
    }
}
