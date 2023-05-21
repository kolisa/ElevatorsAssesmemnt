// See https://aka.ms/new-console-template for more information
using System.Drawing;

int numberOfFloors = 10;
int numberOfElevators = 5;
int numberOfRequests = 10 * 5;
const string QUIT = "q";
Console.WriteLine("Welcome to Elevator Project!!");

Console.WriteLine("How tall is the building that this elevator will be in?");
numberOfFloors = int.Parse(Console.ReadLine());
Console.WriteLine("How many elevators in the build?");
numberOfElevators = int.Parse(Console.ReadLine());
Console.WriteLine("Number of persons requesting the elevator in the build?");
numberOfRequests = int.Parse(Console.ReadLine());
// You may run into a memory limit (.NET fiddle limitation) if this is too high

// Don't change anything below this line
var pickupCount = 0;
var stepCount = 0;
var random = new Random();
IElevatorControlSystem system = new ControlSystem(numberOfElevators);


while (pickupCount < numberOfRequests)
{
    var originatingFloor = random.Next(1, numberOfFloors + 1);
    var destinationFloor = random.Next(1, numberOfFloors + 1);
    if (originatingFloor != destinationFloor)
    {
        system.Pickup(originatingFloor, destinationFloor);
        pickupCount++;
    }

}

while (system.AnyOutstandingPickups())
{
    system.Step();
    stepCount++;
}
for (int i = 0; i < numberOfElevators; i++)
{

    var stattus = system.GetStatus(i);
    Console.WriteLine($"Elevator - {stattus.Id} - From floor {stattus.CurrentFloor} " +
        $"- Destination floor {stattus.DestinationFloor} - Number of persons {stattus.Persons.Count} - direction {stattus.Direction}");
}

string input = string.Empty;
while (input != QUIT)
{
    Console.WriteLine("Please press C to call the elevator and S - to check elevator current floor");
    input = Console.ReadLine();

    if (input.ToLower() == "C".ToLower())
    {
        Console.WriteLine("Please press current floor Number");
        string requestFloor = Console.ReadLine();
        Console.WriteLine("Please press which floor you would like to go to");
        string destinationFloor = Console.ReadLine();
        Console.WriteLine("Please press elevator ID you requesting");
        string elevatorRequsted = Console.ReadLine();
        Console.WriteLine("Please enter number of waiting persons");
        string waitingPerson = Console.ReadLine();
        if (Int32.TryParse(requestFloor, out int requestFloorNo) && Int32.TryParse(destinationFloor, out int destinationFloorNo)
            && Int32.TryParse(elevatorRequsted, out int elevatorRequstedId) && Int32.TryParse(waitingPerson, out int waitingPersonNo))
            if (elevatorRequstedId <= numberOfElevators)
            {
                if (requestFloorNo == destinationFloorNo) 
                {
                    Console.WriteLine("Current and destination floor are the same");
                }
                else
                {
                    system.Update(elevatorRequstedId, requestFloorNo, destinationFloorNo, waitingPersonNo);
                    Console.WriteLine("Stopped at floor {0}", destinationFloorNo);
                }
            }
    }
    if (input.ToLower() == "S".ToLower())
    {
        Console.WriteLine("Please enter Elevator Id to get the current status");
        string elevatorStatus = Console.ReadLine();
        if (Int32.TryParse(elevatorStatus, out int Id))
        {
            var status = system.GetStatus(Id);
            Console.WriteLine($"Elevator - {status.Id} - From floor {status.CurrentFloor} " +
                $"- Destination floor {status.DestinationFloor} - Number of persons {status.Persons.Count} - direction {status.Direction}");
        }
    }   
    else if (input == QUIT)
        Console.WriteLine("GoodBye!");
    else
        Console.WriteLine("You have pressed an incorrect floor, Please try again");
}
//Console.WriteLine("Transported {0} elevator riders to their requested destinations in {1} steps.", pickupCount, stepCount);
Console.ReadLine();

public interface IElevatorControlSystem
{
    Elevator GetStatus(int elevatorId);
    void Update(int elevatorId, int floorNumber, int goalFloorNumber, int waitingPersons = 0);
    void Pickup(int pickupFloor, int destinationFloor);
    void Step();
    bool AnyOutstandingPickups();
}

public class ControlSystem : IElevatorControlSystem
{
    public List<Elevator> Elevators { get; set; }
    public List<Person> WaitingPersons { get; set; }

    public ControlSystem(int numberOfElevators)
    {
        Elevators = Enumerable.Range(0, numberOfElevators).Select(eid => new Elevator(eid)).ToList();
        WaitingPersons = new List<Person>();
    }


