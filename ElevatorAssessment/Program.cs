// See https://aka.ms/new-console-template for more information
using ElevatorAssessment.Model;
using ElevatorAssessment.Repo;
using System.Drawing;

const int numberOfFloors = 100;
const int numberOfElevators = 5;
const int numberOfRequests = 100 * 5;
const string QUIT = "q";
Console.WriteLine("Please press q to quit the application");
Console.WriteLine("");
Console.WriteLine($"There are {numberOfFloors} number of floors in the building");

Console.WriteLine($"The Building has {numberOfElevators} elevators!");

Console.WriteLine($"There are {numberOfRequests} number of  elevators requests in the build!");

Console.WriteLine("");


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

    Display(system.GetStatus(i));
}

string input = string.Empty;
while (input != QUIT)
{
    Console.WriteLine("Please press C to call the elevator and S - to check elevator current floor");
    input = Console.ReadLine();

    if (input.ToLower() == "C".ToLower())
    {
        //  Console.WriteLine("Please press current floor Number");
        //string requestFloor = Console.ReadLine();
        Console.WriteLine("Please press elevator ID you requesting");
        string elevatorRequsted = Console.ReadLine();
        Console.WriteLine("Please press which floor you would like to go to");
        string destinationFloor = Console.ReadLine();
       
        Console.WriteLine("Please enter number of waiting persons");
        string waitingPerson = Console.ReadLine();
        if (Int32.TryParse(destinationFloor, out int destinationFloorNo)
            && Int32.TryParse(elevatorRequsted, out int elevatorRequstedId) && Int32.TryParse(waitingPerson, out int waitingPersonNo))
            if (elevatorRequstedId <= numberOfElevators)
            {
                var elevator = system.GetStatus(elevatorRequstedId);
                if (elevator.DestinationFloor == destinationFloorNo) 
                {
                    Console.WriteLine("Current and destination floor are the same");
                }
                else
                {
                    
                    system.Update(elevatorRequstedId, elevator.DestinationFloor, destinationFloorNo, waitingPersonNo);
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

            Display(system.GetStatus(Id));
        }
    }
    else if (input == QUIT)
    {
        Console.WriteLine("GoodBye!");
        System.Environment.Exit(1);
    }
    else
        Console.WriteLine("You have pressed an incorrect floor, Please try again");
}

Console.ReadLine();

void Display(Elevator elevator)
{
    Console.WriteLine($"Elevator - {elevator.Id} - From floor {elevator.CurrentFloor} " +
                $"- Destination floor {elevator.DestinationFloor} - Number of persons {elevator.Persons.Count} - Direction {elevator.Direction}");

}