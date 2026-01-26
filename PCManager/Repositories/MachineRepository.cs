using MySqlConnector;
using PCManager.Models;

namespace PCManager.Repositories
{
    public class MachineRepository
    {
        public Machine? GetByName(string name)
        {
            return DBManager.SafeExecute(conn =>
            {
                string sql = "SELECT * FROM machine Where Name = @name";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", name);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Machine
                    {
                        Id = Convert.ToInt64(reader["Id"]),
                        Name = reader["Name"]?.ToString() ?? " ",
                        PosX = Convert.ToInt32(reader["PosX"]),
                        PosY = Convert.ToInt32(reader["PosY"]),
                        StartTime = Convert.ToDateTime(reader["StartTime"])
                    };
                }
                return null;
            });
        }

        public Machine? GetById(long id)
        {
            return DBManager.SafeExecute(conn =>
            {
                string sql = "SELECT * FROM machine Where Id = @id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Machine
                    {
                        Id = Convert.ToInt64(reader["Id"]),
                        Name = reader["Name"]?.ToString() ?? " ",
                        PosX = Convert.ToInt32(reader["PosX"]),
                        PosY = Convert.ToInt32(reader["PosY"]),
                        StartTime = Convert.ToDateTime(reader["StartTime"])
                    };
                }
                return null;
            });
        }

        public List<Machine> GetAll()
        {
            return DBManager.SafeExecute(conn =>
            {
                var list = new List<Machine>();
                string sql = "SELECT * FROM machine";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Machine
                    {
                        Id = Convert.ToInt64(reader["Id"]),
                        Name = reader["Name"]?.ToString() ?? " ",
                        PosX = Convert.ToInt32(reader["PosX"]),
                        PosY = Convert.ToInt32(reader["PosY"]),
                        StartTime = Convert.ToDateTime(reader["StartTime"])
                    });
                }
                return list;
            }) ?? new List<Machine>();  // 에러 시 빈 리스트 반환
        }

        public bool Save(Machine machine)
        {
            return DBManager.SafeExecute(conn =>
            {
                using var trans = conn.BeginTransaction();
                string query = "" + "INSERT INTO machine(Name, StartTime, PosX, PosY) VALUES (@name, @startTime, @x, @y);";

                using var cmd = new MySqlCommand(query, conn, trans);

                cmd.Parameters.AddWithValue("@x", machine.PosX);
                cmd.Parameters.AddWithValue("@y", machine.PosY);
                cmd.Parameters.AddWithValue("@name", machine.Name);
                cmd.Parameters.AddWithValue("@startTime", machine.StartTime);
                try
                {
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }

                return true;
            });
        }

        public bool SaveAll(IEnumerable<Machine> machines)
        {
            return DBManager.SafeExecute(conn =>
            {
                using var trans = conn.BeginTransaction();
                try
                {
                    foreach (var m in machines)
                    {
                        // ID 존재 여부에 따라 쿼리 선택
                        string query = m.Id == 0
                            ? "INSERT INTO machine(Name, StartTime, PosX, PosY) VALUES (@name, @startTime, @x, @y)"
                            : "UPDATE machine SET PosX = @x, PosY = @y WHERE Id = @id";

                        using var cmd = new MySqlCommand(query, conn, trans);

                        cmd.Parameters.AddWithValue("@x", m.PosX);
                        cmd.Parameters.AddWithValue("@y", m.PosY);
                        cmd.Parameters.AddWithValue("@name", m.Name);
                        cmd.Parameters.AddWithValue("@startTime", m.StartTime);

                        if (m.Id != 0)
                            cmd.Parameters.AddWithValue("@id", m.Id);

                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    System.Diagnostics.Debug.WriteLine($"저장 실패: {ex.Message}");
                    return false;
                }
            });
        }
    }
}
