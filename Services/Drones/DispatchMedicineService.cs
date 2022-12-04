using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Interfaces;
using static CommonEnum.CommonEnum;
using static System.Net.WebRequestMethods;

namespace Services.Drones
{
    public interface IDispatchMedicineService
    {
        Response DispatchMedicine(DispatchMedicine droneRegistration);
        Response UpdateDispatchInformation(string dispatchCode, int droneId, int droneState, double batteryPercentage);
    }

    public class DispatchMedicineService : IDispatchMedicineService
    {
        private IUnitOfWork _unitOfWork;

        public DispatchMedicineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///  Description : This method is responsible for dispatching medication items 
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="dispatchMedicine"></param>
        /// <returns></returns>
        public Response DispatchMedicine(DispatchMedicine dispatchMedicine)
        {

            using (var context = _unitOfWork.Create())
            {

                // Get the Drone Information First to get weight limit 
                var drone = context.Repositories.DronesRepository.GetDroneInformationByDroneId(dispatchMedicine.DroneId);
                if (drone == null || drone.Id == 0)
                {
                    Response response = new Response();
                    response.Status = Convert.ToString(HttpStatusCode.NotFound);
                    response.Remarks = "Drone not found";
                    return response;
                }
                else
                {
                    if (IsProvidedDroneIsIdle(drone.Id))
                    {
                        // Checking Medications weight 
                        // Med weight < drone weight
                        double medicineWeightInTotal = 0;
                        if (dispatchMedicine.Medications.Count > 0)
                        {
                            medicineWeightInTotal = GetMedicationsWeightInTotal(dispatchMedicine.Medications);
                        }
                        else
                        {
                            Response response = new Response();
                            response.Status = String.Empty;
                            response.Remarks = "Medications item not found";
                            return response;
                        }

                        if (medicineWeightInTotal < drone.Weight)
                        {

                            var result = context.Repositories.DispatchMedicineRepository.Create(dispatchMedicine);
                            context.SaveChanges();
                            return result;
                        }
                        else
                        {
                            Response response = new Response();
                            response.Status = String.Empty;
                            response.Remarks = "Medications item weight cannot be greater than drone weight";
                            return response;
                        }

                    }
                    else
                    {
                        Response response = new Response();
                        response.Status = String.Empty;
                        var dispatchInformation = GetDispatchMedicationsInformationByDroneId(drone.Id);

                        int statevalue = dispatchInformation.DroneState;
                        var state = (DroneStateEnum)statevalue;
                        string droneState = state.ToString();
                        response.Remarks = "Operation Deny : Drone with Serial Number : " + drone.SerialNumber.ToString() + " is in state : " + droneState + "";
                        return response;
                    }
                }
            }
        }

        /// <summary>
        ///  Description : This method is for getting Dispatching medications item information by droneId
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public DispatchMedicine GetDispatchMedicationsInformationByDroneId(int droneId)
        {
            DispatchMedicine dispatchMedicine = new DispatchMedicine();
            try
            {
                using (var context = _unitOfWork.Create())
                {
                    dispatchMedicine = context.Repositories.DispatchMedicineRepository.GetDispatchMedicationItemInformationByDroneId(droneId);
                    return dispatchMedicine;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  Description : This method is for checking provided drone is Idle or Not
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public bool IsProvidedDroneIsIdle(int droneId)
        {
            bool isIdle = false;
            try
            {
                var medications = GetDispatchMedicationsInformationByDroneId(droneId);
                if (medications != null || medications.Id > 0)
                {
                    switch (medications.DroneState)
                    {

                        case 1:          // 1: IDLE
                            isIdle = true;
                            break;
                        case 2:          // 2: LOADING 
                            isIdle = false;
                            break;
                        case 3:          // 3: LOADED 
                            isIdle = false;
                            break;
                        case 4:          // 4: DELIVERING  
                            isIdle = false;
                            break;
                        case 5:          // 5: DELIVERED  
                            isIdle = false;
                            break;
                        case 6:          // 6: RETURNING  
                            isIdle = false;
                            break;
                        default:
                            isIdle = true;
                            break;
                    }

                }
                return isIdle;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        ///  Description : This method is responsible for returning total medications item weight
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="medicines"></param>
        /// <returns></returns>
        public double GetMedicationsWeightInTotal(List<Medicine> medicines)
        {
            double weight = 0;
            try
            {
                if (medicines.Count > 0)
                {
                    foreach (var item in medicines)
                    {
                        weight += item.Weight;
                    }
                }
                return weight;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        ///  Description : this method is for updating drone dispatch information 
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="droneState"></param>
        /// <param name="batteryPercentage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Response UpdateDispatchInformation(string dispatchCode, int droneId, int droneState, double batteryPercentage)
        {
            using (var context = _unitOfWork.Create())
            {
                var result = context.Repositories.DispatchMedicineRepository.UpdateDispatchInformation(dispatchCode,droneId, droneState,batteryPercentage);
                // Confirm changes
                context.SaveChanges();
                return result;
            }
        }
    }
}
