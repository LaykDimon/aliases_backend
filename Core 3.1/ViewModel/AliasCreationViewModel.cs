﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core_3._1.ViewModel
{
    public class AliasCreationViewModel
    {
        [Required]
        public string Domain { get; set; }
    }
}
