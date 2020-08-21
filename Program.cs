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
                this.AttachBattery(this.GetChargedBattery(), device);
            } else {
                Console.WriteLine("Attach failed.");
            }
        }
        
        public void DetachDevice(Device device) {
            if (deviceList.Contains(device)) {
                deviceList.Remove(device);
                device.Unload();
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
        
        public Battery GetChargedBattery() {
            Battery returnBattery = this.batteryList[0];
            foreach (Battery battery in this.batteryList) {
                if (battery.charge > returnBattery.charge) {
                    returnBattery = battery;
                }
            }
            return returnBattery;
        }
        
        public void Move(int x, int y) {
            int oldX;
            int oldY;
            if (this.location != null) {
                oldX = this.location.x;
                oldY = this.location.y;
                this.location.hasRover = false;
                this.location.rover = null;
                
                try {
                    this.location = this.map.locationMap[oldX + x, oldY + y];
                } catch {
                    Console.WriteLine("Move failed - map ends.");
                }
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
            Random rnd = new Random();
            if ((this.battery != null) && (this.rover != null) && (this.rover.location != null)) {
                if ((this.wearFactor >= 100) && (rnd.Next(5) == 1)) {
                    Console.WriteLine("Drill failure");
                } else {
                    this.battery.Drain(1);
                    if (this.rover.location.hasSpecimen) {
                        this.wearFactor += 5;
                        this.rover.location.hasSpecimen = false;
                        this.rover.collectedSpecimenList.Add(this.rover.location.specimen);
                        this.rover.location.specimen = null;
                    } else {
                        this.wearFactor += 10;
                    }
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
    
    class Game {
        public Map gameMap;
        List<Rover> roverList;
        public Rover selectedRover;
        List<Specimen> specimenList;
        List<Battery> batteryList;
        List<Drill> drillList;
        List<Motor> motorList;
        List<SolarPanel> solarPanelList;
        List<Radar> radarList;
        
        public Game(int mapSize) {
            
            // Generate map
            this.gameMap = new Map(mapSize);
            
            // Generate rovers
            
            this.roverList = new List<Rover>();
            
            roverList.Add(new Rover("Opportunity"));
            roverList[0].Place(Convert.ToInt32(mapSize/2), Convert.ToInt32(mapSize/2), gameMap);
            
            roverList.Add(new Rover("Spirit"));
            roverList[1].Place(Convert.ToInt32(mapSize/3), Convert.ToInt32(mapSize/3), gameMap);
            
            this.selectedRover = roverList[0];
            
            // Generate batteries
            
            this.batteryList = new List<Battery>();
            
            batteryList.Add(new Battery(1));
            batteryList.Add(new Battery(2));
            
            // Generate devices
            // Generate motors
            
            this.motorList = new List<Motor>();
            
            motorList.Add(new Motor("RotoPro 10X"));
            motorList.Add(new Motor("Kawasaki Wanderer"));
            
            // Generate drills
            
            this.drillList = new List<Drill>();
            
            drillList.Add(new Drill("Drillmaster X"));
            
            // Generate solar panels
            
            this.solarPanelList = new List<SolarPanel>();
            
            solarPanelList.Add(new SolarPanel("SunFriend"));
            
            // Generate Radars
            
            this.radarList = new List<Radar>();
            
            radarList.Add(new LocationRadar("Where 5i"));
            radarList.Add(new SizeRadar("SeeBiggy"));
            radarList.Add(new NameRadar("NomSeek"));
            
            // Add devices to Rovers
            
            roverList[0].LoadBattery(batteryList[0]);
            roverList[0].AttachDevice(motorList[0]);
            roverList[0].AttachBattery(batteryList[0], motorList[0]);
            
            roverList[1].LoadBattery(batteryList[1]);
            roverList[1].AttachDevice(motorList[1]);
            roverList[1].AttachBattery(batteryList[1], motorList[1]);
            
            // Add specimens to map
            
            this.specimenList = new List<Specimen>();
            
            specimenList.Add(new Specimen("John: Rock", 5));
            specimenList.Add(new Specimen("John: Dust", 2));
            specimenList.Add(new Specimen("Elizabeth: Rock", 6));
            specimenList.Add(new Specimen("Elizabeth: Possible Fossil", 8));
            specimenList.Add(new Specimen("Elizabeth: Water", 3));
            specimenList.Add(new Specimen("Milo: Rock", 5));
            specimenList.Add(new Specimen("Milo: Abandoned Rover", 120));
            specimenList.Add(new Specimen("Milo: Dust", 2));
            specimenList.Add(new Specimen("Svetlana: Scraping", 2));
            specimenList.Add(new Specimen("Svetlana: Dust", 2));
            
            this.gameMap.PlaceSpecimen(this.specimenList);
        }
        public void ViewRovers() {
            int input = new int();
            int choice = 1;
            foreach (Rover rover in this.roverList) {
                Console.Write($"{choice}: ");
                Console.WriteLine(rover.ToString());
                choice += 1;
            }
            Console.Write("Select rover: ");
            input = Convert.ToInt32(Console.ReadLine());
            this.selectedRover = roverList[input - 1];
        }
        
        public void ViewDevices() {
            bool quit = false;
            do {
                Console.WriteLine("1. Motors\n2. Drills\n3. Solar Panels\n4. Radars\n5. Quit");
                Console.WriteLine("Select device type:");
                int input = Convert.ToInt32(Console.ReadLine());
                
                switch (input) {
                    case 1:
                        this.ViewMotors();
                        quit = true;
                        break;
                    case 2:
                        this.ViewDrills();
                        quit = true;
                        break;
                    case 3:
                        this.ViewSolarPanels();
                        quit = true;
                        break;
                    case 4:
                        this.ViewRadars();
                        quit = true;
                        break;
                    case 5:
                        quit = true;
                        break;
                    default:
                        Console.WriteLine("Please select an option from the list.");
                        break;
                }
            } while (quit == false);
        }
        
        public void ViewMotors() {
            int choice = 1;
            int input = new int();
            foreach (Motor motor in this.motorList) {
                Console.Write($"{choice}: ");
                Console.WriteLine(motor.ToString());
                choice += 1;
            }
            input = Convert.ToInt32(Console.ReadLine());
            bool quit = false;
            int selection = new int();
            do {
                Console.WriteLine(motorList[input - 1].ToString());
                Console.WriteLine("1. Attach/detach device\n2. Attach/detach battery\n3. Use device\n4. Cancel");
                selection = Convert.ToInt32(Console.ReadLine());
                switch (selection) {
                    case 1:
                        if (this.selectedRover == motorList[input - 1].rover) {
                            selectedRover.DetachDevice(motorList[input - 1]);
                        } else {
                            selectedRover.AttachDevice(motorList[input - 1]);
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        this.RunMotor(motorList[input - 1]);
                        break;
                    case 4:
                        quit = true;
                        break;
                    default:
                        Console.WriteLine("Please select an option from the list.");
                        break;
                }
            } while (quit != true);
        }
        
        public void ViewDrills() {
            int choice = 1;
            int input = new int();
            foreach (Drill drill in this.drillList) {
                Console.Write($"{choice}: ");
                Console.WriteLine(drill.ToString());
                choice += 1;
            }
            input = Convert.ToInt32(Console.ReadLine());
            bool quit = false;
            int selection = new int();
            do {
                Console.WriteLine(drillList[input - 1].ToString());
                Console.WriteLine("1. Attach/detach device\n2. Attach/detach battery\n3. Use device\n4. Cancel");
                selection = Convert.ToInt32(Console.ReadLine());
                switch (selection) {
                    case 1:
                        if (this.selectedRover == drillList[input - 1].rover) {
                            selectedRover.DetachDevice(drillList[input - 1]);
                        } else {
                            selectedRover.AttachDevice(drillList[input - 1]);
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        this.RunDrill(drillList[input - 1]);
                        break;
                    case 4:
                        quit = true;
                        break;
                    default:
                        Console.WriteLine("Please select an option from the list.");
                        break;
                }
            } while (quit != true);
        }
        
        public void ViewSolarPanels() {
            int choice = 1;
            int input = new int();
            foreach (SolarPanel solarPanel in this.solarPanelList) {
                Console.Write($"{choice}: ");
                Console.WriteLine(solarPanel.ToString());
                choice += 1;
            }
            input = Convert.ToInt32(Console.ReadLine());
            bool quit = false;
            int selection = new int();
            do {
                Console.WriteLine(solarPanelList[input - 1].ToString());
                Console.WriteLine("1. Attach/detach device\n2. Attach/detach battery\n3. Use device\n4. Cancel");
                selection = Convert.ToInt32(Console.ReadLine());
                switch (selection) {
                    case 1:
                        if (this.selectedRover == solarPanelList[input - 1].rover) {
                            selectedRover.DetachDevice(solarPanelList[input - 1]);
                        } else {
                            selectedRover.AttachDevice(solarPanelList[input - 1]);
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        this.RunSolarPanel(solarPanelList[input - 1]);
                        break;
                    case 4:
                        quit = true;
                        break;
                    default:
                        Console.WriteLine("Please select an option from the list.");
                        break;
                }
            } while (quit != true);
        }
        
        public void ViewRadars() {
            int choice = 1;
            int input = new int();
            foreach (Radar radar in this.radarList) {
                Console.Write($"{choice}: ");
                Console.WriteLine(radar.ToString());
                choice += 1;
            }
            input = Convert.ToInt32(Console.ReadLine());
            bool quit = false;
            int selection = new int();
            do {
                Console.WriteLine(radarList[input - 1].ToString());
                Console.WriteLine("1. Attach/detach device\n2. Attach/detach battery\n3. Use device\n4. Cancel");
                selection = Convert.ToInt32(Console.ReadLine());
                switch (selection) {
                    case 1:
                        if (this.selectedRover == radarList[input - 1].rover) {
                            selectedRover.DetachDevice(radarList[input - 1]);
                        } else {
                            selectedRover.AttachDevice(radarList[input - 1]);
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        this.RunRadar(radarList[input - 1]);
                        break;
                    case 4:
                        quit = true;
                        break;
                    default:
                        Console.WriteLine("Please select an option from the list.");
                        break;
                }
            } while (quit != true);
        }
        
        public void ViewBatteries() {
            int choice = 1;
            foreach (Battery battery in this.batteryList) {
                Console.Write($"{choice}: ");
                Console.WriteLine(battery.ToString());
                choice += 1;
            }
        }
        
        public void RunMotor(Motor motor) {
            ConsoleKeyInfo keypress;
            bool quit = false;
            do {
                this.gameMap.PrintMap();
                Console.WriteLine("Arrow keys to control motor, Esc to quit.");
                keypress = Console.ReadKey(true);
                switch (keypress.Key) {
                    case ConsoleKey.UpArrow:
                        motor.Move(1);
                        break;
                    case ConsoleKey.DownArrow:
                        motor.Move(3);
                        break;
                    case ConsoleKey.LeftArrow:
                        motor.Move(4);
                        break;
                    case ConsoleKey.RightArrow:
                        motor.Move(2);
                        break;
                    case ConsoleKey.Escape:
                        quit = true;
                        break;
                    default:
                        Console.WriteLine("Enter a key, silly.");
                        break;
                }
            } while (quit != true);
        }
        
        public void RunDrill(Drill drill) {
            drill.DrillHere();
        }
        
        public void RunSolarPanel(SolarPanel solarPanel) {
            solarPanel.ChargeBattery();
        }
        
        public void RunRadar(Radar radar) {
            List<Specimen> outputList = radar.Scan();
            foreach (Specimen specimen in outputList) {
                Console.WriteLine(specimen.ToString());
            }
        }
    }
    
    class Program {
        
        public static void Main(String[] args) {
            
            bool mainQuit = false;
            int input = new int();
            
            // World setup
            
            Game game = new Game(20);
            
            // Main loop
            
            do {
                Console.WriteLine($"Mars! Rover: {game.selectedRover.ToString()}");
                game.gameMap.PrintMap();
                Console.WriteLine("1. View Rover, 2. View Devices, 3. View Batteries, 4. Quit");
                input = (Convert.ToInt32(Console.ReadLine()));
                switch (input) {
                    case 1:
                        game.ViewRovers();
                        break;
                    case 2:
                        game.ViewDevices();
                        break;
                    case 3:
                        game.ViewBatteries();
                        break;
                    case 4:
                        mainQuit = true;
                        break;
                }
                
            } while (mainQuit != true);
        }
    }
    
    class ProgramTest {
        public static void Test() {
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
