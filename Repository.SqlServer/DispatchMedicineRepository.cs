using Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.SqlHelper;
using static CommonEnum.CommonEnum;

namespace Repository.SqlServer
{
    public class DispatchMedicineRepository : Repository, IDispatchMedicineRepository
    {
        public DispatchMedicineRepository(SqlConnection context, SqlTransaction transaction)
        {
            this._context = context;
            this._transaction = transaction;
        }


        /// <summary>
        ///  Description : This method is responsible for save dispatching medications information 
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="dispatchMedicine"></param>
        /// <returns></returns>
        public Response Create(DispatchMedicine dispatchMedicine)
        {
            string _vMsg = string.Empty;
            Response response;
            SqlManager dbManager = new SqlManager();

            // State
            int statevalue = dispatchMedicine.DroneState;
            var state = (DroneStateEnum)statevalue;
            string droneState = state.ToString();



            var command = CreateCommand("SP_DISPATCH_MEDICINE");
            command.CommandType = CommandType.StoredProcedure;

            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 1, DbType.Int16));
            parameters.Add(dbManager.CreateParameter("@DroneId", dispatchMedicine.DroneId, DbType.Int32));
            parameters.Add(dbManager.CreateParameter("@DispatchStartDate", dispatchMedicine.DispatchStartDate, DbType.DateTime));
            parameters.Add(dbManager.CreateParameter("@DeliveryFrom", dispatchMedicine.DeliveryFrom, DbType.String));
            parameters.Add(dbManager.CreateParameter("@DeliveryTo", dispatchMedicine.DeliveryTo, DbType.String));
            parameters.Add(dbManager.CreateParameter("@DroneControlBy", dispatchMedicine.DroneControlBy, DbType.String));
            parameters.Add(dbManager.CreateParameter("@BatteryPercentage", dispatchMedicine.BatterCapacity, DbType.Double));
            parameters.Add(dbManager.CreateParameter("@DroneState", droneState, DbType.String));

