using Dapper;
using PCManager.Models;
using PCManager.Repositories;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace PCManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DBConnectionString = "UID=root;PWD=1234;Server=localhost;Port=3306;Database=exercise";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureDapper();
        }

        private void ConfigureDapper()
        {
            //User 클래스에 대한 매핑 예시
           var machineMap = new CustomPropertyTypeMap(
               typeof(Machine),
               (type, columnName) => type.GetProperties().FirstOrDefault(prop =>
                   prop.GetCustomAttributes(false).OfType<ColumnAttribute>()
                       .Any(attr => attr.Name == columnName))
           );

            var weldingMap = new CustomPropertyTypeMap(
                typeof(WeldingData),
                (type, columnName) => type.GetProperties().FirstOrDefault(prop =>
                prop.GetCustomAttributes(false).OfType<ColumnAttribute>()
                    .Any(attr => attr.Name == columnName))
                );

            Dapper.SqlMapper.SetTypeMap(typeof(Machine), machineMap);
            Dapper.SqlMapper.SetTypeMap(typeof(WeldingData), weldingMap);
            Dapper.SqlMapper.AddTypeHandler(new DoubleArrayHandler());
        }
    }
}

