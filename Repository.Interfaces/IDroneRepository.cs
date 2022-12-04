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
        Response Create(Drone drone);
        IEnumerable<Drone> GetAll();
        Drone GetDroneInformationByDroneId(int droneId);
    }
}
