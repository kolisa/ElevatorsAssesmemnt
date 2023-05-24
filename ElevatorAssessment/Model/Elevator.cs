using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAssessment.Model
{
    public class Elevator
    {
        public Elevator(int id)
        {
            Id = id;
            Persons = new List<Person>();
        }

        public int Id { get; private set; }
        public int CurrentFloor { get; set; }
        public int DestinationFloor { get; set; }
        public int MaxNumberOfPersons { get; set; } = 50;
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
}
