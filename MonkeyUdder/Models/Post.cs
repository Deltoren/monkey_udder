using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyUdder.Models
{
	public class Post
	{
		public Int32 Id { get; set; }

		[Required]
		public String Title { get; set; }
		[Required]
		public String ImagePath { get; set; }
		[Required]
		public DateTime Created { get; set; }
		[Required]
		public Int32 Rating { get; set; }
	}
}
