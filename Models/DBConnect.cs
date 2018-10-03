using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Neat.Procedure;

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
            database = "projeto_de_sistemas_embarcados";
            uid = "root";
            password = "";
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
        public bool Insert(string nome, string ip)
        {
            bool result = false;
            string query;

            query = "INSERT INTO modulo (nome,ip) VALUES('" + nome + "','" + ip + "'" + ")";

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
        public bool UpdatePotencia(int id, int potencia)
        {
            string query = "UPDATE modulo SET potencia=" + potencia + " WHERE idmodulo=" + id;
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

        //Select statement
        public Modulo Select()
        {
            string query = "select * from modulo";

            //Create a list to store the result

            Modulo modulo = new Modulo();
            List<No> lista = new List<No>();
            List<Ligacao> ligacao = new List<Ligacao>();
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

                    No auxiliarNo = new No();
                    Ligacao auxiliarLigacao = new Ligacao();
                    auxiliarNo.id = Convert.ToInt32(dataReader["idmodulo"]);
                    auxiliarNo.label = Convert.ToString(dataReader["nome"]);

                    auxiliarNo.attributes=Convert.ToString(dataReader["estado"])=="1"? "Ligado" : "Desligado";
                    //if (!dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                    //{
                    //    auxiliarLigacao.from = Convert.ToInt32(dataReader["origem_ligacao"]);
                    //    auxiliarLigacao.to = Convert.ToInt32(dataReader["destino_ligacao"]);
                    //}

                    lista.Add(auxiliarNo);
                    //ligacao.Add(auxiliarLigacao);
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
       
            string query = "select * from consumo where idmodulo="+ idmodulo;

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
                    

                  

                    consumos.Add(new Consumo() { valor= Convert.ToInt32(dataReader["valor"]), data = Convert.ToString(dataReader["data"]) });
                    //ligacao.Add(auxiliarLigacao);
                }
           

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
               
            }
           

            
           
        }

    }
}
