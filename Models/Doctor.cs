namespace Hospital_San_Vicente.Models; 

public class Doctor
{
    public int Id { get; set; }
    public string Document { get; set; } = string.Empty; 
    public string Name { get; set; } = string.Empty; 
    public string Specialty { get; set; } = string.Empty; 
    public string Phone { get; set; } = string.Empty;  
    public string Email { get; set; } = string.Empty;
 
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}