using ElevatorAssessment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAssessment.Repo
{
    internal class ElevatorSystem : IElevatorSystem
    {
        public List<Elevator> Elevators { get; set; }
        public List<Person> WaitingPersons { get; set; }

        public ElevatorSystem(int numberOfElevators)
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

            // Embark persons to available elevators
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
}
