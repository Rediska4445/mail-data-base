using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;
using SqlTransaction = Microsoft.Data.SqlClient.SqlTransaction;

namespace mailRu
{
    public partial class Form1 : Form
    {
        private SqlConnector sqlConnector;
        private SqlConnectorService sqlService;

        private const string mainDb = "MailTest";

        public Form1()
        {
            InitializeComponent();

            console.Text += $"[{DateTime.Now}] Инициализация Form1...\r\n";

            sqlConnector = new SqlConnector();
            sqlConnector.Open();

            console.Text += $"[{DateTime.Now}] Подключение к серверу SQL открыто.\r\n";

            try
            {
                var checkDbSql = $"SELECT database_id FROM sys.databases WHERE Name = '{mainDb}'";
                var dt = sqlConnector.Fill(checkDbSql);

                if (dt.Rows.Count == 0)
                {
                    console.Text += $"[{DateTime.Now}] База {mainDb} не найдена. Создаём базу...\r\n";

                    sqlConnector.Push($"CREATE DATABASE {mainDb};", null);
                    console.Text += $"[{DateTime.Now}] База {mainDb} успешно создана.\r\n";
                }
                else
                {
                    console.Text += $"[{DateTime.Now}] База {mainDb} существует.\r\n";
                }

                sqlConnector.Push($"USE {mainDb};", null);
                console.Text += $"[{DateTime.Now}] Используется база {mainDb}.\r\n";

                sqlConnector.Push(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='printing_house' and xtype='U')
                    CREATE TABLE printing_house (
                        id INT PRIMARY KEY NOT NULL,
                        addr VARCHAR(96)
                    );", null);
                console.Text += $"[{DateTime.Now}] Создание таблицы printing_house (если отсутствует).\r\n";

                sqlConnector.Push(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='newspaper' and xtype='U')
                    CREATE TABLE newspaper (
                        id INT PRIMARY KEY NOT NULL,
                        title VARCHAR(64),
                        edition_code VARCHAR(64),
                        price INT NOT NULL,
                        full_name VARCHAR(96) NOT NULL,
                        printing_house INT NULL,
                        number INT,
                        CONSTRAINT FK_newspaper_printing_house FOREIGN KEY (printing_house) REFERENCES printing_house(id)
                    );", null);
                console.Text += $"[{DateTime.Now}] Создание таблицы newspaper (если отсутствует).\r\n";

                sqlConnector.Push(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='main' and xtype='U')
                CREATE TABLE main (
                    id INT PRIMARY KEY NOT NULL,
                    addr VARCHAR(96),
                    newspaper_id INT,
                    number_newspaper INT,
                    CONSTRAINT FK_main_newspaper FOREIGN KEY (newspaper_id) REFERENCES newspaper(id)
                );", null);
                console.Text += $"[{DateTime.Now}] Создание таблицы main (если отсутствует).\r\n";

                sqlService = new SqlConnectorService(sqlConnector);

                this.dataGridView1.DataError += new DataGridViewDataErrorEventHandler(dataGridView1_DataError);
                console.Text += $"[{DateTime.Now}] Подписка на событие DataError завершена.\r\n";
            }
            catch (Exception ex)
            {
                console.Text += $"[{DateTime.Now}] Ошибка при инициализации базы данных: {ex.Message}\r\n";
                MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message);
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            console.Text += $"[{DateTime.Now}] Ошибка DataGridView: {e.Exception.Message}\r\n";

            MessageBox.Show("Произошла ошибка при обработке данных: " + e.Exception.Message);

            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                console.Text += $"[{DateTime.Now}] Ошибка контекста Commit.\r\n";
                MessageBox.Show("Ошибка при сохранении данных.");
            }
            else if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                console.Text += $"[{DateTime.Now}] Ошибка контекста Parsing.\r\n";
                MessageBox.Show("Ошибка разбора данных.");
            }

