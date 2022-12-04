using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Interfaces;
using static CommonEnum.CommonEnum;

namespace Services.Drones
{
    public interface IDroneService
    {
        Response Create(Drone drone);
        IEnumerable<Drone> GetAll();
        Drone GetDroneInformationByDroneId(int id);

    }
    public class DroneService: IDroneService
    {
        private IUnitOfWork _unitOfWork;

        public DroneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///  Description : This method is for registring drone item
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="drone"></param>
        /// <returns></returns>
        public Response Create(Drone drone)
        {
            using (var context = _unitOfWork.Create())
            {
                var result = context.Repositories.DronesRepository.Create(drone);
                context.SaveChanges();
                return result;
            }
        }

        /// <summary>
        ///  Description : This method is responsible for fetching All drones items
        ///  Author      : Fosyal Alam
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> GetAll()
        {
            List<Drone> _drones = new List<Drone>();
            try
            {
                using (var context = _unitOfWork.Create())
                {
                    var drones = context.Repositories.DronesRepository.GetAll();

                    if (drones.Count() > 0) 
                    {
                        foreach (var item in drones) 
                        {
                            var dispatchinformation = context.Repositories.DispatchMedicineRepository.GetDispatchMedicationItemInformationByDroneId(item.Id);

                            if (dispatchinformation != null || dispatchinformation.Id != 0)
                            {
                                int statevalue = dispatchinformation.DroneState;
                                var state = (DroneStateEnum)statevalue;
                                string droneState = state.ToString();
                                item.DroneState = droneState;
                                item.DroneBatteryPercentage = dispatchinformation.BatterCapacity;
                                _drones.Add(item);
                            }
                        }
                    }


                    return _drones;
                }
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        /// <summary>
        ///  Description : This method is for getting registered drone information by drone Id
        ///  Author      : Foysal Alam
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public Drone GetDroneInformationByDroneId(int droneId) 
        {
            try
            {
                using (var context = _unitOfWork.Create())
                {
                    var drone = context.Repositories.DronesRepository.GetDroneInformationByDroneId(droneId);
                    if (drone != null)
                    {
                        var dispatchInfo = context.Repositories.DispatchMedicineRepository.GetDispatchMedicationItemInformationByDroneId(drone.Id);
                        if (dispatchInfo != null)
                        {
                            int statevalue = dispatchInfo.DroneState;
                            var state = (DroneStateEnum)statevalue;
                            string droneState = state.ToString();
                            drone.DroneState = droneState;
                            drone.DroneBatteryPercentage = dispatchInfo.BatterCapacity;
                        }
                    }
                    return drone;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
