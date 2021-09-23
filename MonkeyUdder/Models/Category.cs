using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyUdder.Models
{
	public class Category
	{
		public Int32 Id { get; set; }
		public Int32 ParentId { get; set; }

		[Required]
		public String Name { get; set; }
		public ICollection<Category> Categories { get; set; }
	}
}
