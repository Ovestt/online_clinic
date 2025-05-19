using System;

public class Person
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    public string PhoneNumber { get; set; }
    public string Gender { get; set; } // "M" или "F"
    public string SNILS { get; set; }
    public string RegistrationAddress { get; set; }
    public string ActualAddress { get; set; }

    public string FullName => $"{LastName} {FirstName} {MiddleName}";
    public string ShortGender => Gender == "M" ? "М" : "Ж";
    public int Age => DateTime.Now.Year - BirthDate.Year -
                     (DateTime.Now.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
}
