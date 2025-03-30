namespace Domain.DBEntities;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public bool IsActive { get; set; }
}