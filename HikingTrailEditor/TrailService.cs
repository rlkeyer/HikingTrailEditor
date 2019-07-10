using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HikingTrailEditor
{
    class TrailService
    {
        private string _DataDirectory;
        private string _DataFile = "";

        public TrailService(string DataDirectory)
        {
            try
            {
                if (Directory.Exists(DataDirectory))
                {
                    //get the directory containing the data file
                    _DataDirectory = DataDirectory;
                }
                else
                {
                    //we couldn't find the data directory, throw an error
                    throw new Exception($"InstantiationError: Unable to create the service, Can't find the data directory: {DataDirectory}");
                }

                //derive the filename from the class
                string currentJsonFileName = this.GetType().Name.Replace("Service", "") + ".json";

                //establish the path to the data file
                _DataFile = $@"{DataDirectory}{currentJsonFileName}";
            }
            catch (Exception ex)
            {
                ex.Data.Add("InstantiationError",
                    $"An error occurred while trying to create the service.");
                throw;
            }

        }

        public List<Trail> GetAll() {
            List<Trail> returnValue = new List<Trail>();

            try
            {
                //always make sure the file exists before attempting to access it
                if (File.Exists(_DataFile))
                {
                    //read the file
                    string jsonData = File.ReadAllText(_DataFile);

                    if (!String.IsNullOrEmpty(jsonData))
                    {
                        //deserialize the file back into a list
                        returnValue = JsonConvert.DeserializeObject<List<Trail>>(jsonData);
                    }
                }
                else
                {
                    //we couldn't find the file, throw an error
                    throw new Exception($"GetAllError: Unable to find file: {_DataFile}");
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("GetAllError",
                    $"An error occurred while trying to get players.");
                throw;
            }

            return returnValue;
        }

        public Trail Add(Trail trail, List<Trail> trails)
        {
            //get the next player id
            int newTrailId = GetNextId(trails);

            //assign the playe an id
            trail.Trail_Id = newTrailId;

            //add the player to the list
            trails.Add(trail);

            //save the list
            Save(trails);

            //return the player with the new ID
            return trail;
        }

        public List<Trail> Delete(Trail trail, List<Trail> trails)
        {
            try
            {
                trails.Remove(trail);
                Save(trails);
            }
            catch (Exception ex)
            {
                ex.Data.Add("DeleteError",
                    $"An error occurred while trying to delete a trail. (Trail ID: {trail.Trail_Id}");
                throw;
            }

            return trails;
        }

        public List<Trail> Edit(Trail trail, List<Trail> trails, string newName, string newLocation, int newLength, string newSummary)
        {

            trail.TrailName = newName;
            trail.TrailLocation = newLocation;
            trail.TrailLength = newLength;
            trail.TrailSummary = newSummary;
            Save(trails);

            return trails;
        }

        private void Save(List<Trail> trails)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(trails);

                if (!string.IsNullOrEmpty(jsonData))
                {
                    File.WriteAllText(_DataFile, jsonData);
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("SaveError",
                    $"An error occurred while trying to save the list.");
                throw;
            }
        }

        private int GetNextId(List<Trail> trails)
        {
            int returnValue = 1;

            try
            {
                if (trails.Any())
                {
                    //get the trail with the highest ID
                    var trail = trails.OrderByDescending(u => u.Trail_Id).FirstOrDefault();

                    //get that trail's ID and add 1
                    int id = trail.Trail_Id;
                    id++;
                    returnValue = id;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("GetNextIdError",
                    "An error occurred while trying to get the next trail Id.");
                throw;
            }

            return returnValue;
        }

    }
}
