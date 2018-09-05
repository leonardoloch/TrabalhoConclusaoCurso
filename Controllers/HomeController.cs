using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.Encodings;
using System.Diagnostics;
using System.Web.Helpers;
using System.Drawing;
using Chart.Mvc.ComplexChart;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;



namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        public string resposta;
        public bool flag;
        JObject json;
        int potencia;
        string nome;
        public IActionResult Index()
        {

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            return View();
        }
        public IActionResult teste()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Descrição da Aplicação";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Meu contato";

            return View();
        }

        public IActionResult CadastroModulo()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }


        public IActionResult Historico()


        {

            
            return View();
        }


        public IActionResult Modulos()


        {

            DBConnect db = new DBConnect();
            Modulo modulo;
            modulo = db.Select();
            ViewBag.No = modulo.nos;
            return View();
        }

        [HttpGet]
        public string Save(string nome, string ip)
        {
            string result;
            DBConnect db = new DBConnect();
            if (db.Insert(nome, ip)) result = "Salvo com sucesso";
            else result = "Error ao salvar - Tente novamente";
            db = null;
            return result;
        }
        public bool Delete(int id)
        {
            bool result;
            DBConnect db = new DBConnect();
            result =db.Delete(id);
            db = null;
            return result;
        }


        public string AtualizarConsumo(int id)
        {
            DBConnect db = new DBConnect();
            List<Consumo> consumos = new List<Consumo>();
            db.getConsumo(id,consumos);
            var json = JsonConvert.SerializeObject(new
            {
                operations = consumos
            });
            /* 
            json =  JObject.([
            [new Date(2015, 0, 1), 5], [new DateTime(2015, 1, 2), 7], [new Date(2015, 2, 3), 3],
            [new Date(2015, 3, 4), 1], [new Date(2015, 4, 5), 3], [new Date(2015, 5, 6), 4],
            [new Date(2015, 6, 7), 3], [new Date(2015, 7, 8), 4], [new Date(2015, 8, 9), 2],
            [new Date(2015, 9, 10), 5], [new Date(2015, 10, 11), 8], [new Date(2015, 11, 12), 6]
        ]);*/
            return json;

        }
        
            public JObject GetHistoric(int id)
        {

            flag = true;
            DBConnect db = new DBConnect();
            string ip = db.getIP(id);
            if (ip.Equals("")) return null;
            AccessTheWebAsync(ip);
            while (flag) ;
            flag = true;
            if (resposta.Equals("")) return null;
            int potencia = json.SelectToken(@"potencia").Value<int>();
            db.UpdatePotencia(id, potencia);
            return json;

        }
        public bool AtualizarInformacao(int id,string nome,string ip)
        {
            
            DBConnect db = new DBConnect();
            return  db.UpdateNomeIp(id, ip, nome);

        }


        public async void AccessTheWebAsync(string ip)
        {
            json = null;
            resposta = "";
            try
            {
                HttpClient client = new HttpClient();            
                Task<string> getStringTask = client.GetStringAsync("http://"+ip+"");
                resposta = await getStringTask;
                json = JObject.Parse(resposta); 
                }
            catch (Exception e) { }

            flag = false;
        }

      

    }
}