            e.ThrowException = false;
        }

        private void buttosList_SelectedIndexChanged(object sender, EventArgs e)
        {
            console.Text += $"[{DateTime.Now}] Выбран пункт списка: {buttosList.SelectedItem}\r\n";
            UpdateDataGridView();
        }

        private void UpdateDataGridView()
        {
            console.Text += $"[{DateTime.Now}] Обновляется DataGridView для индекса: {buttosList.SelectedIndex}\r\n";

            switch (buttosList.SelectedIndex)
            {
                case 0:
                    updateView("SELECT m.id, m.addr, m.number_newspaper, m.newspaper_id, n.title AS newspaper_title\r\nFROM main m\r\nLEFT JOIN newspaper n ON m.newspaper_id = n.id;\r\n",
                        dataGridView1
                    );
                    console.Text += $"[{DateTime.Now}] Загружены данные почты.\r\n";
                    break;
                case 1:
                    updateView("SELECT * FROM printing_house;", dataGridView1);
                    console.Text += $"[{DateTime.Now}] Загружены данные типографии.\r\n";
                    break;
                case 2:
                    updateView("SELECT n.id, n.title, n.edition_code, n.price, n.full_name, n.printing_house, ph.addr AS printing_house_addr, n.number\r\nFROM newspaper n\r\nLEFT JOIN printing_house ph ON n.printing_house = ph.id;",
                        dataGridView1
                    );
                    console.Text += $"[{DateTime.Now}] Загружены данные газет.\r\n";
                    break;
                default:
                    console.Text += $"[{DateTime.Now}] Неизвестный индекс в списке.\r\n";
                    break;
            }
        }

        private void updateView(string sql, DataGridView dataGridView1)
        {
            console.Text += $"[{DateTime.Now}] Выполняется SQL-запрос: {sql.Trim()}\r\n";
            try
            {
                dataGridView1.DataSource = sqlConnector.Fill(sql);
                console.Text += $"[{DateTime.Now}] Данные успешно загружены и применены к DataGridView.\r\n";
            }
            catch (Exception ex)
            {
                console.Text += $"[{DateTime.Now}] Ошибка при загрузке данных: {ex.Message}\r\n";
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void save_Click(object sender, EventArgs e1)
        {
            var table = (DataTable)dataGridView1.DataSource;
            var changedRows = table?.GetChanges();

            if (changedRows != null)
            {
                console.Text += $"[{DateTime.Now}] Обнаружено изменений: {changedRows.Rows.Count}\r\n";

                foreach (DataRow row in changedRows.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        console.Text += $"[{DateTime.Now}] Добавляется новая запись.\r\n";

                        switch (buttosList.SelectedIndex)
                        {
                            case 0:
                                sqlService.AddNewMail(row);
                                console.Text += $"[{DateTime.Now}] Вставлена новая почтовая запись.\r\n";
                                break;
                            case 1:
                                sqlService.AddNewPrintingHouse(row);
                                console.Text += $"[{DateTime.Now}] Вставлена новая типография.\r\n";
                                break;
                            case 2:
                                sqlService.AddNewNewspaper(row);
                                console.Text += $"[{DateTime.Now}] Вставлена новая газета.\r\n";
                                break;
                            default:
                                console.Text += $"[{DateTime.Now}] Неизвестный тип данных для вставки.\r\n";
                                break;
                        }
                    }
                }
            }
            else
            {
                console.Text += $"[{DateTime.Now}] Изменений для сохранения не обнаружено.\r\n";
            }
        }

        private void commandLine_TextChanged(object sender, EventArgs e)
        {
            Exception error;
            console.Text += $"[{DateTime.Now}] Выполняется поиск с ключевым словом: '{commandLine.Text}'.\r\n";

            var resultTable = sqlService.SearchAndFillDataGridView(buttosList.SelectedItem?.ToString(), commandLine.Text, out error);

            if (error != null)
            {
                console.Text += $"[{DateTime.Now}] Ошибка при поиске: {error.Message}\r\n";
                MessageBox.Show("Ошибка при поиске: " + error.Message);
            }
            else
            {
                dataGridView1.DataSource = resultTable;
                console.Text += $"[{DateTime.Now}] Поиск выполнен успешно, данные обновлены.\r\n";
            }
        }

        private void SaveChanges(DataTable combinedTable, Action<SqlConnection, SqlTransaction, DataRow> updateAction)
        {
            try
            {
                console.Text += $"[{DateTime.Now}] Начинается сохранение изменений.\r\n";
                sqlService.SaveChanges(combinedTable, updateAction);
                console.Text += $"[{DateTime.Now}] Изменения успешно сохранены.\r\n";
                MessageBox.Show("Изменения сохранены.");
            }
            catch (Exception ex)
            {
                console.Text += $"[{DateTime.Now}] Ошибка при сохранении изменений: {ex.Message}\r\n";
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void update_Click(object sender, EventArgs e)
        {
            console.Text += $"[{DateTime.Now}] Обновление записи с индексом: {buttosList.SelectedIndex}\r\n";

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
                default:
                    console.Text += $"[{DateTime.Now}] Неизвестный индекс для обновления.\r\n";
                    break;
            }
        }

        private int GetSelectedIdFromDataGrid()
        {
            console.Text += $"[{DateTime.Now}] Определение выбранного ID в DataGridView.\r\n";

            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                object cellValue = selectedRow.Cells["id"].Value;
                if (cellValue != null && int.TryParse(cellValue.ToString(), out int id))
                {
                    console.Text += $"[{DateTime.Now}] Выбранный ID: {id}.\r\n";
                    return id;
                }
            }
            else if (dataGridView1.CurrentRow != null)
            {
                object cellValue = dataGridView1.CurrentRow.Cells["id"].Value;
                if (cellValue != null && int.TryParse(cellValue.ToString(), out int id))
                {
                    console.Text += $"[{DateTime.Now}] Текущий ID: {id}.\r\n";
                    return id;
                }
            }

            string errorMessage = "Не выбрана строка или столбец 'id' отсутствует или содержит неверное значение";
            console.Text += $"[{DateTime.Now}] Ошибка: {errorMessage}\r\n";
            throw new InvalidOperationException(errorMessage);
        }

        private void remove_Click(object sender, EventArgs e)
        {
            console.Text += $"[{DateTime.Now}] Начало удаления записи с индексом: {buttosList.SelectedIndex}\r\n";

            switch (buttosList.SelectedIndex)
            {
                case 0:
                    {
                        var idToDelete = GetSelectedIdFromDataGrid();
                        console.Text += $"[{DateTime.Now}] Удаляется запись из main с ID: {idToDelete}\r\n";

                        sqlConnector.Push("DELETE FROM main WHERE id = @id", cmd =>
                        {
                            cmd.Parameters.AddWithValue("@id", idToDelete);
                        });

                        console.Text += $"[{DateTime.Now}] Удаление записи из main завершено.\r\n";
                    }
                    break;

                case 1:
                    {
                        var idToDelete = GetSelectedIdFromDataGrid();
                        console.Text += $"[{DateTime.Now}] Удаление связанных записей с типографией ID: {idToDelete}\r\n";

                        sqlConnector.Push(@"
                        DELETE main 
                        WHERE newspaper_id IN 
                        (SELECT id FROM newspaper WHERE printing_house = @id);
                        DELETE newspaper WHERE printing_house = @id;
                        DELETE printing_house WHERE id = @id;", cmd =>
                        {
                            cmd.Parameters.AddWithValue("@id", idToDelete);
                        });

                        console.Text += $"[{DateTime.Now}] Удаление типографии и связанных записей завершено.\r\n";
                    }
                    break;

                case 2:
                    {
                        var idToDelete = GetSelectedIdFromDataGrid();
                        console.Text += $"[{DateTime.Now}] Удаление газеты с ID: {idToDelete} и связанных почтовых записей.\r\n";

                        sqlConnector.Push(@"
                        DELETE FROM main WHERE newspaper_id = @id;
                        DELETE FROM newspaper WHERE id = @id;", cmd =>
                        {
                            cmd.Parameters.AddWithValue("@id", idToDelete);
                        });

                        console.Text += $"[{DateTime.Now}] Удаление газеты и связанных записей завершено.\r\n";
                    }
                    break;

                default:
                    console.Text += $"[{DateTime.Now}] Неизвестный индекс для удаления.\r\n";
                    break;
            }

            UpdateDataGridView();
            console.Text += $"[{DateTime.Now}] Обновление DataGridView после удаления.\r\n";
        }

        private void request_TextChanged(object sender, EventArgs e)
        {

        }

        private void request_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                console.Text += $"[{DateTime.Now}] Выполняется SQL-запрос:\r\n{request.Text}\r\n";

                sqlConnector.Read(request.Text, reader =>
                {
                    try
                    {
                        var rowValues = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object value = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i);
                            rowValues.Add($"{columnName}={value}");
                        }

                        string rowText = string.Join(", ", rowValues);
                        console.Text += $"[{DateTime.Now}] Строка результата: {rowText}\r\n";
                    }
                    catch (Exception ex)
                    {
                        console.Text += $"[{DateTime.Now}] Ошибка при обработке результата: {ex.Message}\r\n";
                    }
                });

                console.Text += $"[{DateTime.Now}] Выполнение запроса завершено.\r\n";
            }
        }
    }
}