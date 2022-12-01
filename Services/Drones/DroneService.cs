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
        DroneResponse Create(Drone drone);
    }
    public class DroneService: IDroneService
    {
        private IUnitOfWork _unitOfWork;

        public DroneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DroneResponse Create(Drone drone)
        {
            using (var context = _unitOfWork.Create())
            {
                var result = context.Repositories.DronesRepository.Create(drone);
                context.SaveChanges();
                return result;
            }
        }
    }
}
