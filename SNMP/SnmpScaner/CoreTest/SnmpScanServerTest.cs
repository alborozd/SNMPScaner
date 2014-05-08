﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Core;
using DomainModel;
using DomainModel.Interfaces;
using Lextm.SharpSnmpLib;
using Moq;
using NUnit.Framework;
using StructureMap;

namespace CoreTest
{
	[TestFixture]
	public class SnmpScanServerTest
	{
		[Test]
		public void Test()
		{
			var server = new SnmpScanServer();
			Assert.DoesNotThrow(server.Run);
		}

		[Test]
		public void TestAsync()
		{
			var server = new SnmpScanServer();
			Assert.DoesNotThrow(server.RunAsync);
		}

		[Test]
		public void TestInit()
		{
			var devices = new[]
			{
				new Device
				{
					Id = 1,
					Ip = Dns.GetHostAddresses("demo.snmplabs.com")[0],
					Port = 161,
					Timeout = 6000,
					VersionCode = VersionCode.V1,
					Items = new List<DeviceItem>
					{
						new DeviceItem{ Oid = new ObjectIdentifier(".1.3.6.1.2.1.1.1.0")},
						new DeviceItem {Oid = new ObjectIdentifier(".1.3.6.1.2.1.1.3.0")},
						new DeviceItem {Oid = new ObjectIdentifier(".1.3.6.1.2.1.1.5.0")}
					}
				}
			};

			ObjectFactory.Configure(item =>item.For<IConfigRepo>().Add(Mock.Of<IConfigRepo>(m => m.GetAllDevices() == devices)));
			
			var mock = new Mock<IHistoryRepo>();
			mock.Setup(x => x.Save(It.Is<long>(v => v != 0), It.IsAny<ISnmpData>(), It.IsAny<DateTime>()));
			ObjectFactory.Configure(item => item.For<IHistoryRepo>().Add(mock.Object));

			var server = new SnmpScanServer();
			Thread.Sleep(6000);
			var values = server.GetAllValues();
			
			Assert.AreEqual(values.Count(), devices.SelectMany(d=>d.Items).Count());
			Assert.AreEqual(values.First(i => i.Oid.ToString() == ".1.3.6.1.2.1.1.1.0").Value.ToString(), "SunOS zeus.snmplabs.com 4.1.3_U1 1 sun4m");
			Assert.AreEqual(values.First(i => i.Oid.ToString() == ".1.3.6.1.2.1.1.5.0").Value.ToString(), "zeus.snmplabs.com");
		}


		/// <summary>
		/// If local emulator not installed, then will fail
		/// </summary>
		[Test]
		public void TestLocalEmulator()
		{
			#region Init
			var devices = new[]
			{
				new Device
				{
					Id = 1,
					Ip = IPAddress.Parse("127.0.0.1"),
					Port = 162,
					Timeout = 6000,
					VersionCode = VersionCode.V1,
					Items = new List<DeviceItem>
					{
						new DeviceItem
						{
							Id = 1,
							Oid = new ObjectIdentifier(".1.3.6.1.2.1.1.3.0"), //TimeTicks
							Timestamp = DateTime.MinValue,
							Value = null,
							TimeDelta = 2,
							ValueDelta = new TimeTicks(123)
						},
						new DeviceItem
						{
							Id = 2,
							Oid = new ObjectIdentifier(".1.3.6.1.2.1.1.9.1.2.1"), //Oid
							Timestamp = DateTime.MinValue,
							Value = null,
							TimeDelta = 4,
							ValueDelta = null
						},
						new DeviceItem
						{
							Id = 3,
							Oid = new ObjectIdentifier(".1.3.6.1.2.1.1.9.1.3.800"), //String Invalid OID
							Timestamp = DateTime.MinValue,
							Value = null,
							TimeDelta = 6,
							ValueDelta = null
						},
						new DeviceItem
						{
							Id = 4,
							Oid = new ObjectIdentifier(".1.3.6.1.2.1.2.2.1.1.110"), //INTEGER	Invalid OID
							Timestamp = DateTime.MinValue,
							Value = null,
							TimeDelta = 60,
							ValueDelta = new Integer32(10)
						},
						new DeviceItem
						{
							Id = 5,
							Oid = new ObjectIdentifier(".1.3.6.1.2.1.2.2.1.5.16"), //Gauge32: 	
							Timestamp = DateTime.MinValue,
							Value = null,
							TimeDelta = 6,
							ValueDelta = new Gauge32(22)
						},
						new DeviceItem
						{
							Id = 6,
							Oid = new ObjectIdentifier(".1.3.6.1.2.1.2.2.1.12.2"), //Counter32 	
							Timestamp = DateTime.MinValue,
							Value = null,
							TimeDelta = 2,
							ValueDelta = new Counter32(10)
						},
						//new DeviceItem
						//{
						//	Id = 7,
						//	Oid = new ObjectIdentifier(".1.3.6.1.2.1.31.1.1.1.6.1"), //Counter64:  	
						//	Timestamp = DateTime.MinValue,
						//	Value = null,
						//	TimeDelta = 2,
						//	ValueDelta = new Counter64(100)
						//},
					}
				}
			};
			var subscriptions = new[]
			{
				new SubscriptionItem(devices[0].Items[0]){Id = 1, TimeDelta = 60, ValueDelta = devices[0].Items[0].ValueDelta},
				new SubscriptionItem(devices[0].Items[1]){Id = 2, TimeDelta = 60, ValueDelta = devices[0].Items[1].ValueDelta},
				new SubscriptionItem(devices[0].Items[2]){Id = 3, TimeDelta = 60, ValueDelta = devices[0].Items[2].ValueDelta},
				new SubscriptionItem(devices[0].Items[3]){Id = 4, TimeDelta = 60, ValueDelta = devices[0].Items[3].ValueDelta},
				new SubscriptionItem(devices[0].Items[4]){Id = 5, TimeDelta = 60, ValueDelta = devices[0].Items[4].ValueDelta},
				new SubscriptionItem(devices[0].Items[5]){Id = 6, TimeDelta = 60, ValueDelta = devices[0].Items[5].ValueDelta}
			};

			#endregion

			ObjectFactory.Configure(item => item.For<IConfigRepo>().Add(Mock.Of<IConfigRepo>(m =>
				m.GetAllDevices() == devices 
					&& m.GetAllSubscriptions(It.IsAny<Cache>()) == subscriptions
			)));

			var historyMock = new Mock<IHistoryRepo>();
			historyMock.Setup(x => x.Save(It.Is<long>(v => v != 0), It.IsAny<ISnmpData>(), It.IsAny<DateTime>()));
			ObjectFactory.Configure(item => item.For<IHistoryRepo>().Add(historyMock.Object));

			var executorMock = new Mock<INotificationExecutor>();
			executorMock.Setup(x => x.Execute(It.IsAny<IEnumerable<Notification>>()));
			ObjectFactory.Configure(item => item.For<INotificationExecutor>().Add(executorMock.Object));

			var server = new SnmpScanServer();
			//Long sleep for debug purpose
			Thread.Sleep(600000);

			var values = server.GetAllValues();
		} 
	}
}

