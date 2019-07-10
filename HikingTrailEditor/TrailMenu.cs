using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HikingTrailEditor
{
    public class TrailMenu
    {
        private static string _dataDir;
        private static TrailService _trailService;

        public static List<Trail> _trailList { get; set; }

        public static int DisplayMenu()
        {
            // Save the path of the json file
            _dataDir = $@"{Program.DataDirectory}Data\";
            _trailService = new TrailService(_dataDir);

            Console.Clear();
            Console.SetWindowSize(170, 30);
            Console.WriteLine("--------------------");
            Console.WriteLine("Hiking Trail Editor");
            Console.WriteLine("--------------------");
            Console.WriteLine();
            Console.WriteLine(" 1. List Trails");
            Console.WriteLine(" 2. Add Trail");
            Console.WriteLine(" 3. Edit Trail");
            Console.WriteLine(" 4. Delete Trail");
            Console.WriteLine(" 5. Exit");
            Console.WriteLine();
            Console.Write("Choice: ");

            var result = Console.ReadLine();
            
            return Convert.ToInt32(result);

        }

        public static void Run()
        {
            int userInput = 1;
            do
            {
                try
                {
                    userInput = DisplayMenu();
                    // Use a switch statement to decide what action to take based on the user input from the main menu
                    switch (userInput)
                    {
                        case 1:
                            GetAll();
                            break;
                        case 2:
                            Add();
                            break;
                        case 3:
                            Edit();
                            break;
                        case 4:
                            Delete();
                            break;
                        case 5:
                            Console.WriteLine("Closing the program......");
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine();
                            Console.WriteLine(" Error: Invalid Choice");
                            System.Threading.Thread.Sleep(1000);
                            break;
                    }
                }

                catch (Exception e)
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine(" Unexpected Error:");
                    Console.WriteLine(e);
                    System.Threading.Thread.Sleep(2000);
                }
            } while (userInput != 5);
        }

        private static void GetAll()
        {
            //get the list of trails from TrailService
            _trailList = _trailService.GetAll();

            //create a ConsoleTable object for displaying
            //output like a table
            ConsoleTable ct = new ConsoleTable(90);

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Trails");
            ct.PrintLine();

            string[] headers = new[] { "Id", "Name", "Location", "Length", "Summary" };
            ct.PrintRow(headers);
            ct.PrintLine();

            if (_trailList.Any())
            {
                foreach (var trail in _trailList)
                {
                    string[] rowData = new[] { trail.Trail_Id.ToString(), trail.TrailName, trail.TrailLocation, trail.TrailLength.ToString(), trail.TrailSummary };
                    ct.PrintRow(rowData);
                }
            }
            else
            {
                Console.WriteLine(" There are no trails to list. Try adding a trail.");
            }

            ct.PrintLine();

            Console.WriteLine();
            Console.WriteLine(" Press [enter] to return to the menu.");
            Console.ReadLine();

        }

        private static void Add()
        {
            //get the existing list of all trails
            _trailList = _trailService.GetAll();

            //instantiate the new trail object
            Trail trail = new Trail();

            //prompt the user for the name of the trail
            Console.Write("Trail Name: ");
            //read the input from the console as the trail name
            trail.TrailName = Console.ReadLine();

            //prompt the user for the trail location
            Console.Write("Trail Location: ");
            //read the input from the console as the trail location
            trail.TrailLocation = Console.ReadLine();

            //prompt the user for the trail length
            Console.Write("Trail Length (miles): ");

            //create a loop to check that the trail length is greater than 0
            bool success;
            int length;
            do
            {
                success = int.TryParse(Console.ReadLine(), out length);
                //if trail length > 0, read the input as the trail location
                if (success && length > 0)
                {
                    trail.TrailLength = length;
                }
                //if trail length is an invalid number prompt the user for another input
                else
                {
                    Console.WriteLine("You did not enter a correct value for length");
                    Console.Write("Trail Length: ");
                }
            } while (success == false || length <= 0);

            //prompt the user for the trail summary
            Console.Write("Trail Summary: ");
            //read the input from the console as the trail summary
            trail.TrailSummary = Console.ReadLine();


            //get the current date and time for DateAdded
            trail.DateAdded = DateTime.Now;

            //call the player service and add the player
            trail = _trailService.Add(trail, _trailList);

            //give the user feed back--pause for one second on screen
            Console.WriteLine($"Success: Added a new trail ID: {trail.Trail_Id}");
            System.Threading.Thread.Sleep(1000);
        }

        private static void Edit()
        {
            //collect the id of the trail to edit
            Console.Write("ID of trail to edit: ");
            int.TryParse(Console.ReadLine(), out var trailEditId);

            //get the list of trails
            _trailList = _trailService.GetAll();
            Trail trailToEdit = _trailList.Find(s => s.Trail_Id == trailEditId);

            //prompt the user for the name of the edited trail
            Console.Write("Trail Name: ");
            //read the input from the console as the edited trail name
            string newName = Console.ReadLine();

            //prompt the user for the trail location
            Console.Write("Trail Location: ");
            //read the input from the console as the trail location
            string newLocation = Console.ReadLine();

            //prompt the user for the trail length
            Console.Write("Trail Length (miles): ");

            //create a loop to check that the trail length is greater than 0
            bool success;
            int length;
            int newLength = 1;
            do
            {
                success = int.TryParse(Console.ReadLine(), out length);
                //if trail length > 0, read the input as the trail location
                if (success && length > 0)
                {
                    newLength = length;
                }
                //if trail length is an invalid number prompt the user for another input
                else
                {
                    Console.WriteLine("You did not enter a correct value for length");
                    Console.Write("Trail Length: ");
                }
            } while (success == false || length <= 0);

            //prompt the user for the trail summary
            Console.Write("Trail Summary: ");
            //read the input from the console as the trail summary
            string newSummary = Console.ReadLine();

            //make sure a trail with the specified id exists
            //before attempting to edit it
            if (trailToEdit != null)
            {
                //edit the trail
                _trailService.Edit(trailToEdit, _trailList, newName, newLocation, newLength, newSummary);

                //give feedback to the user and pause for one second
                Console.Write($"Trail ID: {trailEditId} was edited.");
                System.Threading.Thread.Sleep(1000);
            }
            else
            {
                //could not find the specified player show and error and pause for a second
                Console.Write($"ERROR: Could not find trail with ID: {trailEditId}.");
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void Delete()
        {
            //collect the id of the trail to delete
            Console.Write("ID of trail to delete: ");
            int.TryParse(Console.ReadLine(), out var trailId);

            //get the list of trails
            _trailList = _trailService.GetAll();
            var trailToRemove = _trailList.SingleOrDefault(s => s.Trail_Id == trailId);

            //make sure a trail with the specified id exists
            //before attempting to delete it
            if (trailToRemove != null)
            {
                //delete the player
                _trailService.Delete(trailToRemove, _trailList);

                //give feedback to the user and pause for one second
                Console.Write($"Trail ID: {trailId} was deleted.");
                System.Threading.Thread.Sleep(1000);
            }
            else
            {
                //could not find the specified player show and error and pause for a second
                Console.Write($"ERROR: Could not find trail with ID: {trailId}.");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
