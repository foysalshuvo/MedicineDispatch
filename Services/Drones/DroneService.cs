using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Interfaces;

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

        public Response Create(Drone drone)
        {
            using (var context = _unitOfWork.Create())
            {
                var result = context.Repositories.DronesRepository.Create(drone);
                context.SaveChanges();
                return result;
            }
        }

        public IEnumerable<Drone> GetAll()
        {
            try
            {
                using (var context = _unitOfWork.Create())
                {
                    var result = context.Repositories.DronesRepository.GetAll();
                    return result;
                }
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        public Drone GetDroneInformationByDroneId(int droneId) 
        {
            try
            {
                using (var context = _unitOfWork.Create())
                {
                    var result = context.Repositories.DronesRepository.GetDroneInformationByDroneId(droneId);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