            parameters.Add(dbManager.CreateParameter("@Msg", 500, null, DbType.String, ParameterDirection.Output));
            try
            {
                var result = dbManager.Insert(command, parameters.ToArray(), out _vMsg);
                //------------------- Parsing from return String ---------------------------/

                string[] _firstSplit = result.Split(" || ");
                string[] _secondSplit;
                response = new Response();
                foreach (string item in _firstSplit)
                {
                    _secondSplit = item.Split(":");
                    if (_secondSplit[0].Trim() == "Status")
                    {
                        response.Status = _secondSplit[1].Trim();
                    }
                    if (_secondSplit[0].Trim() == "Remarks")
                    {
                        response.Remarks = _secondSplit[1].Trim();
                    }
                    if (_secondSplit[0].Trim() == "Reference")
                    {
                        response.Reference = _secondSplit[1].Trim();
                    }
                }

                if (response.Status == "000")
                {
                    if (dispatchMedicine.Medications.Count > 0)
                    {
                        foreach (Medicine medicine in dispatchMedicine.Medications)
                        {
                            medicine.DispatchCode = response.Reference.ToString();
                            SaveDispatchMedicineInformation(medicine);
                        }
                    }

                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  Description : This method is for Savings Medications item information 
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="medicine"></param>
        public void SaveDispatchMedicineInformation(Medicine medicine)
        {

            SqlManager dbManager = new SqlManager();
            SqlCommand command = CreateCommand("SP_DISPATCH_MEDICINE");
            command.CommandType = CommandType.StoredProcedure;

            // Store Procedure Parameter Assign 
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 2, DbType.Int16));
            parameters.Add(dbManager.CreateParameter("@DispatchCode", medicine.DispatchCode, DbType.String));
            parameters.Add(dbManager.CreateParameter("@DroneId", medicine.DroneId, DbType.Int32));
            parameters.Add(dbManager.CreateParameter("@MedicineName", medicine.Name, DbType.String));
            parameters.Add(dbManager.CreateParameter("@MedicineWeight", medicine.Weight, DbType.Double));
            parameters.Add(dbManager.CreateParameter("@MedicineCode", medicine.Code, DbType.String));
            parameters.Add(dbManager.CreateParameter("@MedicineImage", medicine.Image, DbType.Binary));

            parameters.Add(dbManager.CreateParameter("@Msg", 500, null, DbType.String, ParameterDirection.Output));
            try
            {
                dbManager.InsertDetails(command, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  Description : This method is responsible for getting dispatch medications items by drone Id
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public DispatchMedicine GetDispatchMedicationItemInformationByDroneId(int droneId)
        {
            DispatchMedicine dispatchMedicine = new DispatchMedicine();
            SqlManager dbManager = new SqlManager();
            var command = CreateCommand("SP_DISPATCH_MEDICINE");
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 500;

            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 3, DbType.Int32));
            parameters.Add(dbManager.CreateParameter("@DroneId", droneId, DbType.Int16));
            parameters.Add(dbManager.CreateParameter("@Msg", 500, null, DbType.String, ParameterDirection.Output));
            var reader = dbManager.GetDataReader(command, parameters);

            try
            {
                while (reader.Read())
                {

                    dispatchMedicine.Id = (Convert.IsDBNull(reader["Id"])) ? 0 : Convert.ToInt32(reader["Id"]);
                    dispatchMedicine.DispatchCode = (Convert.IsDBNull(reader["DispatchCode"])) ? string.Empty : Convert.ToString(reader["DispatchCode"]);
                    dispatchMedicine.DroneId = (Convert.IsDBNull(reader["DroneId"])) ? 0 : Convert.ToInt32(reader["DroneId"]);
                    dispatchMedicine.DispatchStartDate = (Convert.IsDBNull(reader["DispatchStartDate"])) ? DateTime.MinValue : Convert.ToDateTime(reader["DispatchStartDate"]);
                    dispatchMedicine.DispatchComplateDate = (Convert.IsDBNull(reader["DispatchCompleteDate"])) ? DateTime.MinValue : Convert.ToDateTime(reader["DispatchCompleteDate"]);

                    dispatchMedicine.DeliveryFrom = (Convert.IsDBNull(reader["DeliveryFrom"])) ? string.Empty : Convert.ToString(reader["DeliveryFrom"]);
                    dispatchMedicine.DeliveryTo = (Convert.IsDBNull(reader["DeliveryTo"])) ? string.Empty : Convert.ToString(reader["DeliveryTo"]);
                    dispatchMedicine.DroneControlBy = (Convert.IsDBNull(reader["DroneControlBy"])) ? string.Empty : Convert.ToString(reader["DroneControlBy"]);

                    // Drone State
                    string _vDroneState = (Convert.IsDBNull(reader["DroneState"])) ? string.Empty : Convert.ToString(reader["DroneState"]);
                    DroneStateEnum droneStateEnum = (DroneStateEnum)Enum.Parse(typeof(DroneStateEnum), _vDroneState);
                    dispatchMedicine.DroneState = (Convert.IsDBNull(droneStateEnum) ? 0 : Convert.ToInt32(droneStateEnum));
                    dispatchMedicine.BatterCapacity = (Convert.IsDBNull(reader["BatteryPercentage"])) ? 0 : Convert.ToDouble(reader["BatteryPercentage"]);

                    dispatchMedicine.Medications = GetMedicationItemsListByDispatchCode(dispatchMedicine.DispatchCode);
                }

                return dispatchMedicine;
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
        ///  Description : This method is responsible for updating dispatching information along
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="droneState"></param>
        /// <param name="batteryPercentage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Response UpdateDispatchInformation(string dispatchCode, int droneId, string droneState, double batteryPercentage)
        {
            string _vMsg = string.Empty;
            SqlManager dbManager = new SqlManager();
            var command = CreateCommand("SP_DISPATCH_MEDICINE");
            command.CommandType = CommandType.StoredProcedure;

            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 5, DbType.Int32));
            parameters.Add(dbManager.CreateParameter("@DispatchCode", dispatchCode, DbType.String));
            parameters.Add(dbManager.CreateParameter("@DroneId", droneId, DbType.String));
            parameters.Add(dbManager.CreateParameter("@DroneState", droneState, DbType.String));
            parameters.Add(dbManager.CreateParameter("@BatteryPercentage", batteryPercentage, DbType.Double));

            parameters.Add(dbManager.CreateParameter("@Msg", 500, null, DbType.String, ParameterDirection.Output));

            try
            {
                var result = dbManager.Update(command, parameters.ToArray(), out _vMsg);
                //------------------- Parsing from return String ---------------------------/

                string[] _firstSplit = result.Split(" || ");
                string[] _secondSplit;
                Response response = new Response();
                foreach (string item in _firstSplit)
                {
                    _secondSplit = item.Split(":");
                    if (_secondSplit[0].Trim() == "Status")
                    {
                        response.Status = _secondSplit[1].Trim();
                    }
                    if (_secondSplit[0].Trim() == "Remarks")
                    {
                        response.Remarks = _secondSplit[1].Trim();
                    }
                }
                //---------------------------------------------------------------------------/

                return response; // Resutl will be return 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  Description : This method is responsible for getting medication items list by Dispatch Code
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="dispatchCode"></param>
        /// <returns></returns>
        public List<Medicine> GetMedicationItemsListByDispatchCode(string dispatchCode)
        {
            List<Medicine> medicines = new List<Medicine>();
            Medicine medicine;

            SqlManager dbManager = new SqlManager();
            var command = CreateCommand("SP_DISPATCH_MEDICINE");
            command.CommandType = CommandType.StoredProcedure;


            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 4, DbType.Int32));
            parameters.Add(dbManager.CreateParameter("@DispatchCode", dispatchCode, DbType.String));
            parameters.Add(dbManager.CreateParameter("@Msg", 500, null, DbType.String, ParameterDirection.Output));
            var rdr = dbManager.GetDataReader(command, parameters);

            try
            {
                while (rdr.Read())
                {
                    medicine = new Medicine();
                    medicine.Id = (Convert.IsDBNull(rdr["Id"])) ? 0 : Convert.ToInt32(rdr["Id"]);
                    medicine.DispatchCode = (Convert.IsDBNull(rdr["DispatchCode"])) ? string.Empty : Convert.ToString(rdr["DispatchCode"]);
                    medicine.Name = (Convert.IsDBNull(rdr["MedicineName"])) ? string.Empty : Convert.ToString(rdr["MedicineName"]);
                    medicine.Weight = (Convert.IsDBNull(rdr["MedicineWeight"])) ? 0 : Convert.ToDouble(rdr["MedicineWeight"]);
                    medicine.Code = (Convert.IsDBNull(rdr["MedicineCode"])) ? string.Empty : Convert.ToString(rdr["MedicineCode"]);

                    byte[] imagefileStream = new byte[0];
                    medicine.Image = (Convert.IsDBNull(rdr["MedicineImage"])) ? imagefileStream : Encoding.ASCII.GetBytes(Convert.ToString(rdr["MedicineImage"]));

                    medicines.Add(medicine);
                }

                return medicines;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rdr.Close();
            }
        }

        /// <summary>
        /// Description : This method is responsible for fetching all available drones for LOADING state
        /// Autrho      : Foysal Alam
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DispatchMedicine> GetAllAvailableDronesForLoading() 
        {
            List<DispatchMedicine> dispatchMedicines = new List<DispatchMedicine>();
            DispatchMedicine dispatchMedicine;

            SqlManager dbManager = new SqlManager();
            var command = CreateCommand("SP_DISPATCH_MEDICINE");
            command.CommandType = CommandType.StoredProcedure;


            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 6, DbType.Int32));
            parameters.Add(dbManager.CreateParameter("@DroneState", "IDLE", DbType.String));
            parameters.Add(dbManager.CreateParameter("@Msg", 500, null, DbType.String, ParameterDirection.Output));
            var reader = dbManager.GetDataReader(command, parameters);

