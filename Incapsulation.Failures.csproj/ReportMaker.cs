using System;
using System.Collections.Generic;
using System.Linq;

namespace Incapsulation.Failures
{
    internal class Device
    {
        public readonly int ID;
        public readonly string Name;

        public Device (int id, string name)
        {
            ID = id;
            Name = name;
        }
    }

    internal class Failure
    {
        public readonly int ID;
        public readonly int Type;
        public readonly DateTime Date;

        public Failure (int id, int type, int day, int month, int year)
        {
            ID = id;
            Type = type;
            Date = new DateTime(year, month, day);
        }

        public bool IsFailureSerious()
        {
            switch ((FailureType)Type)
            {
                case FailureType.UnexpectedShutdown:
                    return true;
                case FailureType.HardwareFailures:
                    return true;
                default:
                    return false;
            }
        }
        
        private enum FailureType
        {
            UnexpectedShutdown,
            ShortNonResponding,
            HardwareFailures,
            ConnectionProblems
        }
    }

    public class ReportMaker
    {
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes, 
            int[] deviceId, 
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            var deviceList = new List<Device>();
            var failureList = new List<Failure>();
            
            foreach (var device in devices)
                deviceList.Add(new Device((int)device["DeviceId"], device["Name"] as string));
            
            for (int i = 0; i < failureTypes.Length; i++)
                failureList.Add(new Failure(deviceId[i], 
                    failureTypes[i], 
                    (int)times[i][0], 
                    (int)times[i][1], 
                    (int)times[i][2]));
            
            return FindDevicesFailedBeforeDate(deviceList, failureList, new DateTime(year, month, day));
        }
        
        private static List<string> FindDevicesFailedBeforeDate(
            List<Device> devices, 
            List<Failure> failures, 
            DateTime dateZ)
        {
            return devices
                .Where(device => failures
                    .Where(failure => failure.IsFailureSerious() && failure.Date < dateZ)
                    .Select(failure => failure.ID)
                    .Contains(device.ID))
                .Select(device => device.Name)
                .ToList();
        }
    }
}
