﻿using DAL.EfModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IDevicesItemsRepo
    {
        void Edit(DevicesItems entity);
        IEnumerable<DevicesItems> GetByDeviceId(long id);

    }
}