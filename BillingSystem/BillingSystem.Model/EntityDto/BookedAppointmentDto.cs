using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.EntityDto
{
    public class BookedAppointmentDto
    {
        public int Id { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string AppointmentName { get; set; }

        public string Description { get; set; }

        public int? AppointmentTypeId { get; set; }

        public long? ClinicianId { get; set; }

        public long? PatientId { get; set; }

        public string Status { get; set; }

        public string TimeFrom { get; set; }

        public string TimeTill { get; set; }

        public string Location { get; set; }

        public int? UserId { get; set; }

        public string ClinicianName { get; set; }

        public string TypeOfVisit { get; set; }

        public string DepartmentId { get; set; }

        public int? RoomAssigned { get; set; }

        public int? EquipmentAssigned { get; set; }

        public bool? IsFavorite { get; set; }

        public long? FavoriteId { get; set; }
        public string StatusId { get; set; }
        public string PatientName { get; set; }

        public long CityId { get; set; }
        public string CityName { get; set; }
        public long StateId { get; set; }
        public string StateName { get; set; }
        public long CountryId { get; set; }
        public string CountryName { get; set; }
        public long SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }
        public long FacilityId { get; set; }
        public string FacilityName { get; set; }

        public DateTime AppointmentUtcDateTime { get; set; }
    }
}
