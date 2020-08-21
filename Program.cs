/* Rover testing suite
 * SIT232 Task 5.3C
 * Benjamin Davey
 */

using System;
using System.Collections.Generic;

namespace Program {
    class Battery {
        public int charge;
        public int serial;
        public Rover rover;
        
        public Battery(int serial) {
            this.charge = 100;
            this.serial = serial;
            this.rover = null;
        }
        
        public void Drain(int charge) {
            this.charge -= charge;
        }
        
        public void Charge(int charge) {
            this.charge += charge;
        }
        
        public void Load(Rover rover) {
            if (this.rover == null) {
                this.rover = rover;
            } else {
                Console.WriteLine("Battery otherwise engaged.");
            }
        }
        
        public void Unload() {
            if (this.rover != null) {
                this.rover = null;
            } else {
                Console.WriteLine("Not currently loaded.");
            }
        }
        
        public override String ToString() {
            return ($"Serial number: BAT{serial:D4} Charge: {this.charge}");
        }
    }
    
    class Device {
        public String name;
        public Battery battery;
        public Rover rover;
        
        public Device(String name) {
            this.name = name;
            this.battery = null;
            this.rover = null;
        }
        
        public void AttachBattery(Battery battery) {
            if (this.battery == null) {
                this.battery = battery;
            } else {
                Console.WriteLine("Battery already attached. Detach first.");
            }
        }
        
        public void DetachBattery() {
            if (this.battery != null) {
                this.battery = null;
            } else {
                Console.WriteLine("No battery attached.");
            }
        }
        
        public bool Load(Rover rover) {
            if (this.rover == null) {
                this.rover = rover;
                return true;
            } else {
                return false;
            }
        }
        
        public void Unload() {
            if (this.rover == null) {
                Console.WriteLine("Not loaded on rover.");
            } else {
                this.rover = null;
            }
        }
        
        public override String ToString() {
            String poweredStatus = "Unpowered";
            String roverStatus = "Unattached";
            if (this.battery != null) {
                poweredStatus = "Powered";
            }
            if (this.rover != null) {
                roverStatus = this.rover.name;
            }
            return ($"{this.name} ({poweredStatus}, Rover: {roverStatus})");
        }
    }
    
    class Rover {
        public List<Battery> batteryList;
        public List<Device> deviceList;
        public List<Specimen> collectedSpecimenList;
        public Location location;
        public String name;
        public Map map;
        
        public Rover(String name) {
            this.batteryList = new List<Battery>();
            this.deviceList = new List<Device>();
            this.collectedSpecimenList = new List<Specimen>();
            this.name = name;
            this.map = null;
        }
        
        public void LoadBattery(Battery battery) {
            this.batteryList.Add(battery);
        }
        
        public void LoadBattery(List<Battery> batteryList) {
            foreach (Battery battery in batteryList) {
                this.batteryList.Add(battery);
            }
        }
        
        public bool AttachBattery(Battery battery, Device device) {
            if (this.batteryList.Contains(battery) && this.deviceList.Contains(device)) {
                device.AttachBattery(battery);
                return true;
            } else {
                Console.WriteLine("Device or battery not loaded on rover.");
                return false;
            }
        }
        
        public void AttachDevice(Device device) {
            if (device.Load(this)) {
                this.deviceList.Add(device);
            } else {
                Console.WriteLine("Attach failed.");
            }
        }
        
        void DetachDevice(Device device) {
            if (deviceList.Contains(device)) {
                deviceList.Remove(device);
            } else {
                Console.WriteLine("Device not present.");
            }
        }
        
        Battery FindBattery(int serial) {
            foreach (Battery battery in this.batteryList) {
                if (battery.serial == serial) {
                    return battery;
                }
            }
            return null;
        }
        
        public void Move(int x, int y) {
            int oldX;
            int oldY;
            if (this.location != null) {
                oldX = this.location.x;
                oldY = this.location.y;
                this.location.hasRover = false;
                this.location.rover = null;
            
                this.location = this.map.locationMap[oldX + x, oldY + y];
                this.location.hasRover = true;
                this.location.rover = this;
            } else {
                Console.WriteLine("Move failed - Rover not on map.");
            }
        }
        
        public void Place(int x, int y, Map map) {
            this.map = map;
            this.location = this.map.locationMap[x, y];
            this.location.hasRover = true;
            this.location.rover = this;
        }
        
        public override String ToString() {
            return ($"{this.name}");
        }
    }
    
    class Motor : Device {
                
