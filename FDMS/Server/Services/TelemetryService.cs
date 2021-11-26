/* 
* FILE : TelemetryService.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the Telemetry service which holds all of the database methods related to the Telemetry from the aircraft.
*/
using Microsoft.Data.SqlClient;

namespace FDMS.Server.Services
{
    /*
    * NAME : TelemetryService
    * PURPOSE : The TelemetryService class holds all of the database methods related to the telemetry of the aircraft.
    */
    public class TelemetryService
    {

        /* 
        * FUNCTION : CreateTelemetry
        * DESCRIPTION :
        *   This function will insert a new Telemetry into the Telemetry table in the database.
        * PARAMETERS :
        *   Shared.Telemetry telemetry : This is the telemetry that will need to be added to the database.
        * RETURNS :
        *  int : this will return if the command was successful or not
        */
        public static int CreateTelemetry(Shared.Telemetry telemetry)
        {
            try
            {
                AzureService azureConnection = new AzureService();
                String query = "INSERT INTO dbo.Telemetry (TailNumber, GForceID, AttitudeID,TransmissionTime,TimeOfStorage) OUTPUT INSERTED.AircraftTelemetryID VALUES(@TailNumber,@GForceID,@AttitudeID,@TransmissionTime,@TimeOfStorage)";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TailNumber", telemetry.AircraftTailNumber);
                        command.Parameters.AddWithValue("@GForceID", telemetry.GForceData.GForceId);
                        command.Parameters.AddWithValue("@AttitudeID", telemetry.AttitudeData.AttitudeId);
                        command.Parameters.AddWithValue("@TransmissionTime", telemetry.TimeStamp);
                        command.Parameters.AddWithValue("@TimeOfStorage", DateTime.Now);

                        connection.Open();

                        int result = (int)command.ExecuteScalar();

                        if (result <= 0)
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
        * FUNCTION : GetTelemetry
        * DESCRIPTION :
        *   This function will get a telemetry from the database based on the id supplied.
        * PARAMETERS :
        *   int id : This is the Telemetry ID of the one to be retrieved from the database
        * RETURNS :
        *   Shared.Telemetry : A single telemetry from the database based on the id that was provided
        */
        public static Shared.Telemetry GetTelemetry(int id)
        {
            Shared.Telemetry telemetry = new Shared.Telemetry();

            try
            {
                AzureService azureConnection = new AzureService();
                String query = $"SELECT TailNumber,GForceID,AttitudeID,TransmissionTime FROM dbo.Telemetry WHERE AircraftTelemetryID = {id}";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            telemetry.AircraftTailNumber = reader.GetString(0);
                            telemetry.GForceData.GForceId = reader.GetInt16(1);
                            telemetry.AttitudeData.AttitudeId = reader.GetInt16(2);
                            telemetry.TimeStamp = reader.GetDateTime(3);
                        }

                    }
                    telemetry.AircraftTailNumber = telemetry.AircraftTailNumber.Replace(" ", "");
                    return telemetry;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        /* 
             * FUNCTION : GetTailNumbers
             * DESCRIPTION :
             *   This function will get all of Tail Numbers in the database.
             * PARAMETERS :
             *   none
             * RETURNS :
             *  List<string> : A list of all of the Tail Numbers in the database.
             */
        public static List<string> GetTailNumbers()
        {
            List<string> tailNumbers = new List<string>();

            try
            {
                AzureService azureConnection = new AzureService();
                string query = "SELECT DISTINCT TailNumber FROM dbo.Telemetry";
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
                    return tailNumbers;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        /* 
        * FUNCTION : GetAllTelemetry
        * DESCRIPTION :
        *   This function will get all of the Telemetry in the database for the supplies tailnumber and return a list of them.
        * PARAMETERS :
        *   string tailNumber : This is the tailnumber of the telemetry info that needs to be collected
        * RETURNS :
        *  List<Shared.Telemetry> : A list of all of the telemetry in the database based on the tailnumber provided.
        */
        public static List<Shared.Telemetry> GetTelemetry(string tailNumber)
        {
            List<Shared.Telemetry> telemetry = new List<Shared.Telemetry>();

            try
            {
                AzureService azureConnection = new AzureService();
                string query = $"SELECT TailNumber,GForceID,AttitudeID,TransmissionTime FROM dbo.Telemetry WHERE TailNumber = {tailNumber}";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Shared.GForce gForce = new Shared.GForce();
                            gForce.GForceId = reader.GetInt16(1);

                            Shared.Attitude attitude = new Shared.Attitude();
                            attitude.AttitudeId = reader.GetInt16(2);

                            telemetry.Add(new Shared.Telemetry() { AircraftTailNumber = reader.GetString(0), GForceData = gForce, AttitudeData = attitude, TimeStamp = reader.GetDateTime(3) });
                        }

                    }
                    foreach(Shared.Telemetry tel in telemetry)
                    {
                        tel.AircraftTailNumber = tel.AircraftTailNumber.Replace(" ", "");
                    }
                    return telemetry;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        public static List<Shared.Telemetry> GetTelemetry(string tailNumber, DateTime startDate, DateTime endDate)
        {
            List<Shared.Telemetry> telemetry = new List<Shared.Telemetry>();

            try
            {
                AzureService azureConnection = new AzureService();
                String query = $"SELECT TailNumber,GForceID,AttitudeID,TransmissionTime FROM dbo.Telemetry WHERE TailNumber = '{tailNumber}' AND TransmissionTime BETWEEN '{startDate}' AND '{endDate}'  ";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Shared.GForce gForce = new Shared.GForce();
                            gForce.GForceId = reader.GetInt32(1);

                            Shared.Attitude attitude = new Shared.Attitude();
                            attitude.AttitudeId = reader.GetInt32(2);

                            telemetry.Add(new Shared.Telemetry() { AircraftTailNumber = reader.GetString(0), GForceData = gForce, AttitudeData = attitude, TimeStamp = reader.GetDateTime(3) });
                        }

                    }
                    foreach (Shared.Telemetry tel in telemetry)
                    {
                        tel.AircraftTailNumber = tel.AircraftTailNumber.Replace(" ", "");
                    }
                    return telemetry;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        /* 
        * FUNCTION : GetAllTelemetry
        * DESCRIPTION :
        *   This function will get all of the telemetries in the database and return a list of them.
        * PARAMETERS :
        *   none
        * RETURNS :
        *  List<Shared.Telemetry> : A list of all of the telemetries in the database.
        */
        public static List<Shared.Telemetry> GetAllTelemetry()
        {
            List<Shared.Telemetry> telemetry = new List<Shared.Telemetry>();

            try
            {
                AzureService azureConnection = new AzureService();
                String query = $"SELECT TailNumber,GForceID,AttitudeID,TransmissionTime FROM dbo.Telemetry";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Shared.GForce gForce = new Shared.GForce();
                            gForce.GForceId = reader.GetInt32(1);

                            Shared.Attitude attitude = new Shared.Attitude();
                            attitude.AttitudeId = reader.GetInt32(2);

                            telemetry.Add(new Shared.Telemetry() { AircraftTailNumber = reader.GetString(0), GForceData = gForce, AttitudeData = attitude, TimeStamp = reader.GetDateTime(3) });
                        }

                    }
                    foreach (Shared.Telemetry tel in telemetry)
                    {
                        tel.AircraftTailNumber = tel.AircraftTailNumber.Replace(" ", "");
                    }
                    return telemetry;
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
