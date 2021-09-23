using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyUdder.Models
{
	public class Tag
	{
		public Int32 Id { get; set; }

		[Required]
		public String Fullname { get; set; }

		[Required]
		public String Shortname { get; set; }

		public String PreviewPath { get; set; }
		public String Description { get; set; }
	}
}
