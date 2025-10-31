using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace mailRu
{
    /// <summary>
    /// Класс <c>SqlConnectorService</c> предоставляет методы для управления данными в базе.
    /// Отвечает за операции добавления новых записей, обновления существующих,
    /// сохранения изменений в транзакциях и поиска с фильтрацией по таблицам базы данных.
    /// </summary>
    internal class SqlConnectorService
    {
        /// <summary>
        /// Внутренний экземпляр класса SqlConnector, управляющий соединением с базой данных.
        /// </summary>
        private readonly SqlConnector sqlConnector;

        /// <summary>
        /// Конструктор, инициализирующий сервис с указанным экземпляром <see cref="SqlConnector"/>.
        /// Открывает соединение с базой данных.
        /// </summary>
        /// <param name="connector">Экземпляр <see cref="SqlConnector"/> для работы с базой.</param>
        public SqlConnectorService(SqlConnector connector)
        {
            sqlConnector = connector;
            sqlConnector.Open();
        }

        /// <summary>
        /// Выполняет вставку новой записи в базу внутри транзакции.
        /// </summary>
        /// <param name="row">Строка данных для вставки.</param>
        /// <param name="insertSql">SQL-запрос вставки.</param>
        /// <param name="setParameters">Делегат, задающий параметры команды на основе строки данных.</param>
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
                    throw new InvalidOperationException("Ошибка при добавлении записи", ex);
                }
            }
        }

        /// <summary>
        /// Добавляет новую почтовую запись (main) в базу.
        /// Должен использоваться как ссылка для метода <c>SaveChanges</c>
        /// </summary>
        /// <param name="row">Данные новой почтовой записи.</param>
        public void AddNewMail(DataRow row)
        {
            string insertSql =
                "INSERT INTO main (addr, newspaper_id, number_newspaper, id) VALUES (@addr, @newspaper_id, @number_newspaper, @id)";

            ExecuteInsert(row, insertSql, cmd =>
            {
                cmd.Parameters.AddWithValue("@addr", row["addr"]);
                cmd.Parameters.AddWithValue("@newspaper_id", row["newspaper_id"]);
                cmd.Parameters.AddWithValue("@number_newspaper", row["number_newspaper"]);
                cmd.Parameters.AddWithValue("@id", row["id"]);
            });
        }

        /// <summary>
        /// Добавляет новую запись в таблицу газет (newspaper).
        /// Должен использоваться как ссылка для метода <c>SaveChanges</c>
        /// </summary>
        /// <param name="row">Данные газеты.</param>
        public void AddNewNewspaper(DataRow row)
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

        /// <summary>
        /// Добавляет новую типографию (printing_house) в базу.
        /// Должен использоваться как ссылка для метода <c>SaveChanges</c>
        /// </summary>
        /// <param name="row">Данные типографии.</param>
        public void AddNewPrintingHouse(DataRow row)
        {
            string insertSql = "INSERT INTO printing_house (id, addr) VALUES (@id, @addr)";

            ExecuteInsert(row, insertSql, cmd =>
            {
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.Parameters.AddWithValue("@addr", row["addr"]);
            });
        }

        /// <summary>
        /// Обновляет существующую запись в таблице main (почта).
        /// Должен использоваться как ссылка для метода <c>SaveChanges</c>
        /// </summary>
        /// <param name="conn">Активное соединение с базой данных.</param>
        /// <param name="transaction">Активная транзакция БД.</param>
        /// <param name="row">Строка с новыми данными для обновления.</param>
        public void UpdateMail(SqlConnection conn, SqlTransaction transaction, DataRow row)
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

        /// <summary>
        /// Обновляет запись в таблице newspaper (газеты).
        /// Должен использоваться как ссылка для метода <c>SaveChanges</c>
        /// </summary>
        /// <param name="conn">Активное соединение с базой данных.</param>
        /// <param name="transaction">Активная транзакция БД.</param>
        /// <param name="row">Строка с новыми данными для обновления.</param>
        public void UpdateNewspaper(SqlConnection conn, SqlTransaction transaction, DataRow row)
        {
            using (var cmd = new SqlCommand(
                @"UPDATE newspaper
                  SET title = @title,
                      edition_code = @edition_code,
                      price = @price,
                      full_name = @full_name,
                      printing_house = @printing_house,
                      number = @number
                  WHERE id = @id;", conn, transaction))
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

        /// <summary>
        /// Обновляет запись в таблице printing_house (типографии).
        /// Должен использоваться как ссылка для метода <c>SaveChanges</c>
        /// </summary>
        /// <param name="conn">Активное соединение с базой данных.</param>
        /// <param name="transaction">Активная транзакция БД.</param>
        /// <param name="row">Строка с новыми данными для обновления.</param>
        public void UpdatePrintingHouse(SqlConnection conn, SqlTransaction transaction, DataRow row)
        {
            using (var cmd = new SqlCommand(
                "UPDATE printing_house SET addr = @addr WHERE id = @id", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@addr", row["addr"]);
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Сохраняет изменения в объединённой таблице данных, вызывая переданный метод обновления для каждой изменённой строки.
        /// </summary>
        /// <param name="combinedTable">Объединённая таблица с изменениями.</param>
        /// <param name="updateAction">Делегат метода обновления записи в базе.</param>
        public void SaveChanges(DataTable combinedTable, Action<SqlConnection, SqlTransaction, DataRow> updateAction)
        {
            var changedRows = combinedTable.GetChanges();
            if (changedRows == null)
                return;

            var conn = sqlConnector.GetConnection();
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    foreach (DataRow row in changedRows.Rows)
                    {
                        updateAction(conn, transaction, row);
                    }

                    transaction.Commit();
                    combinedTable.AcceptChanges();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw new InvalidOperationException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Выполняет поиск записей в таблице базы с возможностью фильтрации по ключевому слову.
        /// Автоматически выбирает столбцы для текстового и числового поиска по типам данных.
        /// </summary>
        /// <param name="tableName">Имя таблицы для поиска.</param>
        /// <param name="keyword">Ключевое слово для фильтрации.</param>
        /// <param name="error">Выходной параметр с описанием ошибки при выполнении запроса, если возникла.</param>
        /// <returns>Таблица с результатами поиска или null при ошибке.</returns>
        public DataTable SearchAndFillDataGridView(string tableName, string keyword, out Exception error)
        {
            error = null;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Имя таблицы не задано.");

            var conn = sqlConnector.GetConnection();

            var columnQuery = @"
                SELECT COLUMN_NAME, DATA_TYPE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @tableName";

            var textColumns = new List<string>();
            var numericColumns = new List<string>();

            try
            {
                using (var columnCmd = new SqlCommand(columnQuery, conn))
                {
                    columnCmd.Parameters.AddWithValue("@tableName", tableName);
                    using (var reader = columnCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string colName = reader.GetString(0);
                            string dataType = reader.GetString(1).ToLower();

                            if (new[] { 
                                "char", "varchar", "nchar", "nvarchar", "text", "ntext" 
                            }.Contains(dataType))
                                textColumns.Add(colName);
                            else if (new[] { 
                                "int", "bigint", "smallint", "tinyint", "decimal", "numeric", "float", "real" 
                            }.Contains(dataType))
                                numericColumns.Add(colName);
                        }
                    }
                }

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

                    return resultTable;
                }
            }
            catch (Exception ex)
            {
                error = ex;
                return null;
            }
        }
    }
}