using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyUdder.Models.ViewModels
{
	public class CategoryEditModel
	{
		public Int32? ParentId { get; set; }

		[Required]
		public String Name { get; set; }
	}
}
