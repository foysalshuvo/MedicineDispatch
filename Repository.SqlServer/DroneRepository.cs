using CommonEnum;
using Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
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

        /// <summary>
        ///  Description : This method is for register drone
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="drone"></param>
        /// <returns></returns>
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


            

            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 1, DbType.Int16)); // 1 is for Create Panel at Store procedure
            parameters.Add(dbManager.CreateParameter("@SerialNumber", Convert.ToString(drone.SerialNumber), DbType.String));
            parameters.Add(dbManager.CreateParameter("@Model", droneModel, DbType.String));
            parameters.Add(dbManager.CreateParameter("@Weight", drone.Weight, DbType.Double));
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


        /// <summary>
        ///  Description : This method is for Getting All drones information list
        ///  Author      : Foysal Alam
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> GetAll()
        {
            List<Drone> drones = new List<Drone>();
            Drone drone;
            SqlManager dbManager = new SqlManager();
            var command = CreateCommand("SP_DRONE");
            command.CommandType = CommandType.StoredProcedure;


            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 4, DbType.String));
            parameters.Add(dbManager.CreateParameter("@msg", 500, null, DbType.String, ParameterDirection.Output));
            var reader = dbManager.GetDataReader(command, parameters);

            try
            {
                while (reader.Read())
                {
                    drone = new Drone();
                    drone.Id = (Convert.IsDBNull(reader["Id"])) ? 0 : Convert.ToInt32(reader["Id"]);
                    drone.SerialNumber = (Convert.IsDBNull(reader["SerialNumber"])) ? string.Empty : Convert.ToString(reader["SerialNumber"]);

                    // Model 
                    string _vModel = (Convert.IsDBNull(reader["Model"])) ? string.Empty : Convert.ToString(reader["Model"]);
                    DroneModelEnum droneModelEnum = (DroneModelEnum)Enum.Parse(typeof(DroneModelEnum), _vModel);
                    drone.Model = (Convert.IsDBNull(droneModelEnum) ? 0 : Convert.ToInt32(droneModelEnum));

                    drone.Weight = (Convert.IsDBNull(reader["Weight"])) ? 0 : Convert.ToDouble(reader["Weight"]);
                 
                    drone.RegistrationDate = (Convert.IsDBNull(reader["RegistrationDate"])) ? DateTime.MinValue : Convert.ToDateTime(reader["RegistrationDate"]);
                    drones.Add(drone);
                }

                return drones;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Description : This method is for getting drone information by drone Id
        /// Author      : Foysal Alam
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public Drone GetDroneInformationByDroneId(int droneId)
        {
            Drone drone = new Drone();
            SqlManager dbManager = new SqlManager();
            var command = CreateCommand("SP_DRONE");
            command.CommandType = CommandType.StoredProcedure;


            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 2, DbType.String));
            parameters.Add(dbManager.CreateParameter("@Id", droneId, DbType.Int32));
            parameters.Add(dbManager.CreateParameter("@msg", 500, null, DbType.String, ParameterDirection.Output));
            var reader = dbManager.GetDataReader(command, parameters);

            try
            {
                while (reader.Read())
                {
                    drone.Id = (Convert.IsDBNull(reader["Id"])) ? 0 : Convert.ToInt32(reader["Id"]);
                    drone.SerialNumber = (Convert.IsDBNull(reader["SerialNumber"])) ? string.Empty : Convert.ToString(reader["SerialNumber"]);

                    string _vModel = (Convert.IsDBNull(reader["Model"])) ? string.Empty : Convert.ToString(reader["Model"]);
                    DroneModelEnum droneModelEnum = (DroneModelEnum)Enum.Parse(typeof(DroneModelEnum), _vModel);
                    drone.Model = (Convert.IsDBNull(droneModelEnum) ? 0 : Convert.ToInt32(droneModelEnum));

                    drone.Weight = (Convert.IsDBNull(reader["Weight"])) ? 0 : Convert.ToDouble(reader["Weight"]);
                    drone.RegistrationDate = (Convert.IsDBNull(reader["RegistrationDate"])) ? DateTime.MinValue : Convert.ToDateTime(reader["RegistrationDate"]);
                  
                }

                return drone;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
            }
        }

    }
}
