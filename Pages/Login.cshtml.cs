using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIMIV.Data;
using PIMIV.Models;
using System.Linq;
using System.Security.Claims;

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
        public string MensagemSucesso { get; set; } = "";
        public string MensagemCadastro { get; set; } = "";
        public string SucessoCadastro { get; set; } = "";
        public string ActiveTab { get; set; } = "login";

        public async Task<IActionResult> OnGetAsync(bool logout = false)
        {
            if (logout)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                MensagemSucesso = "Você saiu da sua conta.";
            }

            if (User?.Identity?.IsAuthenticated == true && !logout)
            {
                return RedirectToPage("/Tickets/Index");
            }

            if (!_context.Usuarios.Any())
                return RedirectToPage("/Usuarios/Create");
            return Page();
        }

        public async Task<IActionResult> OnPostEntrarAsync()
        {
            ActiveTab = "login";
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == Email && u.Senha == Senha);
            if (usuario == null)
            {
                Mensagem = "Usuário ou senha incorretos!";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(usuario.Nome) ? usuario.Email : usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

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