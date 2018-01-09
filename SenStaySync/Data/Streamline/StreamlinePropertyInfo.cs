namespace SenStaySync.Data
{
    using System;
    using System.Collections.Generic;
    using Streamline;

    public class FormFieldAttribute : Attribute
    {
        public FormFieldAttribute()
        {
            Type = FormFieldType.Text;
        }

        public FormFieldAttribute(string fieldName, FormFieldType type)
        {
            Field = fieldName;
            Type = type;
        }

        public string Field { get; set; }
        public FormFieldType Type { get; set; }

        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class StreamlinePropertyInfo
    {
        public string AirBnbExacName; // example "LUXURY 2BR Condo, Sunset Strip! L04"

        public string SenStayID; // example "LA017"
        public int StreamlineBusinessCardID; // example "48261"
        public int StreamlineEditID; // example "23451"
        public int StreamlinePropertyID; // example "67856"
        public string StreamlinePropertyName; // example "LA004 Sunset Strip 2 Bedroom Luxury Condo LA004"

        public HouseDetails HouseDetails { get; set; }
        public PropertyOwnerInformation OwnerInformation { get; set; }
        public Dictionary<string, bool> CustomAmenities { get; set; }
        public Dictionary<string, bool> TaxesAndFees { get; set; }
        public Dictionary<string, bool> HaVrboAmenities { get; set; }
    }
}