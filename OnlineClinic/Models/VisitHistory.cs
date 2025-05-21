using System;

public class VisitHistory
{
    public int VisitID { get; set; }
    public string DoctorName { get; set; }   
    public string Speciality { get; set; }     
    public DateTime VisitDateTime { get; set; }  
    public string Notes { get; set; }          
    public string PatientName { get; set; }     

    public string VisitDate => VisitDateTime.ToString("dd.MM.yyyy");
    public string VisitTime => VisitDateTime.ToString("HH:mm");
}