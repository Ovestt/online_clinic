using System;

public class MedicalReferralModel
{
    public int ReferralID { get; set; }
    public string ReferralNumber { get; set; }
    public int PatientID { get; set; }
    public string PatientName { get; set; }
    public int DoctorUserID { get; set; }
    public string DoctorName { get; set; }
    public DateTime ReferralDate { get; set; } = DateTime.Now;
    public string Purpose { get; set; }
    public string Speciality { get; set; }
    public string ServiceType { get; set; }
    public int? DiagnosisID { get; set; }
    public string DiagnosisName { get; set; }
}

public class DiagnosisItem
{
    public int ID { get; set; }
    public string Name { get; set; }
}