using Repository.Interfaces;
using Repository.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Interfaces;

namespace UnitOfWork.SqlServer
{
    public class UnitOfWorkSqlServerRepository: IUnitOfWorkRepository
    {
        public IDroneRepository DronesRepository { get; }
        public IDispatchMedicineRepository DispatchMedicineRepository { get; }
        public UnitOfWorkSqlServerRepository(SqlConnection context, SqlTransaction transaction)
        {
            DronesRepository = new DroneRepository(context, transaction);
            DispatchMedicineRepository = new DispatchMedicineRepository(context, transaction); 
        }
    }
}
