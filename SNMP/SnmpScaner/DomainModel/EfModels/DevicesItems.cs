﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EfModels
{
    public class DevicesItems
    {
        //[Key, Column(Order = 0)]
        //public int IdDevicesItems { get; set; }
        [Key, Column(Order = 1)]
        public long IdDeviceEntity { get; set; }
        [Key, Column(Order = 2)]
        public long IdDeviceItemEntity { get; set; }

        public int DeltaT { get; set; }

        [ForeignKey("IdDeviceEntity")]
        public virtual DeviceEntity DeviceEntity { get; set; }

        [ForeignKey("IdDeviceItemEntity")]
        public virtual DeviceItemEntity DeviceItemEntity { get; set; }

        public virtual ICollection<DeviceItemHistory> ItemHistory { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}