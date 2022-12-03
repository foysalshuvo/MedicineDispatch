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

namespace Repository.SqlServer
{
    public class DispatchMedicineRepository : Repository, IDispatchMedicineRepository
    {
        public DispatchMedicineRepository(SqlConnection context, SqlTransaction transaction)
        {
            this._context = context;
            this._transaction = transaction;
        }

        public Response Create(DispatchMedicine dispatchMedicine)
        {
            string _vMsg = string.Empty;
            Response response;
            SqlManager dbManager = new SqlManager();

            var command = CreateCommand("SP_DISPATCH_MEDICINE");
            command.CommandType = CommandType.StoredProcedure;

            // Store Procedure Parameter Assign 
            var parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 1, DbType.Int16));
            parameters.Add(dbManager.CreateParameter("@DroneId", dispatchMedicine.DroneId, DbType.Int32));
            parameters.Add(dbManager.CreateParameter("@DispatchDate", dispatchMedicine.DispatchDate, DbType.DateTime));
            parameters.Add(dbManager.CreateParameter("@DeliveryFrom", dispatchMedicine.DeliveryFrom, DbType.String));
            parameters.Add(dbManager.CreateParameter("@DeliveryTo", dispatchMedicine.DeliveryTo, DbType.String));
            parameters.Add(dbManager.CreateParameter("@DroneControlBy", dispatchMedicine.DroneControlBy, DbType.String));
            parameters.Add(dbManager.CreateParameter("@DispatchStatus", dispatchMedicine.DispatchStatus, DbType.String));
            parameters.Add(dbManager.CreateParameter("@Msg", 500, null, DbType.String, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("@DispatchID", 20, null, DbType.Int32, ParameterDirection.Output));
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

        public void SaveDispatchMedicineInformation(Medicine medicine)
        {

            SqlManager dbManager = new SqlManager();
            SqlCommand command = CreateCommand("SP_DISPATCH_MEDICINE");
            command.CommandType = CommandType.StoredProcedure;

            // Store Procedure Parameter Assign 
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(dbManager.CreateParameter("@IntQuery", 2, DbType.Int16));
            parameters.Add(dbManager.CreateParameter("@DispatchCode", medicine.DispatchCode, DbType.String));
            parameters.Add(dbManager.CreateParameter("@MedicineName", medicine.Name, DbType.String));
            parameters.Add(dbManager.CreateParameter("@MedicineWeight", medicine.Weight, DbType.Double));
            parameters.Add(dbManager.CreateParameter("@MedicineCode", medicine.Code, DbType.String));
            parameters.Add(dbManager.CreateParameter("@MedicineImage", medicine.Image, DbType.Byte));
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

    }
}
