namespace BillingSystem.Bal.BusinessAccess
{
    using System.Linq;
    using Mapper;
    using Model.CustomModel;
    using BillingSystem.Model;

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class SchedulingParametersBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentTypesService"/> class.
        /// </summary>
        public SchedulingParametersBal()
        {
            SchedulingParametersMapper = new SchedulingParametersMapper();
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the appointment types mapper.
        /// </summary>
        /// <value>
        /// The appointment types mapper.
        /// </value>
        private SchedulingParametersMapper SchedulingParametersMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        public SchedulingParametersCustomModel FindById(int? id)
        {
            using (var rep = UnitOfWork.SchedulingParametersRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                var vm = SchedulingParametersMapper.MapModelToViewModel(model);
                return vm;
            }
        }

        public long SaveRecord(SchedulingParametersCustomModel vm)
        {
            using (var rep = UnitOfWork.SchedulingParametersRepository)
            {
                var model = SchedulingParametersMapper.MapViewModelToModel(vm);

                var current = rep.Where(a => a.FacilityId == model.FacilityId).FirstOrDefault();
                if (model.Id > 0 || current != null)
                {
                    current.StartHour = model.StartHour;
                    current.EndHour = model.EndHour;
                    current.ModifiedBy = model.ModifiedBy;
                    current.ModifiedDate = model.ModifiedDate;
                    rep.UpdateEntity(current, model.Id);
                }
                else
                {
                    rep.Create(model);
                }

                return model.Id;
            }
        }

        public SchedulingParametersCustomModel GetDataByFacilityId(long facilityId)
        {
            using (var rep = UnitOfWork.SchedulingParametersRepository)
            {
                var m = rep.Where(x => x.FacilityId == facilityId && x.IsActive).FirstOrDefault();
                if (m != null)
                {
                    var vm = SchedulingParametersMapper.MapModelToViewModel(m);
                    return vm;
                }
                return new SchedulingParametersCustomModel();
            }
        }
        #endregion
    }
}
