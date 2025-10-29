using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using SqlCommand = Microsoft.Data.SqlClient.SqlCommand;
using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;
using SqlTransaction = Microsoft.Data.SqlClient.SqlTransaction;

namespace mailRu
{
    public partial class Form1 : Form
    {
        private SqlConnector sqlConnector;

        public Form1()
        {
            InitializeComponent();

            dataGridView1.RowsAdded += dataGridView1_RowValidated;

            sqlConnector = new SqlConnector();
            sqlConnector.Open();
            sqlConnector.Push("USE Mail;", null);
        }

        private void dataGridView1_RowValidated(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var table = (DataTable)dataGridView1.DataSource;
            var changedRows = table.GetChanges();

            if (changedRows != null)
            {
                foreach (DataRow row in changedRows.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        SaveChanges((DataTable)dataGridView1.DataSource, AddNewMain);
                        SaveChanges((DataTable)dataGridView1.DataSource, AddNewNewspaper);
                        SaveChanges((DataTable)dataGridView1.DataSource, AddNewPrintingHouse);

                    }
                }
            }
        }

        private void AddNewMain(SqlConnection conn, SqlTransaction transaction, DataRow row)
        {
            string insertSql = "INSERT INTO main (id, addr, number_newspaper) VALUES (@id, @addr, @number_newspaper)";
            using (var cmd = new SqlCommand(insertSql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.Parameters.AddWithValue("@addr", row["main_addr"]);
                cmd.Parameters.AddWithValue("@number_newspaper", row["number_newspaper"]);
                cmd.ExecuteNonQuery();
            }
        }

        private void AddNewNewspaper(SqlConnection conn, SqlTransaction transaction, DataRow row)
        {
            string insertSql = "INSERT INTO newspaper (id, title) VALUES (@id, @title)";
            using (var cmd = new SqlCommand(insertSql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.Parameters.AddWithValue("@title", row["newspaper_title"]);
                cmd.ExecuteNonQuery();
            }
        }

        private void AddNewPrintingHouse(SqlConnection conn, SqlTransaction transaction, DataRow row)
        {
            string insertSql = "INSERT INTO printing_house (id, addr) VALUES (@id, @addr)";
            using (var cmd = new SqlCommand(insertSql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.Parameters.AddWithValue("@addr", row["addr"]);
                cmd.ExecuteNonQuery();
            }
        }

        private void buttosList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(buttosList.SelectedIndex)
            {
                case 0:
                    updateView("SELECT \r\n    m.id,\r\n    m.addr AS main_addr,\r\n    m.number_newspaper,\r\n    np.title AS newspaper_title\r\nFROM main m\r\nJOIN newspaper np ON m.id = np.id",
                        dataGridView1
                    );
                    break;
                case 1:
                    updateView("SELECT * FROM printing_house;",
                        dataGridView1
                    );
                    break;
                case 2:
                    updateView("SELECT\r\n    ph.id AS id,\r\n    ph.addr AS printing_house_addr,\r\n  " +
                        "  np.title AS newspaper_title,\r\n    np.edition_code,\r\n    np.price,\r\n    np.full_name,\r\n   " +
                        " np.number,\r\n    m.addr AS main_addr,\r\n    m.number_newspaper\r\nFROM\r\n    printing_house ph\r\nJOIN\r\n   " +
                        " newspaper np ON np.id = ph.id\r\nJOIN\r\n    main m ON m.id = np.id;", 
                        dataGridView1
                    );
                    break;
            }
        }

        private void SaveChanges(DataTable combinedTable, Action<SqlConnection, SqlTransaction, DataRow> updateAction)
        {
            var changedRows = combinedTable.GetChanges();
            if (changedRows == null) 
                return;

            using (SqlTransaction transaction = sqlConnector.GetConnection().BeginTransaction())
            {
                try
                {
                    foreach (DataRow row in changedRows.Rows)
                    {
                        updateAction(sqlConnector.GetConnection(), transaction, row);
                    }

                    transaction.Commit();
                    combinedTable.AcceptChanges();
                    MessageBox.Show("Изменения сохранены.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                }
            }
        }

        private void UpdateMain(SqlConnection conn, SqlTransaction transaction, DataRow row)
        {
            using (var cmd = new SqlCommand(
                "UPDATE main SET addr = @addr, number_newspaper = @number_newspaper WHERE id = @id", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@addr", row["main_addr"]);
                cmd.Parameters.AddWithValue("@number_newspaper", row["number_newspaper"]);
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateNewspaper(SqlConnection conn, SqlTransaction transaction, DataRow row)
        {
            using (var cmd = new SqlCommand(
                @"UPDATE newspaper 
          SET title = @title 
          WHERE id = @id", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@title", row["newspaper_title"]);
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdatePrintingHouse(SqlConnection conn, SqlTransaction transaction, DataRow row)
        {
            using (var cmd = new SqlCommand(
                "UPDATE printing_house SET addr = @addr WHERE id = @id", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@addr", row["addr"]);
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.ExecuteNonQuery();
            }
        }

        public void updateView(string sql, DataGridView dataGridView1)
        {
            dataGridView1.DataSource = sqlConnector.Fill(sql);
        }

        private void save_Click(object sender, EventArgs e1)
        {
            switch (buttosList.SelectedIndex)
            {
                case 0:
                    SaveChanges((DataTable) dataGridView1.DataSource, UpdateMain);
                    break;
                case 1:
                    SaveChanges((DataTable)dataGridView1.DataSource, UpdatePrintingHouse);
                    break;
                case 2:
                    SaveChanges((DataTable)dataGridView1.DataSource, UpdateNewspaper);
                    break;
            }
        }

    }
}
