using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Belarus1.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BelarusUser class
public class BelarusUser : IdentityUser
{
	[PersonalData]
	public string ? Name { get; set; }
	[PersonalData]
	public DateTime DOB { get; set; }
}

