﻿
using System;
using System.Linq;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace BillingSystem.UnitTests
{

    /// <summary>
    ///     The xml upload test.
    /// </summary>
    [TestClass]
    public class XMLUploadTest
    {
        private readonly IBillHeaderService _blService;
        private readonly IBillActivityService _baService;
        #region Fields

        /// <summary>
        /// The corporate id.
        /// </summary>
        private readonly int corporateId;

        /// <summary>
        /// The facility id.
        /// </summary>
        private readonly int facilityId;

        /// <summary>
        ///     The model.
        /// </summary>
        private readonly string xmlFilePath =
            @"D:\Projects\OmniBilling\OmniBillingV3.0\BillingSystem\BillingSystem.UnitTests\XmlPath\TestXMlNEW_29.xml";

        #endregion

        #region Constructors and Destructors

        public XMLUploadTest(IBillHeaderService blService, IBillActivityService baService, int corporateId, int facilityId, string xmlFilePath)
        {
            _blService = blService;
            _baService = baService;
            this.corporateId = this.GetRandomCorporateId();
            this.facilityId = this.GetRandomFacilityId(this.corporateId);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get random corporate id.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetRandomCorporateId()
        {
            return 0;
            //var corporatebal = new CorporateService();
            //Corporate randomselectedCorporate = corporatebal.GetAllCorporate().PickRandom();
            //return randomselectedCorporate.CorporateID;
        }

        /// <summary>
        /// The get random facility id.
        /// </summary>
        /// <param name="cId">
        /// The c id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetRandomFacilityId(int cId)
        {
            var facilitybal = new FacilityBal();
            var randomselectedFacility = facilitybal.GetFacilitiesByCorpoarteId(cId).PickRandom();
            return randomselectedFacility.FacilityId;
        }

        /// <summary>
        ///     The upload xml.
        /// </summary>
        [TestMethod]
        public void UploadXML()
        {
            string xml = XmlParser.GetXML(this.xmlFilePath);
            if (!string.IsNullOrEmpty(xml))
            {
                using (var bal = new XMLBillingBal())
                {
                    string result = bal.XMLBillFileParser(
                        xml,
                        this.xmlFilePath,
                        true,
                        this.corporateId,
                        this.facilityId,
                        string.Empty); // ..... Parsing of the XMl Data

                    // get the parsed File for the CID and FID
                    var objBal = new TPXMLParsedDataBal();
                    var fileInserted = objBal.TPXMLFilesListCIDFID(this.corporateId, this.facilityId);
                    Assert.IsTrue(fileInserted.Any());

                    // get the parsed Data for the CID and FID
                    var xmlparsedData = objBal.TPXMLParsedDataListCIDFID(this.corporateId, this.facilityId);
                    Assert.IsTrue(xmlparsedData.Any());

                    foreach (var item in xmlparsedData.GroupBy(x => x.OMBillID).Select(x => x.FirstOrDefault()).ToList())
                    {
                        Assert.IsNotNull(item.OMBillID);

                        // ...BillHeader Check
                        var getclaimBill = _blService.GetBillHeaderById(Convert.ToInt32(item.OMBillID));
                        Assert.IsNotNull(getclaimBill.BillHeaderID);
                        Assert.AreEqual(item.OMBillID, getclaimBill.BillHeaderID);


                        // ... BillActivity Checks

                        var getBillAct = _baService.GetBillActivitiesByBillHeaderId(Convert.ToInt32(item.OMBillID));
                        Assert.IsTrue(getBillAct.Any());
                        Assert.AreEqual(getBillAct.PickRandom().BillHeaderID, Convert.ToInt32(item.OMBillID));


                        // ... OpenOrder Checks
                        Assert.IsNotNull(item.OMEncounterID);
                        using (var openOrderbal = new OpenOrderBal())
                        {
                            var getOpenOrders = openOrderbal.GetOrdersByEncounterid(Convert.ToInt32(item.OMEncounterID));
                            Assert.IsTrue(getOpenOrders.Any());
                            Assert.AreEqual(getOpenOrders.PickRandom().EncounterID, Convert.ToInt32(item.OMEncounterID));
                        }

                        // ... OpenOrderActivity Checks
                        using (var openOrderactbal = new OrderActivityBal())
                        {
                            var getOpenOrdersAct = openOrderactbal.GetOrderActivitiesByEncounterId(Convert.ToInt32(item.OMEncounterID));
                            Assert.IsTrue(getOpenOrdersAct.Any());
                            Assert.AreEqual(getOpenOrdersAct.PickRandom().EncounterID, Convert.ToInt32(item.OMEncounterID));
                        }

                        // ... Scrub Header Checks
                        // using (var scrubHeaderbal = new ScrubReportBal())
                        // {
                        // var getOpenOrdersAct = scrubHeaderbal.GetScrubHeaderByEncounter(Convert.ToInt32(item.OMEncounterID));
                        // Assert.IsTrue(getOpenOrdersAct.Any());
                        // Assert.AreEqual(getOpenOrdersAct.PickRandom().EncounterID, Convert.ToInt32(item.OMEncounterID));
                        // Assert.IsTrue(getOpenOrdersAct.PickRandom().Passed > 0);
                        // }

                    }

                    // Delete the Data from all tables involved
                    var isdeleted = objBal.DeleteXMLParsedData(this.corporateId, this.facilityId);
                    Assert.IsTrue(isdeleted);
                }
            }
        }

        /// <summary>
        /// The my test cleanup.
        /// </summary>
        [TestCleanup]
        public void MyTestCleanup()
        {
            using (var objBal = new TPXMLParsedDataBal())
            {
                var isdeleted = objBal.DeleteXMLParsedData(this.corporateId, this.facilityId);
            }
        }

        /// <summary>
        /// The my test initialize.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
        }
        #endregion
    }
}