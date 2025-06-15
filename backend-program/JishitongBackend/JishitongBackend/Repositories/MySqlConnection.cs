namespace JishitongBackend.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MySql.Data.MySqlClient;

    public class MySqlConnection
    {
        private readonly string _connectionString;

        public MySqlConnection(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// 执行查询并返回结果的列表
        /// </summary>
        /// <param name="query">查询语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>查询结果列表</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string query, List<object> parameters = null)
        {
            var results = new List<Dictionary<string, object>>();

            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(query, connection))
                {
                    // 添加参数（如果有）
                    if (parameters != null)
                    {
                        for (int i = 0; i < parameters.Count; i++)
                        {
                            command.Parameters.AddWithValue($"@param{i}", parameters[i]);  // 可以使用匿名参数
                        }
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }

                            results.Add(row);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// 执行非查询语句（INSERT、UPDATE、DELETE）
        /// </summary>
        /// <param name="query">非查询语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> ExecuteNonQueryAsync(string query, List<object> parameters = null)
        {
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(query, connection))
                {
                    // 添加参数（如果有）
                    if (parameters != null)
                    {
                        for (int i = 0; i < parameters.Count; i++)
                        {
                            command.Parameters.AddWithValue($"@param{i}", parameters[i]);  // 可以使用匿名参数
                        }
                    }

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
    }

}
