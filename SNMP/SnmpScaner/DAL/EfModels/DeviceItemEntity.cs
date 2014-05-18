﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EfModels
{
    public class DeviceItemEntity
    {
        [Key]
        public long IdDeviceItemEntity { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Oid { get; set; }

        public int IdModel { get; set; }

        [ForeignKey("IdModel")]
        public DeviceModel Model { get; set; }

        //public virtual ICollection<DeviceItemHistory> ItemHistory { get; set; }

        //public virtual ICollection<EmailNotification> EmailNotifications { get; set; }

        //public virtual ICollection<PhoneNotification> PhoneNotifications { get; set; }

        public virtual ICollection<DevicesItems> DevicesItems { get; set; }
        
    }
}