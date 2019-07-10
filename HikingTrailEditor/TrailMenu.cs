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
                            Console.WriteLine(" Press [enter] to return to the menu.");
                            Console.ReadLine();
                            break;
                        case 2:
                            Add();
                            break;
                        case 3:
                            GetAll();
                            Edit();
                            break;
                        case 4:
                            GetAll();
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

            //create a ConsoleTable object to display the list of trails
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
            

        }

        private static void Add()
        {
            //get the list of trails from TrailService
            _trailList = _trailService.GetAll();

            //instantiate the new trail object
            Trail trail = new Trail();

            //prompt the user for the name of the trail and save the result as TrailName
            Console.Write("Trail Name: ");
            trail.TrailName = Console.ReadLine();

            //prompt the user for the trail location and save the result as TrailLocation
            Console.Write("Trail Location: ");
            trail.TrailLocation = Console.ReadLine();

            //prompt the user for the trail length
            Console.Write("Trail Length (miles): ");

            //create a loop to check that the trail length is greater than 0
            bool success;
            int length;
            do
            {
                success = int.TryParse(Console.ReadLine(), out length);
                //if trail length > 0, save the result as TrailLength
                if (success && length > 0)
                {
                    trail.TrailLength = length;
                }
                //if trail length input is an invalid number prompt the user for another input
                else
                {
                    Console.WriteLine("You did not enter a correct value for length");
                    Console.Write("Trail Length: ");
                }
            } while (success == false || length <= 0);

            //prompt the user for the trail summary and save the result as TrailSummary
            Console.Write("Trail Summary: ");
            trail.TrailSummary = Console.ReadLine();

            //get the current date and time for DateAdded
            trail.DateAdded = DateTime.Now;

            //add the trail using the trail service
            trail = _trailService.Add(trail, _trailList);

            Console.WriteLine($"Success: Added a new trail ID: {trail.Trail_Id}");
            System.Threading.Thread.Sleep(2000);
        }

        private static void Edit()
        {
            //collect the id of the trail to edit
            Console.Write("ID of trail to edit: ");
            int.TryParse(Console.ReadLine(), out var trailEditId);

            //get the list of trails
            _trailList = _trailService.GetAll();
            Trail trailToEdit = _trailList.Find(s => s.Trail_Id == trailEditId);

            //prompt the user for the name of the edited trail and save the result as newName
            Console.Write("Trail Name: ");
            string newName = Console.ReadLine();

            //prompt the user for the trail location and save the result as newLocation
            Console.Write("Trail Location: ");
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
                //if trail length > 0, save the result as newLength
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

            //prompt the user for the trail summary and save the result as newSummary
            Console.Write("Trail Summary: ");
            string newSummary = Console.ReadLine();

            //make sure a trail with the specified id exists before attempting to edit it
            if (trailToEdit != null)
            {
                //edit the trail
                _trailService.Edit(trailToEdit, _trailList, newName, newLocation, newLength, newSummary);

                Console.Write($"Trail ID: {trailEditId} was edited.");
                System.Threading.Thread.Sleep(2000);
            }
            else
            {
                //if a trail with that id doesn't exist show an error
                Console.Write($"ERROR: Could not find trail with ID: {trailEditId}.");
                System.Threading.Thread.Sleep(2000);
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

            //make sure a trail with the specified id exists before attempting to delete it
            if (trailToRemove != null)
            {
                //delete the player
                _trailService.Delete(trailToRemove, _trailList);

                Console.Write($"Trail ID: {trailId} was deleted.");
                System.Threading.Thread.Sleep(2000);
            }
            else
            {
                //if a trail with that id doesn't exist show an error
                Console.Write($"ERROR: Could not find trail with ID: {trailId}.");
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}
