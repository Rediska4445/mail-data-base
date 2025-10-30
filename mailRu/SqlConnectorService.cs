using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace mailRu
{
    internal class SqlConnectorService
    {
        private readonly SqlConnector sqlConnector;

        public SqlConnectorService(SqlConnector connector)
        {
            sqlConnector = connector;
            sqlConnector.Open();
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
                    throw new InvalidOperationException("Ошибка при добавлении записи", ex);
                }
            }
        }

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

        public void AddNewPrintingHouse(DataRow row)
        {
            string insertSql = "INSERT INTO printing_house (id, addr) VALUES (@id, @addr)";

            ExecuteInsert(row, insertSql, cmd =>
            {
                cmd.Parameters.AddWithValue("@id", row["id"]);
                cmd.Parameters.AddWithValue("@addr", row["addr"]);
            });
        }

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
