namespace AuthService;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime Created_at { get; set; }
    public DateTime Updated_at { get; set; }
    public bool Is_banned { get; set; }
}

public class Profile
{
    public Guid Id { get; set; }
    public Guid User_Id { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    //public string Region { get; set; }
}

public class Character
{
    public Guid Id { get; set; }
    public Guid Profile_Id { get; set; }
    public long Character_Def_Id { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
}

public class Character_Inventory
{
    public Guid Id { get; set; }
    public Guid Character_Id { get; set; }
    public Guid Item_Id { get; set; }
}

public class Inventory
{
    public Guid Id { get; set; }
    public Guid Profile_Id { get; set; }
    public long Item_Def_Id { get; set; }
    public int Quantity { get; set; }
}