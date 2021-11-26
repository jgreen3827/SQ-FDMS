/* 
* FILE : GForceService.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the GForce service which holds all of the database methods related to the GForce from the aircraft.
*/
using Microsoft.Data.SqlClient;

namespace FDMS.Server.Services
{
    /*
    * NAME : GForceService
    * PURPOSE : The GForceService class holds all of the database methods related to the GForce of the aircraft.
    */
    public class GForceService
    {
        /* 
        * FUNCTION : CreateGForce
        * DESCRIPTION :
        *   This function will insert a new GForce into the GForce table in the database.
        * PARAMETERS :
        *   Shared.GForce gForce : This is the GForce that will need to be added to the database.
        * RETURNS :
        *  int : this will return if the command was successful or not
        */
        public static int CreateGForce(Shared.GForce gForce)
        {
            try
            {
                AzureService azureConnection = new AzureService();
                String query = "INSERT INTO dbo.GForce (AccelX, AccelY, AccelZ, Weight) OUTPUT INSERTED.GForceID VALUES(@AccelX, @AccelY, @AccelZ, @Weight)";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccelX", gForce.AccelX);
                        command.Parameters.AddWithValue("@AccelY", gForce.AccelY);
                        command.Parameters.AddWithValue("@AccelZ", gForce.AccelZ);
                        command.Parameters.AddWithValue("@Weight", gForce.Weight);

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
        * FUNCTION : GetGForce
        * DESCRIPTION :
        *   This function will get a GForce from the database based on the id supplied.
        * PARAMETERS :
        *   int id : This is the Gforce ID of the one to be retrieved from the database
        * RETURNS :
        *   Shared.GForce : A single GForce from the database based on the id that was provided
        */
        public static Shared.GForce GetGForce(int  id)
        {
            Shared.GForce gForce = new Shared.GForce();

            try
            {
                AzureService azureConnection = new AzureService();
                String query = $"SELECT AccelX,AccelY,AccelZ,Weight FROM dbo.GForce WHERE GForceID = {id}";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            gForce.GForceId = id;
                            gForce.AccelX = reader.GetDouble(0);
                            gForce.AccelY = reader.GetDouble(1);
                            gForce.AccelZ = reader.GetDouble(2);
                            gForce.Weight = reader.GetDouble(3);
                        }

                    }
                    return gForce;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        /* 
        * FUNCTION : GetAllGForce
        * DESCRIPTION :
        *   This function will get all of the GForces in the database and return a list of them.
        * PARAMETERS :
        *   none
        * RETURNS :
        *  List<Shared.GForce> : A list of all of the Gforces in the database.
        */
        public static List<Shared.GForce> GetAllGForce()
        {
            List<Shared.GForce> gForce = new List<Shared.GForce>();

            try
            {
                AzureService azureConnection = new AzureService();
                String query = $"SELECT GForceId,AccelX,AccelY,AccelZ,Weight FROM dbo.GForce";
                using (SqlConnection connection = new SqlConnection(azureConnection.GetSqlbuilder().ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {


                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            gForce.Add(new Shared.GForce() { GForceId = reader.GetInt32(0), AccelX = reader.GetDouble(1), AccelY = reader.GetDouble(2), AccelZ = reader.GetDouble(3), Weight = reader.GetDouble(4) }) ;
                        }

                    }
                    return gForce;
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
