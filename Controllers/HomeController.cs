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
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        public static Usuario user=new Usuario();

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
        public IActionResult login()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Descrição da Aplicação";
            
            return View();
        }

        public IActionResult InformacaoUsuario()
        {
            ViewData["Nome"] = user.Nome;
            ViewData["codigo"] = user.Id;
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
            modulo = db.Select(user.Id);
            ViewBag.No = modulo.nos;
            ViewBag.Ligacoes = modulo.ligacoes;

            return View();
        }

        [HttpGet]
        public string Save(string nome, string localizacao_,bool novo,int idLocalizacao)
        {
            string result;
            DBConnect db = new DBConnect();
            int idLocal = idLocalizacao;
            if (novo) idLocal = db.SaveLocalizacao(localizacao_);

            MqttClient client = new MqttClient("test.mosquitto.org", 1883, false, null, null, MqttSslProtocols.None);

            // register to message received 
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;


            client.Connect(Guid.NewGuid().ToString());
            
            // subscribe to the topic "/home/temperature" with QoS 2 
            client.Subscribe(new string[] { user.Id +"/"+localizacao_ + "/"+nome}, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            
            if (db.Insert(nome, localizacao_,user.Id, idLocal))
            {

                result = "Salvo com sucesso";
            }
            else result = "Error ao salvar - Tente novamente";
            db = null;
            return result;
        }
        public int MudarEstado(int id) {
            MqttClient client = new MqttClient("test.mosquitto.org", 1883, false, null, null, MqttSslProtocols.None);

            // register to message received 
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            DBConnect db = new DBConnect();

            No no = db.getModulo(id);

            client.Connect(Guid.NewGuid().ToString());
            if (no.title == "0")
            {
                client.Publish(user.Id +"/"+no.attributes+"/"+no.label, Encoding.UTF8.GetBytes("1"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            else
            { client.Publish(user.Id + "/" + no.attributes + "/" + no.label, Encoding.UTF8.GetBytes("0"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false); }
            
            db.MudarEstado(id, Convert.ToInt32(no.title));
           
            return Convert.ToInt32(no.title);
        }
        


        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            DBConnect db = new DBConnect();
            string  teste = Encoding.UTF8.GetString(e.Message);
            db.SetConsumo(e.Topic, Convert.ToInt32(teste));
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
            

            string json = JsonConvert.SerializeObject(new
            {
       
       consumos
   
            });    
            return json;

        }
        
        public bool AtualizarInformacao(int id,string nome,string ip)
        {
            
            DBConnect db = new DBConnect();
            return  db.UpdateNomeIp(id, ip, nome);

        }
        
        [HttpGet]
        public bool ValidarAcesso(string nome,string pass)
        {
            
            DBConnect db = new DBConnect();
            int id = db.ValidarAcesso(nome, pass);
            if (id == 0) return false;   
            
            user.Nome = nome; user.Senha = pass; user.Id=id;
            return true;
        }
        [HttpGet]
        public int RegistroUsuario(string nome,string pass,string email) {

            user.Nome = nome;user.Senha = pass;user.Email = email;
            DBConnect db = new DBConnect();
            user.Id= db.RegistroUsuario(nome, pass, email);
            return user.Id;
        }
    }
}
