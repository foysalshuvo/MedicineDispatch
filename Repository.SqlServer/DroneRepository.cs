using Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.SqlServer
{
    public class DroneRepository : Repository, IDroneRepository
    {
        public DroneRepository(SqlConnection context, SqlTransaction transaction)
        {
            this._context = context;
            this._transaction = transaction;
        }

        public DroneResponse Create(Drone drone)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Drone> GetAllDrones()
        {
            throw new NotImplementedException();
        }

        public Drone GetDroneInfoById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
