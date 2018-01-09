namespace SenStaySync.Data.Streamline
{
    public class PropertyOwnerInformation
    {
        [FormField("Select Owner", FormFieldType.Select)]
        public string Name { get; set; }
        [FormField("Company",FormFieldType.Input)]
        public string Company { get; set; }
        [FormField("Address",FormFieldType.Input)]
        public string Address { get; set; }
        [FormField("Address 2",FormFieldType.Input)]
        public string Address2 { get; set; }
        [FormField("City/Area",FormFieldType.Input)]
        public string CityArea { get; set; }
        [FormField("Country",FormFieldType.Select)]
        public string Country { get; set; }
        [FormField("State/Province",FormFieldType.Select)]
        public string StateProvince { get; set; }
        [FormField("Postal code",FormFieldType.Input)]
        public string PostalCode { get; set; }
        [FormField("Home Phone",FormFieldType.Input)]
        public string HomePhone { get; set; }
        [FormField("Home Fax",FormFieldType.Input)]
        public string HomeFax { get; set; }
        [FormField("Business Phone",FormFieldType.Input)]
        public string BusinessPhone { get; set; }
        [FormField("Business Fax",FormFieldType.Input)]
        public string BusinessFax { get; set; }
        [FormField("Mobile Phone",FormFieldType.Input)]
        public string MobilePhone { get; set; }
        [FormField("Local Phone",FormFieldType.Input)]
        public string LocalPhone { get; set; }
        [FormField("Email",FormFieldType.Input)]
        public string Email { get; set; }
        [FormField("From",FormFieldType.Input)]
        public string OwningStartedFrom { get; set; }
        [FormField("Renting Type",FormFieldType.Select)]
        public string RentingType { get; set; }
        [FormField("Auto generate payments for owner statements",FormFieldType.Checkbox)]
        public bool AutoGeneratePayments { get; set; }
        [FormField("Make Check Payable to",FormFieldType.Input)]
        public string MakeCheckPayableTo { get; set; }
        [FormField("Tax ID",FormFieldType.Input)]
        public string TaxId { get; set; }
        [FormField("Bank Account #",FormFieldType.Input)]
        public string BankAccount { get; set; }
        [FormField("Bank Routing #",FormFieldType.Input)]
        public string BankRouting { get; set; }
        [FormField("Name of Bank Account Holder",FormFieldType.Input)]
        public string NameOfBankAccountHolder { get; set; }
        [FormField("Token #",FormFieldType.Input)]
        public string Token { get; set; }
        [FormField("Current balance",FormFieldType.Text)]
        public string ReserveAccountCurrentBalance { get; set; }
    }
}