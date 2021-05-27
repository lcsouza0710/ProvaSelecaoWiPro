using GerenciadorMoedas;
using GerenciadorMoedas.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoedasService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        MoedasController controller = new MoedasController();

        private static List<MoedaRegistroCSV> listaCSV = new List<MoedaRegistroCSV>();

        public Worker(ILogger<Worker> logger, IConfiguration _conf)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            listaCSV = ListarMoedas();

            while (!stoppingToken.IsCancellationRequested)
            {
                var Response = controller.GetItemFila();

                var lista = MoedasResponse(Response);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(30000, stoppingToken);
            }
        }

        public List<MoedaRegistroCSV> ListarMoedas()
        {
            string local = Directory.GetCurrentDirectory();
            var leitor = new StreamReader(File.OpenRead(string.Format(@"{0}\{1}", local, "DadosMoeda.csv")));

            var listaMoedas = new List<MoedaRegistroCSV>();

            //Descartando os dados da primeira linha
            leitor.ReadLine();

            while (!leitor.EndOfStream)
            {
                var linha = leitor.ReadLine();
                var valores = linha.Split(';');

                var registroDeMoeda = new MoedaRegistroCSV()
                {
                    ID_Moeda = valores[0],
                    Data_Ref = Convert.ToDateTime(valores[1])
                };

                listaMoedas.Add(registroDeMoeda);
            }

            return listaMoedas;
        }

        public List<MoedaRegistroCSV> MoedasResponse(object response)
        {
            List<MoedaRegistroCSV> lista = new List<MoedaRegistroCSV>();

            try
            {
                var responseMoeda = (Moeda)response;

                lista = listaCSV.Where(m => m.ID_Moeda.Equals(responseMoeda.moeda) && (m.Data_Ref >= responseMoeda.data_inicio && m.Data_Ref <= responseMoeda.data_fim)).ToList();
            }
            catch
            {
                return null;
            }

            return lista;
        }
    }
}
