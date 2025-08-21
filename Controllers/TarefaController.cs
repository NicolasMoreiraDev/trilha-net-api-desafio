using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            // Busca a tarefa pelo ID fornecido. Find é otimizado para busca por chave primária.
            var tarefa = _context.Tarefas.Find(id);

            // Se a tarefa não for encontrada (null), retorna um 404 Not Found.
            if (tarefa == null)
                return NotFound();

            // Se encontrou, retorna 200 OK com os dados da tarefa.
            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            // Busca todas as tarefas da tabela e as converte para uma lista.
            var tarefas = _context.Tarefas.ToList();
            return Ok(tarefas);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            // Usa o Where para filtrar tarefas cujo título contenha a string recebida.
            // O .ToList() executa a consulta no banco de dados.
            var tarefas = _context.Tarefas.Where(x => x.Titulo.Contains(titulo)).ToList();
            return Ok(tarefas);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            // Filtra as tarefas pela data, ignorando a parte de hora/minuto/segundo.
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date).ToList();
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            // Filtra as tarefas pelo status (Pendente ou Finalizado).
            var tarefa = _context.Tarefas.Where(x => x.Status == status).ToList();
            return Ok(tarefa);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Adiciona a nova tarefa ao contexto do EF.
            _context.Add(tarefa);
            // Salva as mudanças no banco de dados, o que insere o novo registro.
            _context.SaveChanges();

            // Retorna 201 Created com a localização do novo recurso e o objeto criado.
            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Atualiza as propriedades do objeto que foi buscado no banco.
            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            // Marca o objeto como modificado no contexto do EF.
            _context.Update(tarefaBanco);
            // Salva as mudanças no banco de dados.
            _context.SaveChanges();

            // Retorna 200 OK com os dados atualizados.
            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            // Remove o objeto do contexto do EF.
            _context.Remove(tarefaBanco);
            // Salva as mudanças, o que deleta o registro no banco.
            _context.SaveChanges();
            
            // Retorna 204 No Content, indicando sucesso na remoção sem corpo de resposta.
            return NoContent();
        }
    }
}