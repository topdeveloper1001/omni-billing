using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Threading.Tasks;
using BillingSystem.Repository.Interfaces;
using System.Data.SqlClient;
using AutoMapper;
using BillingSystem.Repository.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRepository<Authorization> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public AuthorizationService(IRepository<Authorization> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<AuthorizationCustomModel> GetAuthorization()
        {
            var list = new List<AuthorizationCustomModel>();
            var lst = _repository.GetAll().ToList();
            list = lst.Select(x => _mapper.Map<AuthorizationCustomModel>(x)).ToList();
            //list.AddRange(authorization.Select(item => AuthorizationMapper.MapModelToViewModel(item)));
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public int AddUptdateAuthorization(Authorization authorization)
        {
            if (authorization.AuthorizationID > 0)
                _repository.UpdateEntity(authorization, authorization.AuthorizationID);
            else
                _repository.Create(authorization);
            return authorization.AuthorizationID;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="authorizationId"></param>
        /// <returns></returns>
        public Authorization GetAuthorizationById(int? authorizationId)
        {
            var authorization = _repository.Where(x => x.AuthorizationID == authorizationId).FirstOrDefault();
            return authorization;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="authorizationEncounterId">The authorization encounter identifier.</param>
        /// <returns></returns>
        public Authorization GetAuthorizationByEncounterId(string authorizationEncounterId)
        {
            var authorization =
                _repository.Where(
                    x => x.EncounterID == authorizationEncounterId && (x.IsDeleted == null || x.IsDeleted == false))
                    .FirstOrDefault();
            return authorization;
        }

        /// <summary>
        /// Gets the authorizations by encounter identifier.
        /// </summary>
        /// <param name="authorizationEncounterId">The authorization encounter identifier.</param>
        /// <returns></returns>
        public List<AuthorizationCustomModel> GetAuthorizationsByEncounterId(string authorizationEncounterId)
        {
            var list = new List<AuthorizationCustomModel>();
            var mList = _repository.Where(x => x.EncounterID == authorizationEncounterId).ToList();
            list = mList.Select(x => _mapper.Map<AuthorizationCustomModel>(x)).ToList();
            return list;

        }

        public List<BillHeaderXMLModel> GenerateEAuthorizationFile(int facilityId, string dispositionFlag)
        {
            var spName = string.Format("EXEC {0} @SenderID, @DispositionFlag", StoredProcedures.GenerateEAuthorizationFile);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("SenderID", facilityId);
            sqlParameters[1] = new SqlParameter("DispositionFlag", dispositionFlag);
            IEnumerable<BillHeaderXMLModel> result = _context.Database.SqlQuery<BillHeaderXMLModel>(spName, sqlParameters);
            return result.ToList();
        }

        public async Task<AuthorizationViewData> SaveAuthorizationAsync(AuthorizationCustomModel vm, string docsJson, bool withAuthList, bool withDocs)
        {
            var m = _mapper.Map<Authorization>(vm);
            var sqlParams = new SqlParameter[23];
            sqlParams[0] = new SqlParameter("@Id", m.AuthorizationID);
            sqlParams[1] = new SqlParameter("@pCId", m.CorporateID);
            sqlParams[2] = new SqlParameter("@pFId", m.FacilityID);
            sqlParams[3] = new SqlParameter("@pPId", m.PatientID);
            sqlParams[4] = new SqlParameter("@pEId", m.EncounterID);
            sqlParams[5] = new SqlParameter("@pDateOrdered", m.AuthorizationDateOrdered == null ? m.CreatedDate : m.AuthorizationDateOrdered);
            sqlParams[6] = new SqlParameter("@pStart", m.AuthorizationStart);
            sqlParams[7] = new SqlParameter("@pEnd", m.AuthorizationEnd);
            sqlParams[8] = new SqlParameter("@pCode", m.AuthorizationCode);
            sqlParams[9] = new SqlParameter("@pType", Convert.ToInt32(m.AuthorizationType));
            sqlParams[10] = new SqlParameter("@pComments", string.IsNullOrEmpty(m.AuthorizationComments) ? string.Empty : m.AuthorizationComments);
            sqlParams[11] = new SqlParameter("@pDenialCode", Convert.ToInt32(m.AuthorizationDenialCode));
            sqlParams[12] = new SqlParameter("@pIDPayer", string.IsNullOrEmpty(m.AuthorizationIDPayer) ? string.Empty : m.AuthorizationIDPayer);
            sqlParams[13] = new SqlParameter("@pLimit", m.AuthorizationLimit);
            sqlParams[14] = new SqlParameter("@pMemberId", string.IsNullOrEmpty(m.AuthorizationMemberID) ? string.Empty : m.AuthorizationMemberID);
            sqlParams[15] = new SqlParameter("@pResult", Convert.ToInt32(m.AuthorizationResult));
            sqlParams[16] = new SqlParameter("@pUserId", m.CreatedBy);
            sqlParams[17] = new SqlParameter("@pCreatedDate", m.CreatedDate);
            sqlParams[18] = new SqlParameter("@pServiceLevel", m.AuthorizedServiceLevel);
            sqlParams[19] = new SqlParameter("@pFilesDataInJson", docsJson);
            sqlParams[20] = new SqlParameter("@pWithAuthList", withAuthList);
            sqlParams[21] = new SqlParameter("@pWithDocs", withDocs);
            sqlParams[22] = new SqlParameter("@pMissedEId", vm.MissedEncounterId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveAuthorization.ToString(), false, parameters: sqlParams))
            {
                if (vm.MissedEncounterId > 0)
                {
                    var statusEn = (await r.ResultSetForAsync<int>()).FirstOrDefault();
                }

                var viewData = new AuthorizationViewData
                {
                    AuthorizationId = (await r.ResultSetForAsync<int>()).FirstOrDefault()
                };

                if (withAuthList)
                    viewData.AuthList = (await r.ResultSetForAsync<AuthorizationCustomModel>()).ToList();

                if (withDocs)
                {
                    var docs = (await r.ResultSetForAsync<DocumentsTemplates>()).ToList();
                    if (docs.Any())
                        viewData.Docs = docs.Where(a => a.DocumentName.Equals("Authorization File")).ToList();
                }
                return viewData;
            }
        }

        public AuthorizationViewData SaveAuthorizationAsync1(AuthorizationCustomModel vm, string docsJson, bool withAuthList, bool withDocs)
        {
            var m = _mapper.Map<Authorization>(vm);
            var sqlParams = new SqlParameter[23];
            sqlParams[0] = new SqlParameter("@Id", m.AuthorizationID);
            sqlParams[1] = new SqlParameter("@pCId", m.CorporateID);
            sqlParams[2] = new SqlParameter("@pFId", m.FacilityID);
            sqlParams[3] = new SqlParameter("@pPId", m.PatientID);
            sqlParams[4] = new SqlParameter("@pEId", m.EncounterID);
            sqlParams[5] = new SqlParameter("@pDateOrdered", m.AuthorizationDateOrdered == null ? m.CreatedDate : m.AuthorizationDateOrdered);
            sqlParams[6] = new SqlParameter("@pStart", m.AuthorizationStart);
            sqlParams[7] = new SqlParameter("@pEnd", m.AuthorizationEnd);
            sqlParams[8] = new SqlParameter("@pCode", m.AuthorizationCode);
            sqlParams[9] = new SqlParameter("@pType", Convert.ToInt32(m.AuthorizationType));
            sqlParams[10] = new SqlParameter("@pComments", string.IsNullOrEmpty(m.AuthorizationComments) ? string.Empty : m.AuthorizationComments);
            sqlParams[11] = new SqlParameter("@pDenialCode", Convert.ToInt32(m.AuthorizationDenialCode));
            sqlParams[12] = new SqlParameter("@pIDPayer", string.IsNullOrEmpty(m.AuthorizationIDPayer) ? string.Empty : m.AuthorizationIDPayer);
            sqlParams[13] = new SqlParameter("@pLimit", m.AuthorizationLimit);
            sqlParams[14] = new SqlParameter("@pMemberId", string.IsNullOrEmpty(m.AuthorizationMemberID) ? string.Empty : m.AuthorizationMemberID);
            sqlParams[15] = new SqlParameter("@pResult", Convert.ToInt32(m.AuthorizationResult));
            sqlParams[16] = new SqlParameter("@pUserId", m.CreatedBy);
            sqlParams[17] = new SqlParameter("@pCreatedDate", m.CreatedDate);
            sqlParams[18] = new SqlParameter("@pServiceLevel", m.AuthorizedServiceLevel);
            sqlParams[19] = new SqlParameter("@pFilesDataInJson", docsJson);
            sqlParams[20] = new SqlParameter("@pWithAuthList", withAuthList);
            sqlParams[21] = new SqlParameter("@pWithDocs", withDocs);
            sqlParams[22] = new SqlParameter("@pMissedEId", vm.MissedEncounterId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveAuthorization.ToString(), false, parameters: sqlParams))
            {
                if (vm.MissedEncounterId > 0)
                {
                    var statusEn = r.ResultSetFor<int>().FirstOrDefault();
                }

                var viewData = new AuthorizationViewData
                {
                    AuthorizationId = r.ResultSetFor<int>().FirstOrDefault()
                };

                if (withAuthList)
                    viewData.AuthList = r.ResultSetFor<AuthorizationCustomModel>().ToList();

                if (withDocs)
                {
                    var docs = r.ResultSetFor<DocumentsTemplates>().ToList();
                    if (docs.Any())
                        viewData.Docs = docs.Where(a => a.DocumentName.Equals("Authorization File")).ToList();
                }
                return viewData;
            }
        }
    }
}

