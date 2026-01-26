using MySqlConnector;
using PCManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PCManager.Repositories
{
    public class WeldingDataRepository
    {
        private string TargetTable => "machine_welding_history";
        private string ConnStr => App.DBConnectionString;

        private static void MapToParams(MySqlCommand cmd, WeldingData data)
        {
            cmd.Parameters.AddWithValue("created_at", data.createdAt);
            cmd.Parameters.AddWithValue("count", data.Count);
            cmd.Parameters.AddWithValue("sinData", MemoryMarshal.AsBytes(data.SinData.AsSpan()).ToArray());
            cmd.Parameters.AddWithValue("cosData", MemoryMarshal.AsBytes(data.CosData.AsSpan()).ToArray());
        }

        public async Task InsertWeldingData(WeldingData weldingData)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    await conn.OpenAsync();
                    using (var transaction = await conn.BeginTransactionAsync())
                    {
                        string sql = $"INSERT into {TargetTable} (start_time_utc, count, cos_raw_data, sin_raw_data) values (@start_time_utc, @count, @cosData, @sinData)";

                        using (var cmd = new MySqlCommand(commandText: sql, connection: conn))
                        {
                            MapToParams(cmd, weldingData);
                            var rowsAffected = await cmd.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DB ERROR : " + ex.Message);
            }
        }

        public async Task<WeldingData?> GetWeldingDataByIdAsync(long id)
        {
            using (var conn = new MySqlConnection(ConnStr))
            {
                await conn.OpenAsync();
                string sql = $"SELECT start_time_utc, cos_raw_data, sin_raw_data FROM {TargetTable} WHERE id = @id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            byte[] cosRaw = (byte[])reader["cos_raw_data"];
                            byte[] sinRaw = (byte[])reader["sin_raw_data"];

                            return new WeldingData(
                                MemoryMarshal.Cast<byte, double>(sinRaw).ToArray(),
                                MemoryMarshal.Cast<byte, double>(cosRaw).ToArray());
                        }
                        return null;
                    }
                }
            }
        }

        public void GetWeldingList()
        {
            var list = new List<WeldingData>();
            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                string sql = "Select id, timestamp_utc, data_count FROM welding_history ORDER BY id DESC";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WeldingData data;
                        Console.WriteLine(reader.GetString(0));
                    }
                }
            }
        }
    }
}
