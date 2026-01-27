using Dapper;
using Dommel;
using MySqlConnector;
using PCManager.Models;

namespace PCManager.Repositories
{
    public class MachineRepository
    {
        // Dommel 기반
        public async Task<Machine?> GetByNameAsync(string name)
        {
            return await DBManager.SafeExecuteAsync(async conn =>
            {
                var results = await conn.SelectAsync<Machine>(m => m.Name == name);
                return results.FirstOrDefault();
            });
        }

        public Machine? GetByName(string name)
            => DBManager.SafeExecute(conn => conn.Select<Machine>(m => m.Name == name))?.FirstOrDefault();

        public Machine? GetById(ulong id)
            => DBManager.SafeExecute(conn => conn.QueryFirstOrDefault<Machine>("SELECT * FROM machine WHERE id = @id", new { id }));

        public async Task<Machine?> GetByIdAsync(ulong id)
            => await DBManager.SafeExecuteAsync(async conn => await conn.GetAsync<Machine>(id));

        public IEnumerable<Machine> GetAll()
            => DBManager.SafeExecute(conn => conn.GetAll<Machine>().ToList()) ?? new List<Machine>();


        public bool Save(Machine machine)
        {
            bool result = DBManager.SafeExecute(conn =>
            {
                using var trans = conn.BeginTransaction();
                try
                {
                    bool isSuccess = false;
                    // Insert/Update 분기 (ID가 0 이면 새 데이터/ 아니면 기존 데이터) => AUTO_INCREMENT라 0인 ID가 불가능함
                    if (machine.Id == 0)
                    {
                        var id = conn.Insert(machine, transaction: trans);
                        machine.Id = Convert.ToUInt64(id);
                        isSuccess = true;
                    }
                    else
                    {
                        isSuccess = conn.Update(machine, transaction: trans);
                    }

                    trans.Commit();
                    return isSuccess;
                }
                catch(Exception ex)
                {
                    trans.Rollback();
                    System.Diagnostics.Debug.WriteLine($"저장 실패: {ex.Message}");
                    return false;
                }
            });

            return result;
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
                        if (m.Id == 0) conn.Insert(m, transaction: trans);
                        else conn.Update(m, transaction: trans);
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
