using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ProjectTaskTargetsBal : BaseBal
    {

        private ProjectTaskTargetsMapper ProjectTaskTargetsMapper { get; set; }

        public ProjectTaskTargetsBal()
        {
            ProjectTaskTargetsMapper = new ProjectTaskTargetsMapper();
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ProjectTaskTargetsCustomModel> GetProjectTaskTargetsList(int corporateid, int facilityid)
        {
            var list = new List<ProjectTaskTargetsCustomModel>();
            using (var projectTaskTargetsRep = UnitOfWork.ProjectTaskTargetsRepository)
            {
                var lstProjectTaskTargets = corporateid > 0
                    ? projectTaskTargetsRep.Where(
                        a => a.CorporateId == corporateid && a.FacilityId == facilityid).ToList()
                    : projectTaskTargetsRep.GetAll().ToList();
                list.AddRange(lstProjectTaskTargets.Select(item => ProjectTaskTargetsMapper.MapModelToViewModel(item)));
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<ProjectTaskTargetsCustomModel> SaveProjectTaskTargets(ProjectTaskTargets model)
        {
            using (var rep = UnitOfWork.ProjectTaskTargetsRepository)
            {
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(model.Id);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.Id);
                }
                else
                    rep.Create(model);

                var list = GetProjectTaskTargetsList(model.CorporateId, model.FacilityId);
                return list;
            }
        }

        public List<ProjectTaskTargetsCustomModel> DeleteProjectTaskTargets(int id, int corporateId, int facilityId)
        {
            using (var rep = UnitOfWork.ProjectTaskTargetsRepository)
            {
                rep.Delete(id);
                var list = GetProjectTaskTargetsList(corporateId, facilityId);
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The project task targets identifier.</param>
        /// <returns></returns>
        public ProjectTaskTargets GetProjectTaskTargetsById(int? id)
        {
            using (var rep = UnitOfWork.ProjectTaskTargetsRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                return model;
            }
        }
        /// <summary>
        /// Method will update the task number column of Project Task Target table
        /// </summary>
        /// <param name="oldTaskNumber"></param>
        /// <param name="newTaskNumber"></param>
        /// <param name="projectTaskId"></param>
        /// <returns></returns>
        public bool UpdateProjectTaskTargetTaskNumber(string oldTaskNumber, string newTaskNumber, string projectTaskId)
        {
            bool retValue;
            using (var rep = UnitOfWork.ProjectTaskTargetsRepository)
            {
                retValue = rep.UpdateProjectTaskTargetTaskNumber(oldTaskNumber, newTaskNumber, projectTaskId);
            }
            return retValue;
        }
    }
}