    public Elevator GetStatus(int elevatorId)
    {
        return Elevators.First(e => e.Id == elevatorId);
    }

    public void Update(int elevatorId, int floorNumber, int goalFloorNumber, int waitingPersons = 0)
    {
        UpdateElevator(elevatorId, e =>
        {
            e.CurrentFloor = floorNumber;
            e.DestinationFloor = goalFloorNumber;
            if (waitingPersons > 0)
            {
                e.Persons.Clear();
                for (int i = 0; i < waitingPersons; i++) { e.Persons.Add(new Person(floorNumber, goalFloorNumber)); }
               
            }
        });
    
    }

    public void Pickup(int pickupFloor, int destinationFloor)
    {
        WaitingPersons.Add(new Person(pickupFloor, destinationFloor));

    }

    private void UpdateElevator(int elevatorId, Action<Elevator> update)
    {
        Elevators = Elevators.Select(e =>
        {
            if (e.Id == elevatorId) update(e);
            return e;
        }).ToList();
        //  Console.WriteLine(elevatorId);
    }

    public void Step()
    {
        var busyElevatorIds = new List<int>();
        // unload elevators
        Elevators = Elevators.Select(e =>
        {
            var disembarkingPersons = e.Persons.Where(r => r.DestinationFloor == e.CurrentFloor).ToList();
            if (disembarkingPersons.Any())
            {
                busyElevatorIds.Add(e.Id);
                e.Persons = e.Persons.Where(r => r.DestinationFloor != e.CurrentFloor).ToList();
            }

            return e;
        }).ToList();

        // Embark passengers to available elevators
        WaitingPersons.GroupBy(r => new { r.OriginatingFloor, r.Direction }).ToList().ForEach(waitingFloor =>
        {
            var availableElevator =
                Elevators.FirstOrDefault(
                    e =>
                        e.CurrentFloor == waitingFloor.Key.OriginatingFloor &&
                        (e.Direction == waitingFloor.Key.Direction || !e.Persons.Any()));
            if (availableElevator != null)
            {
                busyElevatorIds.Add(availableElevator.Id);
                var embarkingPassengers = waitingFloor.ToList();
                UpdateElevator(availableElevator.Id, e => e.Persons.AddRange(embarkingPassengers));
                WaitingPersons = WaitingPersons.Where(r => embarkingPassengers.All(er => er.Id != r.Id)).ToList();
            }
        });


        Elevators.ForEach(e =>
        {
            var isBusy = busyElevatorIds.Contains(e.Id);
            int destinationFloor;
            if (e.Persons.Any())
            {
                var closestDestinationFloor =
                    e.Persons.OrderBy(r => Math.Abs(r.DestinationFloor - e.CurrentFloor))
                        .First()
                        .DestinationFloor;
                destinationFloor = closestDestinationFloor;
            }
            else if (e.DestinationFloor == e.CurrentFloor && WaitingPersons.Any())
            {
                // Lots of optimization could be done here, perhaps?
                destinationFloor = WaitingPersons.GroupBy(r => new { r.OriginatingFloor }).OrderBy(g => g.Count()).First().Key.OriginatingFloor;
            }
            else
            {
                destinationFloor = e.DestinationFloor;
            }

            var floorNumber = isBusy
                ? e.CurrentFloor
                : e.CurrentFloor + (destinationFloor > e.CurrentFloor ? 1 : -1);

            Update(e.Id, floorNumber, destinationFloor);
        });
    }

    public bool AnyOutstandingPickups()
    {
        return WaitingPersons.Any();
    }
}

public class Elevator
{
    public Elevator(int id)
    {
        Id = id;
        Persons = new List<Person>();
    }

    public int Id { get; private set; } = 1;
    public int CurrentFloor { get; set; }
    public int DestinationFloor { get; set; }
    public int MaxNumberOfPersons { get; set; }
    public Direction Direction
    {
        get
        {
            return CurrentFloor == 1
                ? Direction.Up
                : DestinationFloor > CurrentFloor ? Direction.Up : Direction.Down;
        }
    }

    public List<Person> Persons { get; set; }
}

public class Person
{
    public Person(int originatingFloor, int destinationFloor)
    {
        OriginatingFloor = originatingFloor;
        DestinationFloor = destinationFloor;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
    public int OriginatingFloor { get; private set; }
    public int DestinationFloor { get; private set; }

    public Direction Direction
    {
        get { return OriginatingFloor < DestinationFloor ? Direction.Up : Direction.Down; }
    }
}

public enum Direction
{
    Up = 1,
    Down = -1
}