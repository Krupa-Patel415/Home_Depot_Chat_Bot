using System.Data.SQLite;
using System.IO;
using System;

namespace DB
{
    public class HomeDepotDB
    {
        public SQLiteConnection conn;
        public HomeDepotDB()
        {
            conn = new SQLiteConnection("Data Source=database.sqlite3");
            if (!File.Exists("./database.sqlite3"))
            {
                SQLiteConnection.CreateFile("database.sqlite3");
                Console.WriteLine("Database File Created");
            } else
            {
                Console.WriteLine("File already exists and connection established");
            }

        }

        public void StartConnection() {
            if(conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            } 
        }

        public void EndConnection()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
            {
                conn.Close();
            }
        }
    }
}
