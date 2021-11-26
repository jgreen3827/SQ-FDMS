/* 
* FILE : AircraftService.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the Aircraft service which holds all of the databse methods related to the aircraft.
*/
using Microsoft.Data.SqlClient;

namespace FDMS.Server.Services
{
    /*
    * NAME : AircraftService
    * PURPOSE : The AircraftService class holds all of the database methods related to the aircraft.
    */
    public class AircraftService
    {
        /* 
        * FUNCTION : CreateAircraft
        * DESCRIPTION :
        *   This function will insert a new aircraft into the aircraft table in the database.
        * PARAMETERS :
        *   string airCraftTailNumber : This is the tail number for the new aircraft to be created.
        * RETURNS :
        *  void : none
        */
        public static void CreateAircraft(string airCraftTailNumber)
        {
            try
            {
                AzureService azureConnection = new AzureService();
                String query = "INSERT INTO dbo.Aircraft (TailNumber) VALUES(@TailNumber)";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TailNumber", airCraftTailNumber);

                        connection.Open();

                        int result = command.ExecuteNonQuery();

                        if (result < 0)
                        {
                            Console.WriteLine("Error inserting data");
                            throw new Exception("Error inserting data");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return;
        }

        /* 
        * FUNCTION : GetAllAircraft
        * DESCRIPTION :
        *   This function will get all of the aircraft tail numbers in the database and return a list of them as a string.
        * PARAMETERS :
        *   none
        * RETURNS :
        *  List<String> : A list of all of the aircrafts in the database.
        */
        public static List<String> GetAllAircraftTailNumbers()
        {
            try
            {
                AzureService azureConnection = new AzureService();
                List<String> tailNumbers = new List<String>();
                String query = "SELECT TailNumber FROM dbo.Aircraft";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            tailNumbers.Add(reader.GetString(0));
                        }

                    }
                }
                return tailNumbers;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null; 
            
        }

        /* 
        * FUNCTION : GetAircraft
        * DESCRIPTION :
        *   This function will get an aircraft from the database based on the one supplied to it.
        * PARAMETERS :
        *   string tailNumber : This is a tail number of the aircraft to retrieve fromt he database.
        * RETURNS :
        *   Shared.Aircraft : A single aircraft from the database based on what was provided
        */
        public static Shared.Aircraft GetAircraft(string tailNumber)
        {
            try
            {
                AzureService azureConnection = new AzureService();
                Shared.Aircraft aircraft = new Shared.Aircraft();
                String query = $"SELECT TailNumber FROM dbo.Aircraft WHERE TailNumber = '{tailNumber}'";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            aircraft.AircraftTailNumber = reader.GetString(0);
                        }

                    }
                }
                return aircraft;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;

        }
    }
}
