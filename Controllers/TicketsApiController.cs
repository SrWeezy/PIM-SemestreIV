using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIMIV.Data;
using PIMIV.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PIMIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _openAiApiKey;

        public TicketsApiController(ApplicationDbContext context, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _openAiApiKey = config["OpenAI:ApiKey"] ?? "";
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Usuario)
                .OrderByDescending(t => t.DataAbertura)
                .ToListAsync();

            return Ok(tickets);
        }

        [HttpPost("abrir")]
        public async Task<IActionResult> AbrirTicket([FromBody] Ticket ticket)
        {
            if (ticket == null) return BadRequest("Ticket inválido.");

            ticket.Usuario = null;
            if (ticket.UsuarioId.HasValue)
            {
                var existe = await _context.Usuarios.AnyAsync(u => u.Id == ticket.UsuarioId.Value);
                if (!existe) return BadRequest("UsuarioId inválido — usuário não encontrado.");
            }

            ticket.DataAbertura = DateTime.UtcNow;
            _context.Tickets.Add(ticket);

            var respostaIa = await ObterRespostaIA(ticket.Titulo, ticket.Descricao);

            ticket.RespostaIA = respostaIa;
            ticket.Status = TicketStatus.Fechado;
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Ticket aberto com sucesso!", Ticket = ticket, RespostaIA = respostaIa });
        }

        [HttpPost("{id}/responder")]
        public async Task<IActionResult> ResponderComIA(int id)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound("Ticket não encontrado.");

            var respostaIa = await ObterRespostaIA(ticket.Titulo, ticket.Descricao);
            ticket.RespostaIA = respostaIa;
            ticket.Status = TicketStatus.Fechado;
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Resposta da IA atualizada com sucesso!", TicketId = ticket.Id, RespostaIA = respostaIa });
        }

        private async Task<string> ObterRespostaIA(string titulo, string descricao)
        {
            if (string.IsNullOrWhiteSpace(_openAiApiKey))
                return "Chave da OpenAI ausente. Configure OpenAI:ApiKey no appsettings.";

            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

                // Use um modelo ativo (gpt-4o-mini é leve e atual)
                var body = new
                {
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = "Você é um suporte técnico especializado. Responda de forma clara, objetiva e prática, com passos." },
                        new { role = "user", content = $"Título: {titulo}\nDescrição: {descricao}" }
                    },
                    max_tokens = 300
                };

                var json = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", json);

                var payload = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Retorna motivo técnico para você depurar (em produção, logar e retornar msg genérica)
                    return $"Falha na OpenAI ({(int)response.StatusCode}): {payload}";
                }

                using var doc = JsonDocument.Parse(payload);
                var content = doc.RootElement
                                 .GetProperty("choices")[0]
                                 .GetProperty("message")
                                 .GetProperty("content")
                                 .GetString();

                return content ?? "Resposta vazia da IA.";
            }
            catch (TaskCanceledException)
            {
                return "Timeout ao consultar a OpenAI (verifique internet/firewall).";
            }
            catch (Exception ex)
            {
                return $"Erro ao consultar a OpenAI: {ex.Message}";
            }
        }
    }
}