        public Motor(String name) : base(name) {
            
        }
        
        public void Move(int cmd) {
            if ((this.battery != null) && (this.rover != null && (this.rover.map != null))) {
                this.battery.Drain(1);
                switch (cmd) {
                    case 1:
                        this.rover.Move(-1, 0);
                        break;
                    case 2:
                        this.rover.Move(0, 1);
                        break;
                    case 3:
                        this.rover.Move(1, 0);
                        break;
                    case 4:
                        this.rover.Move(0, -1);
                        break;
                    default:
                        Console.WriteLine("Move failed.");
                        break;
                }
            } else {
                Console.WriteLine("No battery attached, not attached to rover, or rover not placed on map.");
            }
        }
    }
    
    class Radar : Device {
        
        public Radar(String name) : base(name) {
            
        }
        
        public virtual List<Specimen> Scan() {
            return (new List<Specimen>());
        }
    }
    
    class SolarPanel : Device {
        
        public SolarPanel(String name) : base(name) {
            
        }
        
        public void ChargeBattery() {
            if (!(this.battery == null)) {
                this.battery.Charge(1);
            } else {
                Console.WriteLine("No battery attached.");
            }
        }
    }
    
    class Drill : Device {
        public int wearFactor;
        
        public Drill(String name) : base(name) {
            this.wearFactor = 0;
        }
        
        public void DrillHere() {
            if ((this.battery != null) && (this.rover != null) && (this.rover.location != null)) {
                this.battery.Drain(1);
                if (this.rover.location.hasSpecimen) {
                    this.wearFactor += 5;
                    this.rover.location.hasSpecimen = false;
                    this.rover.collectedSpecimenList.Add(this.rover.location.specimen);
                    this.rover.location.specimen = null;
                } else {
                    this.wearFactor += 10;
                }
            } else {
                Console.WriteLine("Drill failed.");
            }
        }
        
        public override String ToString() {
            String poweredStatus = "Unpowered";
            String roverStatus = "Unattached";
            if (this.battery != null) {
                poweredStatus = "Powered";
            }
            if (this.rover != null) {
                roverStatus = this.rover.name;
            }
            return ($"{this.name} (Wear Factor: {this.wearFactor}, {poweredStatus}, Rover: {roverStatus})");
        }
    }
    
    class LocationRadar : Radar {
        public LocationRadar(String name) : base(name) {
            
        }
        
        public override List<Specimen> Scan() {
            List<Specimen> foundSpecimens = new List<Specimen>();
            if ((this.battery != null) && (this.rover != null) && (this.rover.map != null)) {
                
                int x = this.rover.location.x;
                int y = this.rover.location.y;
                this.battery.Drain(4);
                for (int i = -1; i < 3; i++) {
                    for (int j = -2; j < 3; j++) {
                        if (this.rover.map.locationMap[x + i, y + j].hasSpecimen == true) {
                            foundSpecimens.Add(this.rover.map.locationMap[x+i,y+j].specimen);
                        }
                    }
                }
            } else {
                Console.WriteLine("No battery attached.");
            }
            return foundSpecimens;
        }
    }
    
    class SizeRadar : Radar {
        public SizeRadar(String name) : base(name) {
            
        }
        public override List<Specimen> Scan() {
            List<Specimen> foundSpecimens = new List<Specimen>();
            if ((this.battery != null) && (this.rover != null) && (this.rover.map != null)) {
                
                int x = this.rover.location.x;
                int y = this.rover.location.y;
                this.battery.Drain(4);
                for (int i = -1; i < 3; i++) {
                    for (int j = -2; j < 3; j++) {
                        if (this.rover.map.locationMap[x + i, y + j].hasSpecimen == true) {
                            foundSpecimens.Add(this.rover.map.locationMap[x+i,y+j].specimen);
                        }
                    }
                }
            } else {
                Console.WriteLine("No battery attached.");
            }
            return foundSpecimens;
        }
    }
    
    class NameRadar : Radar {
        public NameRadar(String name) : base(name) {
            
        }
        public override List<Specimen> Scan() {
            List<Specimen> foundSpecimens = new List<Specimen>();
            if ((this.battery != null) && (this.rover != null) && (this.rover.map != null)) {
                
                int x = this.rover.location.x;
                int y = this.rover.location.y;
                this.battery.Drain(4);
                for (int i = -1; i < 3; i++) {
                    for (int j = -2; j < 3; j++) {
                        if (this.rover.map.locationMap[x + i, y + j].hasSpecimen == true) {
                            foundSpecimens.Add(this.rover.map.locationMap[x+i,y+j].specimen);
                        }
                    }
                }
            } else {
                Console.WriteLine("No battery attached.");
            }
            return foundSpecimens;
        }
    }
    
