using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKPZlab5
{
    public class Automobile
    {
        public enum Colour
        {
            Undefined,
            White,
            Black,
            Red,
            Green,
            Yellow,
            Pink
        }
        private uint length;
        private uint width;
        private uint height;
        private double horsepower;
        private uint seats;
        private float zero_to_hundred;
        private Colour color;
        private int number;
        public Automobile()
        {
            length = 0;
            width = 0;
            height = 0;
            horsepower = 0;
            seats = 0;
            zero_to_hundred = 0;
            color = Colour.Undefined;
        }

        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        public uint Length
        {
            get { return length; }
            set { length = value; }
        }
        public uint Width
        {
            get { return width; }
            set { width = value; }
        }
        public uint Height
        {
            get { return height; }
            set { height = value; }
        }
        public double Horsepower
        {
            get { return horsepower; }
            set { horsepower = value; }
        }
        public uint Seats
        {
            get { return seats; }
            set { seats = value; }
        }
        public float Zero_to_hundred
        {
            get { return zero_to_hundred; }
            set { zero_to_hundred = value; }
        }
        public Colour Color
        {
            get { return color; }
            set { color = value; }
        }
        public ulong CalculateVolume()
        {
            ulong volume = (ulong)length * width * height;
            return volume;
        }
        public bool IsBetterPerformance(Automobile other_car)
        {
            if ((horsepower > other_car.horsepower) && (zero_to_hundred < other_car.zero_to_hundred))
            {
                return true;
            }
            return false;
        }
        public bool CanFitInCar(int number_of_passangers)
        {
            if (seats >= number_of_passangers)
            {
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return $"{Color} car | {Horsepower} hp | {Seats} seats";
        }
    }
}
