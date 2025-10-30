using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

            this.dataGridView1.DataError += new DataGridViewDataErrorEventHandler(dataGridView1_DataError);

            dataGridView1.RowsAdded += dataGridView1_RowValidated;

            sqlConnector = new SqlConnector();
            sqlConnector.Open();
            sqlConnector.Push("USE Mail;", null);
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Произошла ошибка при обработке данных: " + e.Exception.Message);

            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Ошибка при сохранении данных.");
            }
            else if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("Ошибка разбора данных.");
            }

            e.ThrowException = false;
        }

        private void dataGridView1_RowValidated(object sender, DataGridViewRowsAddedEventArgs e)
        {

        }

        private void AddNewMain(DataRow row)
        {
            string insertSql = "INSERT INTO main (addr, newspaper_id, number_newspaper, id) VALUES " +
                "(@addr, @newspaper_id, @number_newspaper, @id)";
            ExecuteInsert(row, insertSql, cmd =>
            {
                cmd.Parameters.AddWithValue("@addr", row["addr"]);
                cmd.Parameters.AddWithValue("@newspaper_id", row["newspaper_id"]);
                cmd.Parameters.AddWithValue("@number_newspaper", row["number_newspaper"]);
                cmd.Parameters.AddWithValue("@id", row["id"]);
            });
        }

        private void AddNewNewspaper(DataRow row)
        {
            string insertSql = @"
                INSERT INTO newspaper
                (id, title, edition_code, price, full_name, number, printing_house)
                VALUES
                (@id, @title, @edition_code, @price, @full_name, @number, @printing_house)";

            ExecuteInsert(row, insertSql, cmd =>
            {
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.Parameters.AddWithValue("@title", row["title"]);
                cmd.Parameters.AddWithValue("@edition_code", row["edition_code"]);
                cmd.Parameters.AddWithValue("@price", row["price"]);
                cmd.Parameters.AddWithValue("@full_name", row["full_name"]);
                cmd.Parameters.AddWithValue("@number", row["number"]);
                cmd.Parameters.AddWithValue("@printing_house", row["printing_house"]);
            });
        }

        private void AddNewPrintingHouse(DataRow row)
        {
            string insertSql = "INSERT INTO printing_house (id, addr) VALUES (@id, @addr)";
            ExecuteInsert(row, insertSql, cmd =>
            {
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.Parameters.AddWithValue("@addr", row["addr"]);
            });
        }

        private void ExecuteInsert(DataRow row, string insertSql, Action<SqlCommand> setParameters)
        {
            var conn = sqlConnector.GetConnection();

            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    using (var cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        setParameters(cmd);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Ошибка при добавлении записи: " + Environment.NewLine + ex.Message);
                }
            }
        }

        private void buttosList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(buttosList.SelectedIndex)
            {
                case 0:
                    updateView("SELECT m.id, m.addr, m.number_newspaper, m.newspaper_id, n.title AS newspaper_title\r\nFROM main m\r\nLEFT JOIN newspaper n ON m.newspaper_id = n.id;\r\n",
                        dataGridView1
                    );
                    break;
                case 1:
                    updateView("SELECT * FROM printing_house;",
                        dataGridView1
                    );
                    break;
                case 2:
                    updateView("SELECT n.id, n.title, n.edition_code, n.price, n.full_name, n.printing_house, ph.addr AS printing_house_addr, n.number\r\nFROM newspaper n\r\nLEFT JOIN printing_house ph ON n.printing_house = ph.id;", 
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
                "UPDATE main SET addr = @addr, number_newspaper = @number_newspaper, newspaper_id = @newspaper_id WHERE id = @id", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@addr", row["addr"]);
                cmd.Parameters.AddWithValue("@newspaper_id", row["newspaper_id"]);
                cmd.Parameters.AddWithValue("@number_newspaper", row["number_newspaper"]);
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateNewspaper(SqlConnection conn, SqlTransaction transaction, DataRow row)
        {
            using (var cmd = new SqlCommand(
                @"UPDATE newspaper
                    SET 
                        title = @title,
                        edition_code = @edition_code,
                        price = @price,
                        full_name = @full_name,
                        printing_house = @printing_house,
                        number = @number
                    WHERE id = @id;
                    ", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.Parameters.AddWithValue("@title", row["title"]);
                cmd.Parameters.AddWithValue("@edition_code", row["edition_code"]);
                cmd.Parameters.AddWithValue("@price", row["price"]);
                cmd.Parameters.AddWithValue("@full_name", row["full_name"]);
                cmd.Parameters.AddWithValue("@number", row["number"]);
                cmd.Parameters.AddWithValue("@printing_house", row["printing_house"]);
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
            console.Text += sqlConnector.GetConnection().State;

            dataGridView1.DataSource = sqlConnector.Fill(sql);

            console.Text += sqlConnector.GetConnection().State;
        }

        private void save_Click(object sender, EventArgs e1)
        {
            var table = (DataTable)dataGridView1.DataSource;
            var changedRows = table.GetChanges();

            if (changedRows != null)
            {
                foreach (DataRow row in changedRows.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        console.Text += "ed";

                        switch (buttosList.SelectedIndex)
                        {
                            case 0:
                                AddNewMain(row);
                                break;
                            case 1:
                                AddNewPrintingHouse(row);
                                break;
                            case 2:
                                AddNewNewspaper(row);
                                break;
                        }
                    }
                }
            }
        }

        private void SearchAndFillDataGridView(string tableName, string keyword, DataGridView dgv)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Таблица должна быть задана");

            var conn = sqlConnector.GetConnection();

            var columnQuery = @"
                SELECT COLUMN_NAME, DATA_TYPE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @tableName";

            var columnCmd = new SqlCommand(columnQuery, conn);
            columnCmd.Parameters.AddWithValue("@tableName", tableName);

            var reader = columnCmd.ExecuteReader();
            var textColumns = new List<string>();
            var numericColumns = new List<string>();

            while (reader.Read())
            {
                string colName = reader.GetString(0);
                string dataType = reader.GetString(1).ToLower();

                if (new[] { "char", "varchar", "nchar", "nvarchar", "text", "ntext" }.Contains(dataType))
                    textColumns.Add(colName);
                else if (new[] { "int", "bigint", "smallint", "tinyint", "decimal", "numeric", "float", "real" }.Contains(dataType))
                    numericColumns.Add(colName);
            }
            reader.Close();

            if (textColumns.Count == 0 && numericColumns.Count == 0)
                throw new Exception($"В таблице {tableName} нет поддерживаемых для поиска колонок");

            string sql;
            var conditions = new List<string>();

            if (string.IsNullOrEmpty(keyword))
            {
                sql = $"SELECT * FROM {tableName}";
            }
            else
            {
                conditions.AddRange(textColumns.Select(c => $"{c} LIKE @keyword"));

                if (decimal.TryParse(keyword, out var numericValue))
                {
                    conditions.AddRange(numericColumns.Select(c => $"{c} = @numericValue"));
                }

                sql = $"SELECT * FROM {tableName} WHERE " + string.Join(" OR ", conditions);
            }

            using (var searchCmd = new SqlCommand(sql, conn))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    searchCmd.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                    if (decimal.TryParse(keyword, out var numericValue))
                        searchCmd.Parameters.AddWithValue("@numericValue", numericValue);
                }

                var adapter = new SqlDataAdapter(searchCmd);
                var resultTable = new DataTable();
                adapter.Fill(resultTable);

                dgv.DataSource = resultTable;
            }
        }

        private void commandLine_TextChanged(object sender, EventArgs e)
        {
            SearchAndFillDataGridView(buttosList.SelectedItem?.ToString(), commandLine.Text, dataGridView1);
        }

        private void update_Click(object sender, EventArgs e)
        {
            switch (buttosList.SelectedIndex)
            {
                case 0:
                    SaveChanges((DataTable)dataGridView1.DataSource, UpdateMain);
                    break;
                case 1:
                    SaveChanges((DataTable)dataGridView1.DataSource, UpdatePrintingHouse);
                    break;
                case 2:
                    SaveChanges((DataTable)dataGridView1.DataSource, UpdateNewspaper);
                    break;
            }
        }

        private void remove_Click(object sender, EventArgs e)
        {

        }
    }
}