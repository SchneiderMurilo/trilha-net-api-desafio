using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;
using Microsoft.EntityFrameworkCore;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("api/tarefas")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var tarefa = _context.Tarefas.Find(id);

            if (tarefa == null)
            {
                return NotFound();
            }

            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public async Task<IActionResult> ObterTodos()
        {
            var tarefas = await _context.Tarefas.ToListAsync();
            return Ok(tarefas);
        }

        [HttpGet("ObterPorTitulo")]
        public async Task<IActionResult> ObterPorTitulo(string titulo)
        {
            var tarefas = await _context.Tarefas
                .Where(x => x.Titulo.Contains(titulo))
                .ToListAsync();

            if (!tarefas.Any())
                return NotFound();

            return Ok(tarefas);
        }

        [HttpGet("ObterPorData")]
        public async Task<IActionResult> ObterPorData(DateTime data)
        {
            var tarefas = await _context.Tarefas
                .Where(x => x.Data.Date == data.Date)
                .ToListAsync();

            if (!tarefas.Any())
                return NotFound();

            return Ok(tarefas);
        }

        [HttpGet("ObterPorStatus")]
        public async Task<IActionResult> ObterPorStatus(EnumStatusTarefa status)
        {
            var tarefas = await _context.Tarefas
                .Where(x => x.Status == status)
                .ToListAsync();

            if (!tarefas.Any())
                return NotFound();

            return Ok(tarefas);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            await _context.Tarefas.AddAsync(tarefa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("Atualizar/{id}")]
        public async Task<IActionResult> Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = await _context.Tarefas.FindAsync(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            _context.Tarefas.Update(tarefaBanco);
            await _context.SaveChangesAsync();

            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var tarefaBanco = await _context.Tarefas.FindAsync(id);

            if (tarefaBanco == null)
                return NotFound();

            _context.Tarefas.Remove(tarefaBanco);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
