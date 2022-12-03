using CommonEnum;
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
using static CommonEnum.CommonEnum;

namespace Repository.SqlServer
{
    public class DroneRepository : Repository, IDroneRepository
    {
        public DroneRepository(SqlConnection context, SqlTransaction transaction)
        {
            this._context = context;
            this._transaction = transaction;
        }

        public Response Create(Drone drone)
        {
            string _vMsg = string.Empty;
            Response droneResponse;
            SqlManager dbManager = new SqlManager();

            var command = CreateCommand("SP_DRONE");
            command.CommandType = CommandType.StoredProcedure;

            // Model

            int modelvalue = drone.Model;
            var model = (DroneModelEnum)modelvalue;
            string droneModel = model.ToString();


            // State
            int statevalue = drone.DroneState;
            var state = (DroneStateEnum)statevalue;
            string droneState = state.ToString();


            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 1, DbType.Int16)); // 1 is for Create Panel at Store procedure
            parameters.Add(dbManager.CreateParameter("@SerialNumber", Convert.ToString(drone.SerialNumber), DbType.String));
            parameters.Add(dbManager.CreateParameter("@Model", droneModel, DbType.String));
            parameters.Add(dbManager.CreateParameter("@Weight", drone.Weight, DbType.Double));
            parameters.Add(dbManager.CreateParameter("@BatteryCapacity", drone.BatterCapacity, DbType.Double));
            parameters.Add(dbManager.CreateParameter("@State", droneState, DbType.String));
            parameters.Add(dbManager.CreateParameter("@msg", 500, null, DbType.String, ParameterDirection.Output));

            try
            {
                var result = dbManager.Insert(command, parameters.ToArray(), out _vMsg);
                //------------------- Parsing from return String ---------------------------/

                string[] _firstSplit = result.Split(" || ");
                string[] _secondSplit;
                droneResponse = new Response();
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

    }
}
