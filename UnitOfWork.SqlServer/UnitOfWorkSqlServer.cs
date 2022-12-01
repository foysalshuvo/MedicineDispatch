using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Interfaces;

namespace UnitOfWork.SqlServer
{
    public class UnitOfWorkSqlServer: IUnitOfWork
    {
        private readonly IConfiguration _configuration;

        public UnitOfWorkSqlServer(IConfiguration configuration = null)
        {
            _configuration = configuration;
        }


        /// <summary>
        ///  Description : Database connectivity string would be read out in here and forward to SqlServerAdapter Class
        ///  Author      : Foysal Alam
        ///  Creation    : 28 Feb,2020
        /// </summary>
        /// <returns></returns>
        public IUnitOfWorkAdapter Create()
        {
            try
            {
               
                // This will read data connection string from  appsettings.Development.json
                var connectionString = _configuration.GetValue<string>("SqlConnectionString");
                return new UnitOfWorkSqlServerAdapter(connectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
