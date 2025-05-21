using System;

public class UpcomingVisit
{
    public int VisitID { get; set; }
    public int PatientID { get; set; }
    public string PatientFullName { get; set; }
    public DateTime VisitDateTime { get; set; }
    public string Notes { get; set; }
    public string Speciality { get; set; }
    public string PatientPhone { get; set; }
    public string PatientBirthDate { get; set; }

    public string VisitDate => VisitDateTime.ToString("dd.MM.yyyy");
    public string VisitTime => VisitDateTime.ToString("HH:mm");
    public string TimeUntilVisit
    {
        get
        {
            var timeLeft = VisitDateTime - DateTime.Now;
            if (timeLeft.TotalDays >= 1)
                return $"Через {(int)timeLeft.TotalDays} дн.";
            else if (timeLeft.TotalHours >= 1)
                return $"Через {(int)timeLeft.TotalHours} час.";
            else if (timeLeft.TotalMinutes >= 1)
                return $"Через {(int)timeLeft.TotalMinutes} мин.";
            else if (timeLeft.TotalSeconds >= 0)
                return "Сейчас";
            else
                return "Просрочено";
        }
    }
}