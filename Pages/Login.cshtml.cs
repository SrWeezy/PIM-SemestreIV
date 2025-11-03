using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

        [TempData]
        public string? MensagemErroLogin { get; set; }
        [TempData]
        public string? EmailTentativa { get; set; }

        public async Task<IActionResult> OnGetAsync(bool logout = false, bool trocar = false)
        {
            if (logout || trocar)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                MensagemSucesso = trocar
                    ? "Você pode entrar com outra conta agora."
                    : "Você saiu da sua conta.";
            }

            if (!string.IsNullOrEmpty(MensagemErroLogin))
            {
                Mensagem = MensagemErroLogin;
                MensagemErroLogin = null;
            }

            if (!string.IsNullOrEmpty(EmailTentativa))
            {
                Email = EmailTentativa;
                EmailTentativa = null;
            }

            if (User?.Identity?.IsAuthenticated == true && !logout && !trocar)
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
                MensagemErroLogin = "Email ou senha incorretos.";
                EmailTentativa = Email;
                return RedirectToPage();
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

        public async Task<IActionResult> OnPostCadastrarAsync()
        {
            ActiveTab = "cadastro";

            var email = (EmailCadastro ?? string.Empty).Trim();
            var senha = (SenhaCadastro ?? string.Empty).Trim();
            var nome = (NomeCadastro ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                MensagemCadastro = "Informe um e-mail e uma senha.";
                return Page();
            }

            var jaExiste = _context.Usuarios.Any(u => u.Email == email);
            if (jaExiste)
            {
                MensagemCadastro = "Já existe um usuário com este e-mail.";
                return Page();
            }

            var novo = new Usuario
            {
                Nome = nome,
                Email = email,
                Senha = senha
            };

            _context.Usuarios.Add(novo);
            await _context.SaveChangesAsync();

            SucessoCadastro = "Conta criada com sucesso. Faça login.";
            ActiveTab = "login";
            Email = email;

            return Page();
        }
    }
}
