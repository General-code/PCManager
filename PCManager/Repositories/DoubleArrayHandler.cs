using Dapper;
using System.Data;

namespace PCManager.Repositories
{
    public class DoubleArrayHandler : SqlMapper.TypeHandler<double[]>
    {
        public override double[]? Parse(object value)
        {
            if (value == null || value is DBNull) return null;

            // 데이터가 byte[]가 아닌 string이나 다른 형태로 들어올 경우를 대비
            byte[]? bytes = value as byte[];
            if (bytes == null) return null;

            var waves = new double[bytes.Length / sizeof(double)];
            Buffer.BlockCopy(bytes, 0, waves, 0, bytes.Length);
            return waves;
        }

        public override void SetValue(IDbDataParameter parameter, double[]? value)
        {
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                var bytes = new byte[value.Length * sizeof(double)];
                Buffer.BlockCopy(value, 0, bytes, 0, bytes.Length);

                // [중요] 값을 할당하고, 파라미터의 타입을 명확히 지정합니다.
                parameter.Value = bytes;

                // MySQL에서 BLOB으로 인식하도록 강제 설정
                if (parameter is MySqlConnector.MySqlParameter mySqlParam)
                {
                    mySqlParam.MySqlDbType = MySqlConnector.MySqlDbType.Blob;
                }
            }
        }
    }
}
