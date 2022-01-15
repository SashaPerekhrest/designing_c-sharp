using System;
using System.Globalization;
using System.Linq;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    public class DriversRepository
    {
    }
    
    public class TaxiApi : ITaxiApi<TaxiOrder>
    {
        private readonly DriversRepository driversRepo;
        private readonly Func<DateTime> currentTime;
        private int idCounter;

        public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
        {
            this.driversRepo = driversRepo;
            this.currentTime = currentTime;
        }
        
        public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName,
                                                       string street, string building)
        {
            return new TaxiOrder(
                    idCounter++,
                    new PersonName(firstName, lastName),
                    new Address(street, building),
                    currentTime());
        }
        
        public void UpdateDestination(TaxiOrder order, string street, string building)
        {
            order.UpdateDestination(new Address(street, building));
        }
        
        public void AssignDriver(TaxiOrder order, int driverId)
        {
            order.AssignDriver(driverId, currentTime());
        }
        
        public void UnassignDriver(TaxiOrder order)
        {
            order.UnassignDriver();
        }
        
        public string GetDriverFullInfo(TaxiOrder order) 
            => order.GetDriverFullInfo();

        public string GetShortOrderInfo(TaxiOrder order) 
            => order.GetShortOrderInfo();

        /*private DateTime GetLastProgressTime(TaxiOrder order)
        {
            return order.GetLastProgressTime();
        }*/
        
        public void Cancel(TaxiOrder order)
        {
            order.Cancel(currentTime());
        }
        
        public void StartRide(TaxiOrder order)
        {
            order.StartRide(currentTime());
        }
        
        public void FinishRide(TaxiOrder order)
        {
            order.FinishRide(currentTime());
        }
    }
    
    public class TaxiOrder : Entity<int>
    {
        private readonly int id;
        public PersonName ClientName { get; }
        public Address Start { get; }
        public Address Destination { get; private set; }
        public Driver Driver { get; private set; }
        public TaxiOrderStatus Status { get; private set; }
        public DateTime CreationTime { get; }
        public DateTime DriverAssignmentTime { get; private set; }
        public DateTime CancelTime { get; private set; }
        public DateTime StartRideTime { get; private set; }
        public DateTime FinishRideTime { get; private set; }

        public TaxiOrder(int id, PersonName clientName, Address startAddress, DateTime dateTime) : base(id)
        {
            this.id = id;
            ClientName = clientName;
            Start = startAddress;
            CreationTime = dateTime;
        }
        
        public void Cancel(DateTime cancelTime)
        {
            if (Status == TaxiOrderStatus.InProgress)
                throw new InvalidOperationException();
            Status = TaxiOrderStatus.Canceled;
            CancelTime = cancelTime;
        }
        
        public void UpdateDestination(Address destination) { Destination = destination; }
        
        public void AssignDriver(int driverId, DateTime assignTime)
        {
            if (Driver != null)
                throw new InvalidOperationException();

            if (driverId != 15)
                throw new Exception("Unknown driver id " + driverId);

            Status = TaxiOrderStatus.WaitingCarArrival;
            Driver = new Driver(driverId, new PersonName("Drive", "Driverson"),
                "Lada sedan", "Baklazhan", "A123BT 66");
            DriverAssignmentTime = assignTime;
        }
        
        public void UnassignDriver()
        {
            if (Status == TaxiOrderStatus.InProgress || Driver == null)
                throw new InvalidOperationException(Status.ToString());
            Status = TaxiOrderStatus.WaitingForDriver;
            Driver = new Driver(null, null, null, null, null);
        }
        
        public void StartRide(DateTime startTime)
        {
            if (Driver == null)
                throw new InvalidOperationException();
            Status = TaxiOrderStatus.InProgress;
            StartRideTime = startTime;
        }
        
        public void FinishRide(DateTime finishTime)
        {
            if (Status != TaxiOrderStatus.InProgress || Driver == null)
                throw new InvalidOperationException();
            Status = TaxiOrderStatus.Finished;
            FinishRideTime = finishTime;
        }
        
        public string GetDriverFullInfo()
        {
            if (Status == TaxiOrderStatus.WaitingForDriver)
                return null;
            return string.Join(" ",
                "Id: " + Driver.Identificator,
                "DriverName: " + FormatName(Driver.Name.FirstName, Driver.Name.LastName),
                "Color: " + Driver.Car.Color,
                "CarModel: " + Driver.Car.Model,
                "PlateNumber: " + Driver.Car.PlateNumber);
        }
        
        private static string FormatName(string firstName, string lastName) 
            => string.Join(" ", new[] { firstName, lastName }.Where(n => n != null));

        private static string FormatAddress(string street, string building) 
            => string.Join(" ", new[] { street, building }.Where(n => n != null));

        public string GetShortOrderInfo()
        {
            var gotDriver = Driver == null || Driver.Name == null;
            var driver = gotDriver ? "" : FormatName(Driver.Name.FirstName, Driver.Name.LastName);
            var destination = Destination == null ? "" : FormatAddress(Destination.Street, Destination.Building);
            return string.Join(" ",
                "OrderId: " + id,
                "Status: " + Status,
                "Client: " + FormatName(ClientName.FirstName, ClientName.LastName),
                "Driver: " + driver,
                "From: " + FormatAddress(Start.Street, Start.Building),
                "To: " + destination,
                "LastProgressTime: " + GetLastProgressTime()
                                        .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }
        
        public DateTime GetLastProgressTime()
        {
            switch (Status)
            {
                case TaxiOrderStatus.WaitingForDriver:
                    return CreationTime;
                case TaxiOrderStatus.WaitingCarArrival:
                    return DriverAssignmentTime;
                case TaxiOrderStatus.InProgress:
                    return StartRideTime;
                case TaxiOrderStatus.Finished:
                    return FinishRideTime;
                case TaxiOrderStatus.Canceled:
                    return CancelTime;
            }
            throw new NotSupportedException(Status.ToString());
        }
    }
    
    public class Driver : Entity<int>
    {
        public int? Identificator { get; }
        public PersonName Name { get; }
        public Car Car { get; }

        public Driver(int? id, PersonName name, string carModel,
                      string carColor, string carPlateNumber) : base(0)
        {
            Identificator = id;
            Name = name;
            Car = new Car(carModel, carColor, carPlateNumber);
        }
    }
    
    public class Car : ValueType<Car>
    {
        public string Model { get; }
        public string Color { get; }
        public string PlateNumber { get; }

        public Car(string model, string color, string plateNumber)
        {
            Model = model;
            Color = color;
            PlateNumber = plateNumber;
        }
    }
}