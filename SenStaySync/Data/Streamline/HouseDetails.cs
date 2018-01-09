namespace SenStaySync.Data.Streamline
{
    using System;

    public class HouseDetails
    {
        [FormField("Status", FormFieldType.Text)]
        public string Status { get; set; }

        [FormField("Created", FormFieldType.Text)]
        public string Created { get; set; }

        [FormField("Property Id", FormFieldType.Text)]
        public string PropertyId { get; set; }

        [FormField("Business Card Id", FormFieldType.Text)]
        public string BusinessCardId { get; set; }

        [FormField("ICal Link", FormFieldType.Text)]
        public string ICalLink { get; set; }

        [FormField("Required Guest Security Deposit", FormFieldType.Input)]
        public string RequiredGuestSecurityDeposit { get; set; }

        [FormField("Group", FormFieldType.Select)]
        public string Group { get; set; }

        [FormField("Pricing Group", FormFieldType.Select)]
        public string PricingGroup { get; set; }

        [FormField("Floor", FormFieldType.Select)]
        public string Floor { get; set; }

        [FormField("Max Occupants", FormFieldType.Select)]
        public string MaxOccupants { get; set; }

        [FormField("Standard Occupancy", FormFieldType.Select)]
        public string StandardOccupancy { get; set; }

        [FormField("Max Pets", FormFieldType.Select)]
        public string MaxPets { get; set; }

        [FormField("Bedrooms #", FormFieldType.Select)]
        public string Bedrooms { get; set; }

        [FormField("Bathrooms #", FormFieldType.Select)]
        public string Bathrooms { get; set; }

        [FormField("Approximate Sq. Ft.", FormFieldType.Input)]
        public string ApproximateSqFt { get; set; }

        [FormField("View", FormFieldType.Select)]
        public string View { get; set; }

        [FormField("Home Name", FormFieldType.Input)]
        public string HomeName { get; set; }

        [FormField("Locations/Resort", FormFieldType.Select)]
        public string LocationsResort { get; set; }

        [FormField("Area", FormFieldType.Select)]
        public string Area { get; set; }

        [FormField("Neighborhood", FormFieldType.Select)]
        public string Neighborhood { get; set; }

        [FormField("HomeType", FormFieldType.Select)]
        public string HomeType { get; set; }

        [FormField("Rating", FormFieldType.Select)]
        public string Rating { get; set; }

        [FormField("Local Phone", FormFieldType.Input)]
        public string LocalPhone { get; set; }

        [FormField("WiFi Secret Key", FormFieldType.Input)]
        public string WiFiSecretKey { get; set; }

        [FormField("Lock Box Code", FormFieldType.Input)]
        public string LockBoxCode { get; set; }

        [FormField("Season Group", FormFieldType.Select)]
        public string SeasonGroup { get; set; }


        [FormField("Address", FormFieldType.Input)]
        public string Address { get; set; }

        [FormField("City/Area", FormFieldType.Input)]
        public string CityArea { get; set; }

        [FormField("Country", FormFieldType.Select)]
        public string Country { get; set; }

        [FormField("State/Province", FormFieldType.Select)]
        public string StateProvince { get; set; }

        [FormField("Postal code", FormFieldType.Input)]
        public string PostalCode { get; set; }

        [FormField("Latitude", FormFieldType.Input)]
        public string Latitude { get; set; }

        [FormField("Longitude", FormFieldType.Input)]
        public string Longitude { get; set; }

        [FormField("Comment", FormFieldType.TextArea)]
        public string Comment { get; set; }

        [FormField("Message to Maintenance", FormFieldType.TextArea)]
        public string MessageToMaintenance { get; set; }

        [FormField("Renting Type", FormFieldType.Select)]
        public string RentingType { get; set; }

        [FormField("Virtual Tour URL or Embedd Code [sample", FormFieldType.TextArea)]
        public string VirtualTourUrl { get; set; }

        [FormField("Virtual Tour Image Overlay URL to Image", FormFieldType.TextArea)]
        public string VirtualTourImageOverlayUrl { get; set; }

        [FormField("Floor Plan URL or Embedd Code", FormFieldType.TextArea)]
        public string FloorPlanUrl { get; set; }

        [FormField("Google Map Overlay URL to Image", FormFieldType.TextArea)]
        public string GoogleMapOverlayUrl { get; set; }

        [FormField("HouseKeeper", FormFieldType.Select)]
        public string Housekeeper { get; set; }

        [FormField("Default Commisssion", FormFieldType.Input)]
        public string DefaultComission { get; set; }

        [FormField("WiFi Network", FormFieldType.Input)]
        public string WiFiNetwork { get; set; }

        [FormField("Community Gate Code", FormFieldType.Input)]
        public string CommunityGateCode { get; set; }

        [FormField("Community Access Code", FormFieldType.Input)]
        public string CommunityAccessCode { get; set; }

        [FormField("Guidelines", FormFieldType.TextArea)]
        public string Guidelines { get; set; }

        [FormField("Agent Name", FormFieldType.Input)]
        public string AgentName { get; set; }

        [FormField("Agent Email", FormFieldType.Input)]
        public string AgentEmail { get; set; }

        [FormField("Agent Info", FormFieldType.TextArea)]
        public string AgentInfo { get; set; }

        [FormField("Airbnb Name", FormFieldType.Input)]
        public string AirbnbName { get; set; }

        [FormField("Lease Holder", FormFieldType.Input)]
        public string LeaseHolder { get; set; }

        [FormField("Airbnb Listing URL", FormFieldType.Input)]
        public string AirBnbListingUrl { get; set; }

        [FormField("Airbnb Account Email", FormFieldType.Input)]
        public string AirBnbAccountEmail { get; set; }

        [FormField("Parking Info", FormFieldType.TextArea)]
        public string ParkingInfo { get; set; }

        [FormField("Key Cafe Directions", FormFieldType.TextArea)]
        public string KeyCafeDirections { get; set; }

        [FormField("Key Cafe", FormFieldType.Input)]
        public string KeyCafe { get; set; }

        [FormField(Name = "short_description", Type = FormFieldType.TextArea)]
        public string ShortDescription { get; set; }

        [FormField(Name = "off_site_description", Type = FormFieldType.TextArea)]
        public string OffSiteDescription { get; set; }

        [FormField(Name = "lock_box_directions", Type = FormFieldType.TextArea)]
        public string LockBoxDirections { get; set; }

    }
}