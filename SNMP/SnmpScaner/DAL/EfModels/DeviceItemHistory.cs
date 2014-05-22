﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EfModels
{
    [Table("DeviceItemsHistory")]
    public class DeviceItemHistory
    {
        [Key]
        public int IdItemHistory { get; set; }

        [StringLength(100)]
        public string Value { get; set; }

        public DateTime Timestamp { get; set; }

        public long IdDevicesItems { get; set; }

        [ForeignKey("IdDevicesItems")]
        public DevicesItems DevicesItems { get; set; }


    }
}
