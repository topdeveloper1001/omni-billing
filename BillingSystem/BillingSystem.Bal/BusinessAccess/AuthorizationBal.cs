using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.Mapper;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BillingSystem.Bal.BusinessAccess
{
    public class AuthorizationBal : BaseBal
    {
        private AuthorizationMapper AuthorizationMapper { get; set; }

        public AuthorizationBal()
        {
            AuthorizationMapper = new AuthorizationMapper();
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<AuthorizationCustomModel> GetAuthorization()
        {
            var list = new List<AuthorizationCustomModel>();
            using (var authorizationRep = UnitOfWork.AuthorizationRepository)
            {
                var authorization = authorizationRep.GetAll().ToList();
                list.AddRange(authorization.Select(item => AuthorizationMapper.MapModelToViewModel(item)));
                return list;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public int AddUptdateAuthorization(Authorization authorization)
        {
            using (var authorizationRep = UnitOfWork.AuthorizationRepository)
            {
                if (authorization.AuthorizationID > 0)
                    authorizationRep.UpdateEntity(authorization, authorization.AuthorizationID);
                else
                    authorizationRep.Create(authorization);
                return authorization.AuthorizationID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="authorizationId"></param>
        /// <returns></returns>
        public Authorization GetAuthorizationById(int? authorizationId)
        {
            using (var authorizationRep = UnitOfWork.AuthorizationRepository)
            {
                var authorization = authorizationRep.Where(x => x.AuthorizationID == authorizationId).FirstOrDefault();
                return authorization;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="authorizationEncounterId">The authorization encounter identifier.</param>
        /// <returns></returns>
        public Authorization GetAuthorizationByEncounterId(string authorizationEncounterId)
        {
            using (var authorizationRep = UnitOfWork.AuthorizationRepository)
            {
                var authorization =
                    authorizationRep.Where(
                        x => x.EncounterID == authorizationEncounterId && (x.IsDeleted == null || x.IsDeleted == false))
                        .FirstOrDefault();
                return authorization;
            }
        }

        /// <summary>
        /// Gets the authorizations by encounter identifier.
        /// </summary>
        /// <param name="authorizationEncounterId">The authorization encounter identifier.</param>
        /// <returns></returns>
        public List<AuthorizationCustomModel> GetAuthorizationsByEncounterId(string authorizationEncounterId)
        {
            var list = new List<AuthorizationCustomModel>();
            using (var authorizationRep = UnitOfWork.AuthorizationRepository)
            {
                var mList = authorizationRep.Where(x => x.EncounterID == authorizationEncounterId).ToList();
                list.AddRange(mList.Select(item => AuthorizationMapper.MapModelToViewModel(item)));
                return list;
            }
        }

        public List<BillHeaderXMLModel> GenerateEAuthorizationFile(int facilityId, string dispositionFlag)
        {
            using (var rep = UnitOfWork.AuthorizationRepository)
            {
                var list = rep.GenerateEAuthorizationFile(facilityId, dispositionFlag);
                return list;
            }
        }

        public async Task<AuthorizationViewData> SaveAuthorizationAsync(AuthorizationCustomModel vm, string docsJson, bool withAuthList, bool withDocs)
        {
            using (var rep = UnitOfWork.AuthorizationRepository)
            {
                var model = AuthorizationMapper.MapViewModelToModel(vm);
                var result = await rep.SaveAsync(model, docsJson, withAuthList, withDocs, vm.MissedEncounterId);
                return result;
            }
        }

        public AuthorizationViewData SaveAuthorizationAsync1(AuthorizationCustomModel vm, string docsJson, bool withAuthList, bool withDocs)
        {
            using (var rep = UnitOfWork.AuthorizationRepository)
            {
                var model = AuthorizationMapper.MapViewModelToModel(vm);
                var result = rep.SaveAuthAsync(model, docsJson, withAuthList, withDocs, vm.MissedEncounterId);
                return result;
            }
        }
    }
}

