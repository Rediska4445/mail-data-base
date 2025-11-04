using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using System;
using mailRu;

namespace mailRu.Tests
{
    [TestClass]
    public class SqlConnectorServiceTests
    {
        private const string ConnectionStringMaster = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=master;";
        private const string TestDatabaseName = "TestDbForUnitTests";
        private string TestDbConnectionString => $@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database={TestDatabaseName};";

        private SqlConnector sqlConnector;
        private SqlConnectorService service;

        [TestInitialize]
        public void Setup()
        {
            EnsureDatabaseExists();
            EnsureTableExists();

            sqlConnector = new SqlConnector(TestDbConnectionString);
            service = new SqlConnectorService(sqlConnector);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Удаление тестовой базы (раскомментируйте при необходимости)
            /*
            using (SqlConnection connection = new SqlConnection(ConnectionStringMaster))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand($"ALTER DATABASE [{TestDatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", connection))
                {
                    cmd.ExecuteNonQuery();
                }
                using (SqlCommand cmdDelete = new SqlCommand($"DROP DATABASE [{TestDatabaseName}]", connection))
                {
                    cmdDelete.ExecuteNonQuery();
                }
            }
            */
        }

        private void EnsureDatabaseExists()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStringMaster))
            {
                connection.Open();

                using (SqlCommand checkCmd = new SqlCommand(
                    "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = @dbName) CREATE DATABASE [" + TestDatabaseName + "]",
                    connection))
                {
                    checkCmd.Parameters.AddWithValue("@dbName", TestDatabaseName);
                    checkCmd.ExecuteNonQuery();
                }
            }
        }

        private void EnsureTableExists()
        {
            using (SqlConnection connection = new SqlConnection(TestDbConnectionString))
            {
                connection.Open();

                string createTableSql = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'main')
                CREATE TABLE main (
                    id INT PRIMARY KEY,
                    addr NVARCHAR(255),
                    newspaper_id INT,
                    number_newspaper INT
                );
            ";

                using (SqlCommand cmd = new SqlCommand(createTableSql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [TestMethod]
        public void AddNewMail_ShouldInsertData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("addr", typeof(string));
            dt.Columns.Add("newspaper_id", typeof(int));
            dt.Columns.Add("number_newspaper", typeof(int));
            dt.Columns.Add("id", typeof(int));

            DataRow row = dt.NewRow();
            row["addr"] = "Test Address";
            row["newspaper_id"] = 1;
            row["number_newspaper"] = 100;
            row["id"] = 123;

            dt.Rows.Add(row);

            service.AddNewMail(row);

            using (SqlConnection connection = new SqlConnection(TestDbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM main WHERE id = @id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", 123);
                    int count = (int)cmd.ExecuteScalar();

                    Assert.IsGreaterThan(0, count, "Запись не была добавлена в базу.");
                }
            }
        }

        [TestMethod]
        public void SaveChanges_ShouldCallUpdateMailAndCommit()
        {
            // Подготовка данных
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("addr", typeof(string));
            dt.Columns.Add("newspaper_id", typeof(int));
            dt.Columns.Add("number_newspaper", typeof(int));

            DataRow row = dt.NewRow();
            row["id"] = 123;
            row["addr"] = "Test Address";
            row["newspaper_id"] = 1;
            row["number_newspaper"] = 100;

            dt.Rows.Add(row);
            dt.AcceptChanges();

            row.SetModified();

            service.SaveChanges(dt, service.UpdateMail);

            using (var connection = new SqlConnection(sqlConnector.connectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("SELECT addr, newspaper_id, number_newspaper FROM main WHERE id = @id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", 123);
                    using (var reader = cmd.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        Assert.AreEqual("Test Address", reader.GetString(0));
                        Assert.AreEqual(1, reader.GetInt32(1));
                        Assert.AreEqual(100, reader.GetInt32(2));
                    }
                }
            }
        }

        [TestMethod]
        public void SearchAndFillDataGridView_ShouldReturnResults()
        {
            Exception error;
            DataTable dt = service.SearchAndFillDataGridView("main", "Test", out error);

            Assert.IsNull(error);
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
        }
    }
}
