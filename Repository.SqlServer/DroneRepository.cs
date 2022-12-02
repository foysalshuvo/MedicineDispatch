using Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.SqlHelper;

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
            string _vMsg = string.Empty;
            DroneResponse droneResponse;
            SqlManager dbManager = new SqlManager();

            var command = CreateCommand("SP_DRONE");
            command.CommandType = CommandType.StoredProcedure;

            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 1, DbType.Int16)); // 1 is for Create Panel at Store procedure
            parameters.Add(dbManager.CreateParameter("@SerialNumber", Convert.ToString(drone.SerialNumber), DbType.String));
            parameters.Add(dbManager.CreateParameter("@Model", Convert.ToString(drone.Model), DbType.String));
            parameters.Add(dbManager.CreateParameter("@Weight", drone.Weight, DbType.Double));
            parameters.Add(dbManager.CreateParameter("@BatteryCapacity", drone.Battery, DbType.Double));
            parameters.Add(dbManager.CreateParameter("@State", drone.State, DbType.String));
            parameters.Add(dbManager.CreateParameter("@IsActive", drone.IsActive, DbType.Boolean));
            
            parameters.Add(dbManager.CreateParameter("@msg", 500, null, DbType.String, ParameterDirection.Output));

            try
            {
                var result = dbManager.Insert(command, parameters.ToArray(), out _vMsg);
                //------------------- Parsing from return String ---------------------------/

                string[] _firstSplit = result.Split(" || ");
                string[] _secondSplit;
                droneResponse = new DroneResponse();
                foreach (string item in _firstSplit)
                {
                    _secondSplit = item.Split(":");
                    if (_secondSplit[0].Trim() == "Status")
                    {
                        droneResponse.Status = _secondSplit[1].Trim();
                    }
                    if (_secondSplit[0].Trim() == "Remarks")
                    {
                        droneResponse.Remarks = _secondSplit[1].Trim();
                    }
                }
                //---------------------------------------------------------------------------/
                return droneResponse; // Resutl will be return approval panel response object
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
