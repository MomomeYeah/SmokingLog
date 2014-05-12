using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace SmokingLog.Models
{
    public class Log
    {
        public int ID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Log Date")]
        public DateTime logDate { get; set; }

        [Required]
        [Display(Name = "Number of Cigarettes")]
        public int numberOfCigarettes { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}