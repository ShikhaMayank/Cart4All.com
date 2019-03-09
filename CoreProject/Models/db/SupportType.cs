﻿using System;
using System.Collections.Generic;

namespace CoreProject.Models.db
{
    public partial class SupportType
    {
        public int Id { get; set; }
        public string Descr { get; set; }
        public int? SupportId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastDateModified { get; set; }
        public int? LastUpdatedBy { get; set; }
    }
}