    class Map {
        public Location[,] locationMap;
        public List<Specimen> specimenList;
        public int size;
        
        public Map(int size) {
            this.size = size;
            this.specimenList = new List<Specimen>();
            Random rnd = new Random();
            
            this.locationMap = new Location[size, size];
            
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    this.locationMap[i, j] = new Location(i, j);
                }
            }
        }
        
        public void PlaceSpecimen(Specimen specimen) {
            int x, y;
            bool placed = false;
            do {
                Random rnd = new Random();
                x = rnd.Next(this.size);     
                y = rnd.Next(this.size);
                if (!this.locationMap[x, y].hasSpecimen) {
                    locationMap[x, y].hasSpecimen = true;
                    locationMap[x, y].specimen = specimen;
                    placed = true;
                }
            } while (!placed);
        }
        
        public void PlaceSpecimen(List<Specimen> specimenList) {
            foreach (Specimen specimen in specimenList) {
                int x, y;
                bool placed = false;
                do {
                    Random rnd = new Random();
                    x = rnd.Next(this.size);     
                    y = rnd.Next(this.size);
                    if (!this.locationMap[x, y].hasSpecimen) {
                        locationMap[x, y].hasSpecimen = true;
                        locationMap[x, y].specimen = specimen;
                        placed = true;
                    }
                } while (!placed);
            }
        }
        
        public void PrintMap() {
            for (int i = 0; i < this.size; i++) {
                for (int j = 0; j < this.size; j++) {
                    Console.Write(this.locationMap[i, j].ToString() + " | ");
                }
                Console.WriteLine();
            }
        }
        
        public bool IsSpecimen(int x, int y) {
            return this.locationMap[x, y].hasSpecimen;
        }
    }
    
    class Location {
        public bool hasSpecimen;
        public bool hasRover;
        public Specimen specimen;
        public Rover rover;
        public int x;
        public int y;
        
        public Location(int x, int y) {
            this.x = x;
            this.y = y;
            this.hasSpecimen = false;
            this.hasRover = false;
            this.specimen = null;
            this.rover = null;
        }
        
        public Location(int x, int y, Specimen specimen) {
            this.x = x;
            this.y = y;
            this.hasSpecimen = true;
            this.specimen = specimen;
            this.hasRover = false;
            this.rover = null;
        }
        
        public override String ToString() {
            if (hasRover) {
                return "#";
            } else if (hasSpecimen) {
                return "@";
            }
            else {
                return " ";
            }
        }
    }
    
    class Specimen {
        public String name;
        public int size;
        
        public Specimen(String name, int size) {
            this.name = name;
            this.size = size;
        }
        
        public override String ToString() {
            return ($"{name}: size {size}");
        }
    }
    
    class ProgramTest {
        public static void Main(String[] args) {
            // Constructor tests
            Battery battery0 = new Battery(0);
            Rover rover0 = new Rover("Sojourner");
            Motor motor0 = new Motor("RotoPro 10X");
            Drill drill0 = new Drill("DrillMaster 1000");
            SolarPanel solarPanel0 = new SolarPanel("ReflectoMaster");
            LocationRadar locationRadar0 = new LocationRadar("WhereDoo");
            NameRadar nameRadar0 = new NameRadar("Whatsit-O-Meter");
            SizeRadar sizeRadar0 = new SizeRadar("SeeBiggy");
            Map map0 = new Map(20);
            Location location0 = new Location(5, 5);
            Specimen specimen0 = new Specimen("John: rocks", 10);
            Location location1 = new Location(10, 10, specimen0);
            // Battery tests
                // Drain(int)
                // Charge(int)
                // Load(Rover)
                // Unload()
                // ToString()
            // Device tests
            // Rover tests
                // 
                // LoadBattery(Battery)
                // LoadBattery(List<Battery>)
                // AttachBattery(Battery, Device)
                // AttachDevice(Device)
                // DetachDevice(Device)
                // FindBattery(String)
                // Move(int, int)
                // Place(int, int, Map)
                // ToString()
            // Motor tests
                // Move()
            // Drill tests
                // Drill()
            // SolarPanel tests
                // Charge()
            // Radar tests
            // LocationRadar tests
            // NameRadar tests
            // SizeRadar tests
            // Map tests
            // Location tests
            // Specimen tests
        }
    }
}
