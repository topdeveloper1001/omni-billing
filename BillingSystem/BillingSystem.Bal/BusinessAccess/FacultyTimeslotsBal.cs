// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyTimeslotsBal.cs" company="">
//   
// </copyright>
// <summary>
//   The faculty timeslots bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    using Microsoft.Ajax.Utilities;

    /// <summary>
    ///     The faculty timeslots bal.
    /// </summary>
    public class FacultyTimeslotsBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FacultyTimeslotsBal" /> class.
        /// </summary>
        public FacultyTimeslotsBal()
        {
            this.FacultyTimeslotsMapper = new FacultyTimeslotsMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the faculty timeslots mapper.
        /// </summary>
        /// <value>
        ///     The faculty timeslots mapper.
        /// </value>
        private FacultyTimeslotsMapper FacultyTimeslotsMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the faculty timeslots list.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="weeknumber">The weeknumber.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public FacultyTimeslotsCustomModelView GetFacultyTimeslotsList(int userid, string weeknumber)
        {
            var savedRecordslist = new List<FacultyTimeslotsCustomModel>();
            var departmentOpeningSlotslist = new List<FacultyTimeslotsCustomModel>();
            using (var facultyTimeslotsRep = UnitOfWork.FacultyTimeslotsRepository)
            {
                #region Saved Records in the DB for the Week
                var lstFacultyTimeslots = facultyTimeslotsRep.Where(a => a.UserID == userid && a.WeekDay == weeknumber).ToList();
                if (lstFacultyTimeslots.Count > 0)
                {
                    savedRecordslist.AddRange(lstFacultyTimeslots.Select(item => FacultyTimeslotsMapper.MapModelToViewModel(item)));
                }
                #endregion


                #region Department Opening slots in week.
                var firstdate = this.FirstDateOfWeekBal(DateTime.Now.Year, Convert.ToInt32(weeknumber), CalendarWeekRule.FirstFourDayWeek);
                var timeslotListlist = new List<FacultyTimeslots>();
                for (var i = 0; i < 7; i++)
                {
                    var datetocheck = firstdate.AddDays(i);
                    var weekdday = new FacultyTimeslots { AvailableDateFrom = datetocheck, AvailableDateTill = datetocheck, UserID = userid };
                    timeslotListlist.Add(weekdday);
                }

                departmentOpeningSlotslist.AddRange(timeslotListlist.Select(item => FacultyTimeslotsMapper.DepartmentOpeningSlotMapModelToViewModel(item)));

                //foreach (var itemcheck in departmentOpeningSlotslist)
                //{
                //    if (
                //        savedRecordslist.Any(
                //            x =>
                //            x.FacultyLunchTimeFrom == itemcheck.FacultyLunchTimeFrom))
                //    {
                //        itemcheck.FacultyLunchTimeFrom = string.Empty;
                //        itemcheck.FacultyLunchTimeTill = string.Empty;
                //    }
                //}
                #endregion
                
            }



            var facultyTimeslotsCustomModelView = new FacultyTimeslotsCustomModelView()
                                                      {
                                                          DepartmentOpeningSlotsList =
                                                              departmentOpeningSlotslist,
                                                          FacultySavedSlotsList =
                                                              savedRecordslist
                                                      };
            return facultyTimeslotsCustomModelView;
        }

        /// <summary>
        /// Saves the faculty timeslots.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public FacultyTimeslotsCustomModelView SaveFacultyTimeslots(List<FacultyTimeslots> model)
        {
            using (var rep = UnitOfWork.FacultyTimeslotsRepository)
            {
                foreach (var item in model)
                {
                    if (item.ID > 0)
                    {
                        var current = rep.GetSingle(item.ID);
                        item.CreatedBy = current.CreatedBy;
                        item.CreatedDate = current.CreatedDate;
                    }

                    var item1 = item;
                    var eventPid =
                        rep.Where(
                            x => x.ID == item1.ID &&
                            x.EventId == item1.EventId && x.AvailableDateTill == item1.AvailableDateTill
                            && x.AvailableDateFrom == item1.AvailableDateFrom).Any();

                    if (eventPid)
                    {
                        rep.Delete(item);
                    }
                }

                if (model[0].ID > 0)
                {
                    rep.Update(model);
                }
                else
                {
                    rep.Create(model);
                }
                var weeknumber = this.GetWeekOfYearISO8601Bal(Convert.ToDateTime(model[0].AvailableDateFrom)).ToString();
                var list = this.GetFacultyTimeslotsList(model[0].UserID, weeknumber);
                return list;
            }
        }


        /// <summary>
        /// Deletes the time slot.
        /// </summary>
        /// <param name="timeslotId">The timeslot identifier.</param>
        /// <returns></returns>
        public bool DeleteTimeSlot(int timeslotId)
        {
            try
            {
                using (var rep = UnitOfWork.FacultyTimeslotsRepository)
                {
                    var valueToDelete = rep.Where(x => x.ID == timeslotId);
                    var deletedrecord = rep.Delete(valueToDelete);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
       
    }
}