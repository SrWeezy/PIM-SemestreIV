using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIMIV.Data;
using PIMIV.Models;
using System.Linq;

namespace PIMIV.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public LoginModel(ApplicationDbContext context) { _context = context; }

        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Senha { get; set; } = string.Empty;
        [BindProperty] public string NomeCadastro { get; set; } = string.Empty;
        [BindProperty] public string EmailCadastro { get; set; } = string.Empty;
        [BindProperty] public string SenhaCadastro { get; set; } = string.Empty;

        public string Mensagem { get; set; } = "";
        public string MensagemCadastro { get; set; } = "";
        public string SucessoCadastro { get; set; } = "";
        public string ActiveTab { get; set; } = "login";

        public IActionResult OnGet()
        {
            if (!_context.Usuarios.Any())
                return RedirectToPage("/Usuarios/Create");
            return Page();
        }

        public IActionResult OnPostEntrar()
        {
            ActiveTab = "login";
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == Email && u.Senha == Senha);
            if (usuario == null)
            {
                Mensagem = "Usuário ou senha incorretos!";
                return Page();
            }
            return RedirectToPage("/Tickets/Index");
        }

        public IActionResult OnPostCadastrar()
        {
            ActiveTab = "cadastro";
            var jaExiste = _context.Usuarios.Any(u => u.Email == EmailCadastro);
            if (jaExiste)
            {
                MensagemCadastro = "Já existe um usuário com este email.";
                return Page();
            }
            var novo = new Usuario
            {
                Nome = NomeCadastro,
                Email = EmailCadastro,
                Senha = SenhaCadastro
            };
            _context.Usuarios.Add(novo);
            _context.SaveChanges();
            SucessoCadastro = "Cadastro realizado. Agora você pode entrar.";
            ActiveTab = "login";
            Email = EmailCadastro;
            return Page();
        }
    }
}
