using Dapper;
using Dommel;
using PCManager.Models;

namespace PCManager.Repositories
{
    public class WeldingDataRepository
    {
        public bool Save(WeldingData weldingData)
        {
            // 1. 모델의 값이 확실히 있는지 다시 한번 체크 (방금 확인하셨으니 통과)

            //byte[] sinBytes = new byte[weldingData.sin_raw_data.Length * sizeof(double)];
            //Buffer.BlockCopy(weldingData.sin_raw_data, 0, sinBytes, 0, sinBytes.Length);

            //byte[] cosBytes = new byte[weldingData.cos_raw_data.Length * sizeof(double)];
            //Buffer.BlockCopy(weldingData.cos_raw_data, 0, cosBytes, 0, cosBytes.Length);

            string query = @"
        INSERT INTO machine_welding_history (count, machine_id, created_at, sin_raw_data, cos_raw_data) 
        VALUES (@count, @machine_id, @created_at, @sin_raw_data, @cos_raw_data);";

            return DBManager.SafeExecute(conn =>
            {
                int affectedRows = conn.Execute(query, new
                {
                    count = weldingData.sin_raw_data.Length,
                    machine_id = weldingData.machineId,
                    created_at = weldingData.createdAt,
                    sin_raw_data = weldingData.sin_raw_data, // 핸들러 대신 가공된 byte[] 전달
                    cos_raw_data = weldingData.cos_raw_data  // 핸들러 대신 가공된 byte[] 전달
                });
                return affectedRows > 0;
            });
        }

        //public bool Save(WeldingData weldingData)
        //{
        //    return DBManager.SafeExecute(conn =>
        //    {
        //        using var trans = conn.BeginTransaction();
        //        try
        //        {
        //            // [수정] transaction 인자에 trans 객체를 반드시 명시해야 합니다.
        //            conn.Insert<WeldingData>(weldingData, transaction: trans);

        //            trans.Commit();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            trans.Rollback();
        //            System.Diagnostics.Debug.WriteLine($"저장 실패: {ex.Message}");
        //            return false;
        //        }
        //    });
        //}

        public async Task<bool> InsertWeldingDataAsync(WeldingData data)
        {
            // SafeExecuteAsync가 실패 시 내부적으로 default(bool) 즉 false를 반환
            return await DBManager.SafeExecuteAsync(async conn =>
            {
                await conn.InsertAsync(data);
                return true;
            });
        }

        public async Task<WeldingData?> GetWeldingDataByIdAsync(ulong id)
        {
            return await DBManager.SafeExecuteAsync(async conn =>
            {
                return await conn.GetAsync<WeldingData>(id);
            });
        }

        // 3. 특정 기기의 이력 조회 (Select)
        public async Task<IEnumerable<WeldingData>> GetAllByMachineIdAsync(ulong machineId)
        {
            return await DBManager.SafeExecuteAsync(async conn =>
            {
                var list = await conn.SelectAsync<WeldingData>(w => w.machineId == machineId);
                return list.OrderByDescending(w => w.Id);
            }) ?? Enumerable.Empty<WeldingData>();
        }
        public async Task<IEnumerable<WeldingData>> GetSummaryByMachineIdAsync(ulong machineId)
        {
            return await DBManager.SafeExecuteAsync(async conn =>
            {
                // raw_data(BLOB) 컬럼을 제외하고 조회하여 네트워크/메모리 절약
                string sql = "SELECT id, created_at, count, machine_id FROM machine_welding_history WHERE machine_id = @machineId ORDER BY id DESC";
                return await conn.QueryAsync<WeldingData>(sql, new { machineId });
            }) ?? Enumerable.Empty<WeldingData>();
        }

        public void CreateDummyHistory(ulong machineId, int count)
        {
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                // 1. 1000개짜리 배열 생성
                double[] dummySin = new double[1000];
                double[] dummyCos = new double[1000];
                double offset = random.NextDouble() * 10;

                for (int j = 0; j < 1000; j++)
                {
                    dummySin[j] = Math.Sin(offset + j * 0.1);
                    dummyCos[j] = Math.Cos(offset + j * 0.1);
                }

                // 2. 모델 객체 생성 (DoubleArrayHandler가 있으므로 double[]을 그냥 넣으면 됨!)
                var newData = new WeldingData(machineId, dummySin, dummyCos);

                // 3. 리포지토리의 Insert 호출
                Save(newData);
            }
        }

        public async Task CreateDummyHistoryAsync(ulong machineId, int count)
        {
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                // 1. 1000개짜리 배열 생성
                double[] dummySin = new double[1000];
                double[] dummyCos = new double[1000];
                double offset = random.NextDouble() * 10;

                for (int j = 0; j < 1000; j++)
                {
                    dummySin[j] = Math.Sin(offset + j * 0.1);
                    dummyCos[j] = Math.Cos(offset + j * 0.1);
                }

                // 2. 모델 객체 생성 (DoubleArrayHandler가 있으므로 double[]을 그냥 넣으면 됨!)
                var newData = new WeldingData(machineId, dummySin, dummyCos);

                // 3. 리포지토리의 Insert 호출
                await InsertWeldingDataAsync(newData);
            }
        }
    }
}
