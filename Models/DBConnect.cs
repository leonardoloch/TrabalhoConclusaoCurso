using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using Neat.Procedure;
using WebApplication1.Controllers;

namespace WebApplication1.Models
{
    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "trabalhoconclusaocurso";
            uid = "root";
            password = "root";
            string connectionString;
            connectionString = "Database=" +
            database + ";" + "Server=" + server + ";" + "Uid=" + uid + ";" + "Pwd=" + password + "; CharSet=utf8;port=3306;SslMode=none";

            //MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
            //conn_string.Server = "localhost";
            //conn_string.UserID = "root";
            //conn_string.Password = "";
            //conn_string.Database = "projeto_de_sistemas_embarcados";


            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {

            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:

                        break;

                    case 1045:

                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {

                return false;
            }
        }


        //Insert statement
        public bool Insert(string nome, string localizacao,int idUsuario,int idLocal)
        {
            bool result = false;
            string query;

            query = "INSERT INTO modulo (nome,localizacao,estado,idusuario,tipo) VALUES('" + nome + "','" + localizacao + "'" + ",0,"+idUsuario+",1);";
            query += "INSERT INTO ligacao (idmodulo,para) values ((select Max(idmodulo) from modulo),"+ idLocal + ")";
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
                result = true;
            }
            return result;
        }

        //Update statement
        public void SetConsumo(string topico, int potencia)
        {
            string[] itemValores = topico.Split('/');

            string today =  DateTime.Now.Month.ToString()+"/"+ DateTime.Now.Year.ToString();
            string query = "INSERT INTO consumo(idmodulo, data, valor) select idmodulo,STR_TO_DATE('" + today+ "' , ' %m/%Y'), " + potencia +" from modulo m join usuario u on m.idusuario = u.idusuario  where u.idusuario ="+ Convert.ToInt32(itemValores[0]) + " and m.localizacao = '"+ itemValores [1]+ "' and m.nome = '"+ itemValores [2]+ "' ON DUPLICATE KEY UPDATE consumo.valor = consumo.valor + "+potencia;
            //query= query.Replace("\", "");


            //Open connection
            if (this.OpenConnection() == true)
            {
                
                    //create mysql command
                    MySqlCommand cmd = new MySqlCommand();
                    //Assign the query using CommandText
                    cmd.CommandText = query;
                    //Assign the connection using Connection
                    cmd.Connection = connection;

                    //Execute query
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                  
                
               
            }
       
        }

        public bool UpdateNomeIp(int id, string ip, string nome)
        {
            string query = "UPDATE modulo SET ip='" + ip + "', nome='" + nome + "' WHERE idmodulo=" + id;
            bool flag = false;
            //Open connection
            if (this.OpenConnection() == true)
            {
                try
                {
                    //create mysql command
                    MySqlCommand cmd = new MySqlCommand();
                    //Assign the query using CommandText
                    cmd.CommandText = query;
                    //Assign the connection using Connection
                    cmd.Connection = connection;

                    //Execute query
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                    flag = true;
                }
                catch
                {

                }
            }
            return flag;
        }



        //Delete statement
        public bool Delete(int id)
        {
            bool result = false;
            string query = "DELETE FROM modulo WHERE idmodulo=" + id;
            try
            {
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                }
                result = true;

            }
            catch { }
            return result;
        }

        public void MudarEstado(int id, int estado)
        {

            string x = (estado == 0) ? "1" : "0";
            string query = "update modulo set estado = " + x + " where idmodulo =" + id;

            //Open connection
            if (this.OpenConnection())
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed

            }
        }




        //Select statement
        public Modulo Select(int id)
        {
            string query = "select modulo.tipo,modulo.idmodulo,modulo.nome,localizacao,estado,para from   modulo join usuario on modulo.idusuario=usuario.idusuario left join ligacao l on l.idmodulo = modulo.idmodulo where usuario.idusuario=" + id;

            //Create a list to store the result

            Modulo modulo = new Modulo();
            List<No> lista = new List<No>();
            List<Ligacao> ligacao = new List<Ligacao>();
            No auxiliarNo = new No();
            Ligacao auxiliarLigacao = new Ligacao();
            auxiliarNo.id = 0;auxiliarNo.label = "Casa";
            lista.Add(auxiliarNo);
            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {

                    auxiliarNo = new No();
                    auxiliarLigacao = new Ligacao();
                    auxiliarNo.id = Convert.ToInt32(dataReader["idmodulo"]);
                    auxiliarNo.label = Convert.ToString(dataReader["nome"]);
                    auxiliarLigacao.from = Convert.ToInt32(dataReader["idmodulo"]);
                    auxiliarNo.tipo = Convert.ToInt32(dataReader["tipo"]);
                    if (auxiliarNo.tipo == 1)
                    {
                        auxiliarLigacao.to = Convert.ToInt32(dataReader["para"]);
                        auxiliarNo.attributes = Convert.ToString(dataReader["localizacao"]);
                        auxiliarNo.title = Convert.ToInt32(dataReader["estado"]) == 1 ? "Ligado" : "Desligado";
                        if (Convert.ToInt32(dataReader["estado"]) == 1) auxiliarNo.color = "#FA5858";
                        else auxiliarNo.color = "#00FF80";
                    }
                    else
                    {
                        auxiliarNo.color = "#F2F5A9";
                        auxiliarLigacao.to = 0;

                    }                                        
                    lista.Add(auxiliarNo);
                    ligacao.Add(auxiliarLigacao);
                }
                modulo.nos = lista;
                modulo.ligacoes = ligacao;


                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return modulo;
            }
            else
            {
                return modulo;
            }
        }

        //Count statement
        public int Count()
        {
            string query = "SELECT Count(*) FROM modulo where fl_fim is null";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection();

                return Count;
            }
            else
            {
                return Count;
            }
        }

        //Backup
        public void Backup()
        {
        }

        //Restore
        public void Restore()
        {
        }

        public string getIP(int id)
        {

            string ip = "";
            string query = "select ip from modulo where idmodulo=" + id;

            //Create a list to store the result


            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {


                    ip = Convert.ToString(dataReader["ip"]);

                }


                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return ip;
            }
            else
            {
                return ip;
            }

        }

        public void getConsumo(int idmodulo, List<Consumo> consumos)
        {

            string query = "select cast(data as char) as data,valor from consumo where idmodulo=" + idmodulo;

            //Create a list to store the result


            //Open connection
            if (this.OpenConnection())
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //Read the data and store them in the list
                while (dataReader.Read())
                {




                    consumos.Add(new Consumo() { valor = Convert.ToInt32(dataReader["valor"]), data = dataReader["data"].ToString() });
                    //ligacao.Add(auxiliarLigacao);
                }


                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed

            }


        }
        public No getModulo(int id)
        {

            string query = "select nome,localizacao,estado from modulo where idmodulo=" + id;

            //Create a list to store the result
            No no= new No();
            
            //Open connection
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    //Create Command
                   
                    //Create a data reader and Execute the command
                   

                    //Read the data and store them in the list

                    no.label = Convert.ToString(dataReader["nome"]);
                    no.attributes = Convert.ToString(dataReader["localizacao"]);
                    no.title = Convert.ToString(dataReader["estado"]);
                }
                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
            }
            return no;
        }

        public int ValidarAcesso(string nome, string pass)
        {

            string query = "select idusuario as id from usuario where nome='"+nome+"' and senha='"+pass+"'";

          
            int id=0;

            //Open connection
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    
                    id = Convert.ToInt32(dataReader["id"]);                    
                }
                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
            }
            return id;
        }
        public int RegistroUsuario(string nome, string pass, string email)
        {

            string query = "insert into usuario (nome,senha,email) values ('" + nome + "','" + pass + "','" + email + "');select LAST_INSERT_ID() as id from usuario";


            int id = 0;

            //Open connection
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    id = Convert.ToInt32(dataReader["id"]);
                }
                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
            }
            return id;
        }

        public IList<SelectListItem> GetLocalizacao()
        {

            string query = "select modulo.nome ,modulo.idmodulo from modulo join usuario on usuario.idusuario=modulo.idusuario where modulo.tipo=0 and usuario.idusuario="+ HomeController.user.Id;

            IList<SelectListItem> items = new List<SelectListItem>();
            


            //Open connection
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    SelectListItem aux = new SelectListItem { Text = Convert.ToString(dataReader["nome"]),Value= Convert.ToString(dataReader["idmodulo"]) };
                    items.Add(aux);
                }
                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
            }
            return items;
        }
        public int SaveLocalizacao(string localizacao)
        {
            int id = 0;
            string query = "INSERT INTO modulo (nome,tipo,idusuario) VALUES('"+localizacao+"',0,"+HomeController.user.Id+ ");select MAX(idmodulo) as idmodulo from modulo";

            IList<SelectListItem> items = new List<SelectListItem>();



            //Open connection
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    id = Convert.ToInt32(dataReader["idmodulo"]);
                }
                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
            }
            return id;
        }
 

    }
}
