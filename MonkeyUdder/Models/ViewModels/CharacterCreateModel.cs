using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyUdder.Models.ViewModels
{
	public class CharacterCreateModel
	{
		[Required]
		public String Name { get; set; }
		public IFormFile Image { get; set; }
	}
}
