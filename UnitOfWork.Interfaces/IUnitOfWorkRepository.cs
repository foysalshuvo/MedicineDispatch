﻿using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork.Interfaces
{
    public interface IUnitOfWorkRepository
    {
        IDroneRepository DronesRepository { get; }
        IDispatchMedicineRepository DispatchMedicineRepository { get; }
    }
}
