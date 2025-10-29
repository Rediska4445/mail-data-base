using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace mailRu
{
    internal class SqlConnector
    {
        public string connectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;";
        private SqlConnection connection;

        public Action<SqlConnection> onOpen;
        public Action<SqlConnection> onClose;
        public Action<SqlConnection> onPush;
        public Action<SqlConnection> onRead;

        public SqlConnector()
        {
            connection = new SqlConnection(connectionString);
        }

        public SqlConnector(string conn)
        {
            connection = new SqlConnection(connectionString = conn);
        }

        public DataTable Fill(string sql)
        {
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

            adapter.Fill(table);

            return table;
        }

        public void Open()
        {
            if(connection.State != ConnectionState.Open)
                connection.Open();

            if(onOpen != null) 
                onOpen(connection);
        }

        public void Close()
        {
            if(connection.State != ConnectionState.Closed)
                connection.Close();

            if (onClose != null)
                onClose(connection);
        }

        public void Read(string sql, Action<SqlDataReader> processRow)
        {
            if (connection.State != ConnectionState.Open)
            {
                Open();
            }

            using (SqlCommand command = new SqlCommand(sql, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    processRow(reader);
                }
            }

            if (onRead != null)
                onRead(connection);
        }

        public void Push(string sql, Action<SqlCommand> configureCommand)
        {
            if (connection.State != ConnectionState.Open)
            {
                Open();
            }

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                if(configureCommand != null) 
                    configureCommand(command);

                command.ExecuteNonQuery();
            }

            if (onPush != null)
                onPush(connection);
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }
    }
}
