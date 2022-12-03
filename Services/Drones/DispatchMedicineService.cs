using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Interfaces;

namespace Services.Drones
{
    public interface IDispatchMedicineService
    {
        Response DispatchMedicine(DispatchMedicine droneRegistration);
    }
    
    public class DispatchMedicineService: IDispatchMedicineService
    {
        private IUnitOfWork _unitOfWork;

        public DispatchMedicineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Response DispatchMedicine(DispatchMedicine droneRegistration)
        {
            using (var context = _unitOfWork.Create())
            {
                var result = context.Repositories.DispatchMedicineRepository.Create(droneRegistration);
                context.SaveChanges();
                return result;
            }
        }
    }
}
