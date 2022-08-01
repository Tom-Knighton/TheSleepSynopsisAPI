﻿using System;
namespace TheSleepSynopsisAPI.Domain.Models
{
    public class SleepEntry
    {
        public string SleepUUID { get; set; }
        public string UserUUID { get; set; }
        public DateTime SleepStart { get; set; }
        public DateTime SleepEnd { get; set; }
        public int SleepMood { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Dream> Dreams { get; set; }
    }
}

