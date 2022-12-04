using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IDispatchMedicineRepository
    {
        Response Create(DispatchMedicine droneRegistration);
        DispatchMedicine GetDispatchMedicationItemInformationByDroneId(int droneId);
        Response UpdateDispatchInformation(string dispatchCode,int droneId, string droneState, double batterPercentage);
        IEnumerable<DispatchMedicine> GetAllAvailableDronesForLoading();
    }
}
