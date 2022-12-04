using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Interfaces;
using static System.Net.WebRequestMethods;

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

        public Response DispatchMedicine(DispatchMedicine dispatchMedicine)
        {
            bool isDispatchInformationValid = false;

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
                    // Checking Medinine weight 
                    // Med weight < drone weight
                    double medicineWeightInTotal = 0;
                    if (dispatchMedicine.Medications.Count > 0)
                    {
                        medicineWeightInTotal = GetMedicineWeightInTotal(dispatchMedicine.Medications);
                    }
                    else 
                    {
                        Response response = new Response();
                        response.Status = String.Empty;
                        response.Remarks = "Medicines are not found";
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
                        response.Remarks = "Medicine weight cannot be greater than drone weight";
                        return response;
                    }
                }

            }
        }

        public double GetMedicineWeightInTotal(List<Medicine> medicines) 
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
    }
}
