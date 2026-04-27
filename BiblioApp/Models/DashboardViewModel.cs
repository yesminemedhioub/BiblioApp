namespace BiblioApp.Models;

public class DashboardViewModel
{
    public int NombreLivres { get; set; }
    public int NombreAuteurs { get; set; }
    public int EmpruntsEnCours { get; set; }
    public List<Emprunt> DerniersEmprunts { get; set; } = new();
}