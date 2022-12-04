using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        IEnumerable<Drone> GetAllAvailableDronesForLoading();

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
                            isIdle = true;
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

                // Prevent Loding State if battery level below 25%
                var dispatchInformation = context.Repositories.DispatchMedicineRepository.GetDispatchMedicationItemInformationByDroneId(droneId);
                if (dispatchInformation != null || dispatchInformation.Id != 0)
                {
                    bool isLoadingState = false;
                    switch (droneState)
                    {
                        case 2:          // 2:LOADING 
                            isLoadingState = true;
                            break;
                        default:
                            isLoadingState = false;
                            break;
                    }

                    if (dispatchInformation.BatterCapacity < 25 && isLoadingState == true)
                    {

                        Response response = new Response();
                        response.Status = String.Empty;

                        int statevalue = dispatchInformation.DroneState;
                        var _state = (DroneStateEnum)statevalue;
                        string _droneState = _state.ToString();
                        response.Remarks = "Drone battery percentage is below 25% and LOADING is not permitable";
                        return response;
                    }
                    else
                    {

                        // Enum=>String (Drone State)
                        var state = (DroneStateEnum)droneState;
                        string strdroneState = state.ToString();

                        var result = context.Repositories.DispatchMedicineRepository.UpdateDispatchInformation(dispatchCode, droneId, strdroneState, batteryPercentage);
                        // Confirm changes
                        context.SaveChanges();
                        return result;
                    }
                }
                else
                {

                    Response response = new Response();
                    response.Status = Convert.ToString(HttpStatusCode.BadRequest);
                    response.Remarks = "Dispatch medication information not found";
                    return response;
                }

            }
        }


        /// <summary>
        ///  Description : This method is for getting available drones for loading purpose
        ///  Author      : Foysal Alam
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> GetAllAvailableDronesForLoading()
        {
            List<Drone> drones = new List<Drone>();
            Drone drone;
            try
            {
                using (var context = _unitOfWork.Create())
                {
                    var availableItemForLoading = context.Repositories.DispatchMedicineRepository.GetAllAvailableDronesForLoading();

                    if (availableItemForLoading != null)
                    {
                        foreach (var item in availableItemForLoading)
                        {
                            drone = new Drone();
                            drone = context.Repositories.DronesRepository.GetDroneInformationByDroneId(item.DroneId);

                            int statevalue = item.DroneState;
                            var state = (DroneStateEnum)statevalue;
                            string droneState = state.ToString();
                            drone.DroneState = droneState;
                            drone.DroneBatteryPercentage = item.BatterCapacity;

                            drones.Add(drone);
                        }
                    }

                    return drones;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
