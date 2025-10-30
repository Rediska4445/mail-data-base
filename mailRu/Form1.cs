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
        private SqlConnectorService sqlService;

        public Form1()
        {
            sqlConnector = new SqlConnector();
            sqlConnector.Open();
            sqlConnector.Push("USE Mail;", null);

            sqlService = new SqlConnectorService(sqlConnector);

            InitializeComponent();

            this.dataGridView1.DataError += new DataGridViewDataErrorEventHandler(dataGridView1_DataError);
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

        public void updateView(string sql, DataGridView dataGridView1)
        {
            dataGridView1.DataSource = sqlConnector.Fill(sql);
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
                                sqlService.AddNewMail(row);
                                break;
                            case 1:
                                sqlService.AddNewPrintingHouse(row);
                                break;
                            case 2:
                                sqlService.AddNewNewspaper(row);
                                break;
                        }
                    }
                }
            }
        }

        private void commandLine_TextChanged(object sender, EventArgs e)
        {
            Exception error;

            var resultTable = sqlService.SearchAndFillDataGridView(buttosList.SelectedItem?.ToString(), commandLine.Text, out error);

            if (error != null)
            {
                MessageBox.Show("Ошибка при поиске: " + error.Message);
            }
            else
            {
                dataGridView1.DataSource = resultTable;
            }

        }

        private void SaveChanges(DataTable combinedTable, Action<SqlConnection, SqlTransaction, DataRow> updateAction)
        {
            try
            {
                sqlService.SaveChanges(combinedTable, updateAction);
                MessageBox.Show("Изменения сохранены.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void update_Click(object sender, EventArgs e)
        {
            switch (buttosList.SelectedIndex)
            {
                case 0:
                    SaveChanges((DataTable)dataGridView1.DataSource, sqlService.UpdateMail);
                    break;
                case 1:
                    SaveChanges((DataTable)dataGridView1.DataSource, sqlService.UpdatePrintingHouse);
                    break;
                case 2:
                    SaveChanges((DataTable)dataGridView1.DataSource, sqlService.UpdateNewspaper);
                    break;
            }
        }

        private void remove_Click(object sender, EventArgs e)
        {

        }
    }
}