            try
            {
                while (reader.Read())
                {
                    dispatchMedicine = new DispatchMedicine();
                    dispatchMedicine.Id = (Convert.IsDBNull(reader["Id"])) ? 0 : Convert.ToInt32(reader["Id"]);
                    dispatchMedicine.DispatchCode = (Convert.IsDBNull(reader["DispatchCode"])) ? string.Empty : Convert.ToString(reader["DispatchCode"]);
                    dispatchMedicine.DroneId = (Convert.IsDBNull(reader["DroneId"])) ? 0 : Convert.ToInt32(reader["DroneId"]);
                    dispatchMedicine.DispatchStartDate = (Convert.IsDBNull(reader["DispatchStartDate"])) ? DateTime.MinValue : Convert.ToDateTime(reader["DispatchStartDate"]);
                    dispatchMedicine.DispatchComplateDate = (Convert.IsDBNull(reader["DispatchCompleteDate"])) ? DateTime.MinValue : Convert.ToDateTime(reader["DispatchCompleteDate"]);

                    dispatchMedicine.DeliveryFrom = (Convert.IsDBNull(reader["DeliveryFrom"])) ? string.Empty : Convert.ToString(reader["DeliveryFrom"]);
                    dispatchMedicine.DeliveryTo = (Convert.IsDBNull(reader["DeliveryTo"])) ? string.Empty : Convert.ToString(reader["DeliveryTo"]);
                    dispatchMedicine.DroneControlBy = (Convert.IsDBNull(reader["DroneControlBy"])) ? string.Empty : Convert.ToString(reader["DroneControlBy"]);

                    // Drone State
                    string _vDroneState = (Convert.IsDBNull(reader["DroneState"])) ? string.Empty : Convert.ToString(reader["DroneState"]);
                    DroneStateEnum droneStateEnum = (DroneStateEnum)Enum.Parse(typeof(DroneStateEnum), _vDroneState);
                    dispatchMedicine.DroneState = (Convert.IsDBNull(droneStateEnum) ? 0 : Convert.ToInt32(droneStateEnum));
                    dispatchMedicine.BatterCapacity = (Convert.IsDBNull(reader["BatteryPercentage"])) ? 0 : Convert.ToDouble(reader["BatteryPercentage"]);

                    dispatchMedicine.Medications = GetMedicationItemsListByDispatchCode(dispatchMedicine.DispatchCode);


                    dispatchMedicines.Add(dispatchMedicine);
                }

                return dispatchMedicines;
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
