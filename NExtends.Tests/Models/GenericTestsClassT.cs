﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NExtends.Tests.Models
{
	public class GenericTestsClassT : GenericTestsInterfaceI
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string CustomT { get; set; }
		public string UnexpectedCommonName { get; set; }
	}
}
