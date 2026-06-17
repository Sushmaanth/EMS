using Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Dtos.LeaveRequestDto
{
    public class ApplyLeaveDto
    {
        [Required(ErrorMessage ="Leave Type is required")]
        public LeaveType LeaveType { get; set; }

        [Required(ErrorMessage ="Leave Start data is required")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Leave End date is required")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Reason for leave is required")]
        [StringLength(1000,ErrorMessage ="Reason cannot exceed more than 1000 characters")]
        public string Reason { get; set; }
    }
}
