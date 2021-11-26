/* 
* FILE : AttitudeService.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the Attitude service which holds all of the databse methods related to the Attitude from the aircraft.
*/
using Microsoft.Data.SqlClient;

namespace FDMS.Server.Services
{
    /*
    * NAME : AttitudeService
    * PURPOSE : The AttitudeService class holds all of the database methods related to the attitude of the aircraft.
    */
    public class AttitudeService
    {
        /* 
        * FUNCTION : CreateAttitude
        * DESCRIPTION :
        *   This function will insert a new attitude into the attitude table in the database.
        * PARAMETERS :
        *   Shared.Attitude attitude : This is the attitude that will need to be added to the database.
        * RETURNS :
        *  int : this will return if the command was successful or not
        */
        public static int CreateAttitude(Shared.Attitude attitude)
        {
            try
            {
                AzureService azureConnection = new AzureService();
                String query = "INSERT INTO dbo.Attitude(Altitude, Pitch, Bank) OUTPUT INSERTED.AttitudeID VALUES(@Altitude, @Pitch, @Bank)";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Altitude", attitude.Altitude);
                        command.Parameters.AddWithValue("@Pitch", attitude.Pitch);
                        command.Parameters.AddWithValue("@Bank", attitude.Bank);

                        connection.Open();

                        int result = (int)command.ExecuteScalar();

                        if (result <= 0 )
                        {
                            Console.WriteLine("Error inserting data");
                            throw new Exception("Error inserting data");
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return 0;
        }

        /* 
        * FUNCTION : GetAttitude
        * DESCRIPTION :
        *   This function will get an attitude from the database based on the one supplied to it.
        * PARAMETERS :
        *   int id : This is the attitude ID of the one to be retrieved from the database
        * RETURNS :
        *   Shared.Attitude : A single attitude from the database based on the id that was provided
        */
        public static Shared.Attitude GetAttitude(int id)
        {
            Shared.Attitude attitude = new Shared.Attitude();

            try
            {
                AzureService azureConnection = new AzureService();
                String query = $"SELECT Altitude,Pitch,Bank FROM attitude WHERE AttitudeID = {id}";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            attitude.AttitudeId = id;
                            attitude.Altitude = reader.GetDouble(0);
                            attitude.Pitch = reader.GetDouble(1);
                            attitude.Bank = reader.GetDouble(2);
                        }

                    }
                    return attitude;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        /* 
        * FUNCTION : GetAllAttitude
        * DESCRIPTION :
        *   This function will get all of the attitudes in the database and return a list of them.
        * PARAMETERS :
        *   none
        * RETURNS :
        *  List<Shared.Attitude> : A list of all of the attitudes in the database.
        */
        public static List<Shared.Attitude> GetAllAttitude()
        {
            List<Shared.Attitude> attitude = new List<Shared.Attitude>();

            try
            {
                AzureService azureConnection = new AzureService();
                String query = $"SELECT AttitudeId,Altitude,Pitch,Bank FROM dbo.Attitude";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            attitude.Add(new Shared.Attitude() { AttitudeId = reader.GetInt32(0), Altitude = reader.GetDouble(1), Pitch = reader.GetDouble(2), Bank = reader.GetDouble(3)});
                        }

                    }
                    return attitude;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
