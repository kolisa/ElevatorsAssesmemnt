// See https://aka.ms/new-console-template for more information
using ElevatorAssessment.Repo;
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
IElevatorSystem system = new ElevatorSystem(numberOfElevators);


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

Console.ReadLine();
