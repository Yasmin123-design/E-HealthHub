﻿using E_PharmaHub.Models;

namespace E_PharmaHub.Dtos
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }

        public string DoctorName { get; set; }
        public string UserName { get; set; }
        public string ClinicName { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}
