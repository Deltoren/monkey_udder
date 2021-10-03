using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyUdder.Models.ViewModels
{
	public class TagEditModel
	{
		[Required]
		public String Fullname { get; set; }

		[Required]
		public String Shortname { get; set; }

		public IFormFile Image { get; set; }
		public String Description { get; set; }
	}
}
