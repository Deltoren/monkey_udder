using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyUdder.Models
{
	public class Character
	{
		public Int32 Id { get; set; }

		[Required]
		public String Name { get; set; }
		public String ImagePath { get; set; }
	}
}
