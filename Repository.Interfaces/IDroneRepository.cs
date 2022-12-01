using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IDroneRepository
    {
        IEnumerable<Drone> GetAllDrones();
        Drone GetDroneInfoById(int id);
        DroneResponse Create(Drone drone);
     
    }
}
