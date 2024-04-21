namespace api.Models;

public class GetGoogleApiPeopleResponse 
{
    public List<Locale> Locales { get; init; }
    public List<Name> Names { get; init; }
    public List<Photo> Photos { get; init; }
    public List<Address> Addresses { get; init; }
    public List<EmailAddress> EmailAddresses { get; init; }
    public List<PhoneNumber> PhoneNumbers { get; init; }
}

public class Locale
{
    public string Value { get; init; }
}

public class Name
{
    public string DisplayName { get; init; }
}

public class Photo
{
    public string Url { get; init; }
}

public class Address
{
    public string FormattedValue { get; init; }
}

public class EmailAddress
{
    public string Value { get; init; }
}

public class PhoneNumber
{
    public string CanonicalForm { get; init; }
}