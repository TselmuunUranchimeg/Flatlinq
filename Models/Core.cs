using Microsoft.AspNetCore.Identity;

namespace Flatlinq.Models;

public class User: IdentityUser
{
	public Tenant? TenantAccount { get; set; } = null;
	public Landlord? LandlordAccount { get; set; } = null;
	public bool BankIdVerified { get; set; } = false;
	public bool IsGoldMember { get; set; } = false;
}

public class BaseClass
{
	public int Id { get; set; } = 0;
	public string UserId { get; set; } = "";
	public User User { get; set; } = null!;
	public ICollection<UserSwipes> Swipes { get; set; } = null!;
}

public class House
{
	public int Id { get; set; } = 0;
	public int Price { get; set; } = 0;
	public string Name { get; set; } = "";
	public string Description { get; set; } = "";
	public bool HasInternet { get; set; } = false;
	public bool HasElectricity { get; set; } = false;
	public bool AllowChildren { get; set; } = false;
	public bool AllowPets { get; set; } = false;
	public bool AllowSmoking { get; set; } = false;
	public ICollection<Tenant> Tenants { get; set; } = null!;
	public int LandlordId { get; set; }
	public Landlord Landlord { get; set; } = null!;
	public string Images { get; set; } = "";
	public ICollection<UserSwipes> Swipes { get; set; } = null!;
}

public class Tenant: BaseClass
{
	public ICollection<House> Purchases { get; set; } = null!;
}

public class Landlord: BaseClass
{
	public ICollection<House> Houses { get; set; } = null!;
}
