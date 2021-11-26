/* 
* FILE : AzureService.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the Azure service which holds the ability to get the databse from azure we need to connect to.
*/
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FDMS.Server.Services
{
    /*
    * NAME : AzureService
    * PURPOSE : The AzureService class holds the methods to build the azure database info the server needs.
    */
    public class AzureService
    {

        /* 
        * FUNCTION : GetSqlbuilder
        * DESCRIPTION :
        *   This function will provide all of the information needed to connect to the sql db in Azure
        * PARAMETERS :
        *   none
        * RETURNS :
        *  SqlConnectionStringBuilder : this will an sql connection string builder used to connect to the database
        */
        public SqlConnectionStringBuilder GetSqlbuilder()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "fdms.database.windows.net";
            builder.UserID = "fdms";
            builder.Password = "password1!";
            builder.InitialCatalog = "FDMS";

            return builder;
        }
    }
}
