using System;

public class User
{
    public int UserID { get; set; }
    public string LastName { get; set; }       // Фамилия
    public string FirstName { get; set; }      // Имя
    public string MiddleName { get; set; }     // Отчество
    public string Role { get; set; }           // Роль
    public DateTime BirthDate { get; set; }    // Дата рождения
    public string Login { get; set; }          // Логин
    public string PasswordHash { get; set; }   // Хэш пароля
    public string Speciality { get; set; }

    public string FullName => $"{LastName} {FirstName} {MiddleName}";
}