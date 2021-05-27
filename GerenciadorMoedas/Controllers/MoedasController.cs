using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GerenciadorMoedas.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoedasController : ControllerBase
    {
        public static Stack<Moeda> moedas = new Stack<Moeda>();

        // POST api/<MoedasController>
        [HttpPost("AddItemFila")]
        public async void AddItemFila([FromBody] Moeda[] moedasResponse)
        {
            foreach (Moeda m in moedasResponse) 
            {
                moedas.Push(m);
            }
        }

        [HttpGet("GetItemFila")]
        public object GetItemFila()
        {

            if (moedas.Any())
            {
                return moedas.Pop();
            }

            return "Não há moedas recém adicionadas na fila.";

        }

    }
}
