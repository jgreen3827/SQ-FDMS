namespace FDMS.Shared.ControllerModels
{
    // Used to contain values as a JSON object for an API call
    public class FilteredAircraftRequest
    {
        // The tail number of the aircraft (also ID)
        public string AircraftTailNumber { get; set; }

        // The start date for which to filter
        public DateTime StartDate { get; set; }

        // The end date for which to filter
        public DateTime EndDate { get; set; }
    }
}
