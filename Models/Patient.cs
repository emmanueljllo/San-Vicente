using System.Collections.Generic;
using Hospital_San_Vicente.Models;
public class Patient 
{ 
    public int Id { get; set; } 
    public string Document { get; set; } 
    public string Name { get; set; } 
    public int Age { get; set; } 
    public string Phone { get; set; } 
    public string Email { get; set; } 
    
    // Navigation property
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>(); 
}