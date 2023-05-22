using ElevatorAssessment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAssessment.Repo
{
    public interface IElevatorSystem
    {
        Elevator GetStatus(int elevatorId);
        void Update(int elevatorId, int floorNumber, int goalFloorNumber, int waitingPersons = 0);
        void Pickup(int pickupFloor, int destinationFloor);
        void Step();
        bool AnyOutstandingPickups();
    }
}
