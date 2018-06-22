$(function () {
    var hfConsult = $("#hfConsult").val();
    $("#ddlConsult").val(hfConsult);

    BindGlobalCodesWithValue("#ddNurseAssessmentForm", 4950, "");
    $("#ddNurseAssessmentForm option[value='102']").remove();
    $("#ddNurseAssessmentForm option[value='103']").remove();
    $("#ddNurseAssessmentForm option[value='104']").remove();
});

function SaveEMForm(status, id) {

    var imgpath = $("#imagesource").attr('src');
    if (imgpath == "" || imgpath == null) {
        ShowMessage("Form must be signed first before it gets saved!", "Warning", "warning", true);
        return false;
    }
    var setId = id;
    var patientId = $("#GlobalPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    /*Global code category values start*/
    var EMBasicInfoGCCValue = 4629;
    var ChiefComplaintGCCValue = 4610;
    var HistoryPresentIllnessGCCValue = 4613;
    var ReviewSystemsGCCValue = 4615;
    var OrganSystemsGCCValue = 4616;
    var BodyAreasGCCValue = 4617;
    var AmountReviewedGCCValue = 4624;
    var DiagnosisGCCValueGCCValue = 4625;
    var RiskComplicationsGCCValue = 4626;
    var PatientCounseledReGCCValue = 4627;
    var ElectronicSignatureGCCValue = 4628;

    /*Global code category values end*/
    /*Basic Info tab code start*/
    var basicInfoArray = new Array();
    var chkNewPatient = $("#chkNewPatient")[0].checked == true ? "1" : "0";
    var chkEstPatient = $("#chkEstPatient")[0].checked == true ? "1" : "0";
    var chkPatientNonVerbal = $("#chkPatientNonVerbal")[0].checked == true ? "1" : "0";
    var chkFamily = $("#chkFamily")[0].checked == true ? "1" : "0";
    var chkMedPersonnel = $("#chkMedPersonnel")[0].checked == true ? "1" : "0";


    basicInfoArray.push(chkNewPatient + "-1-" + EMBasicInfoGCCValue + "-0");//controlvalue-globalcodevalue-globalcodecategoryvalue-parentglobalcodevalue
    basicInfoArray.push(chkEstPatient + "-2-" + EMBasicInfoGCCValue + "-0");
    basicInfoArray.push(chkPatientNonVerbal + "-3-" + EMBasicInfoGCCValue + "-0");
    basicInfoArray.push(chkFamily + "-4-" + EMBasicInfoGCCValue + "-0");
    basicInfoArray.push(chkMedPersonnel + "-5-" + EMBasicInfoGCCValue + "-0");
    /*Basic Info tab code end*/

    /*Complaint/Reason for Consult start*/
    var ddlConsult = $("#ddlConsult").val();
    var txtConsultOthers = $("#txtConsultOthers").val();
    basicInfoArray.push(ddlConsult + "-1-" + ChiefComplaintGCCValue + "-0");
    basicInfoArray.push(txtConsultOthers + "-2-" + ChiefComplaintGCCValue + "-0");
    /*Complaint/Reason for Consult end*/

    /*History of Present Illness start*/
    var chkPTNewProblem = $("#chkPTNewProblem")[0].checked == true ? "1" : "0";
    var chkPTExitingProblem = $("#chkPTExitingProblem")[0].checked == true ? "1" : "0";
    var txtLocation = $("#txtLocation").val();
    var txtQuality = $("#txtQuality").val();
    var txtSeverity = $("#txtSeverity").val();
    var txtDuration = $("#txtDuration").val();
    var txtTiming = $("#txtTiming").val();
    var txtContext = $("#txtContext").val();
    var txtModifyingFactors = $("#txtModifyingFactors").val();
    var txtOtherSignSymptoms = $("#txtOtherSignSymptoms").val();
    basicInfoArray.push(chkPTNewProblem + "-1-" + HistoryPresentIllnessGCCValue + "-0");
    basicInfoArray.push(chkPTExitingProblem + "-2-" + HistoryPresentIllnessGCCValue + "-0");
    basicInfoArray.push(txtLocation + "-3-" + HistoryPresentIllnessGCCValue + "-0");
    basicInfoArray.push(txtQuality + "-4-" + HistoryPresentIllnessGCCValue + "-0");
    basicInfoArray.push(txtSeverity + "-5-" + HistoryPresentIllnessGCCValue + "-0");
    basicInfoArray.push(txtDuration + "-6-" + HistoryPresentIllnessGCCValue + "-0");
    basicInfoArray.push(txtTiming + "-7-" + HistoryPresentIllnessGCCValue + "-0");
    basicInfoArray.push(txtContext + "-8-" + HistoryPresentIllnessGCCValue + "-0");
    basicInfoArray.push(txtModifyingFactors + "-9-" + HistoryPresentIllnessGCCValue + "-0");
    basicInfoArray.push(txtOtherSignSymptoms + "-10-" + HistoryPresentIllnessGCCValue + "-0");
    /*History of Present Illness end*/

    /*Review of Systems start*/

    //Constitutional start
    var chkConsNegative = $("#chkConsNegative")[0].checked == true ? "1" : "0";
    var chkConsWtLoss = $("#chkConsWtLoss")[0].checked == true ? "1" : "0";
    var chkConsFever = $("#chkConsFever")[0].checked == true ? "1" : "0";
    var chkConsFatigue = $("#chkConsFatigue")[0].checked == true ? "1" : "0";
    var chkConsOther = $("#chkConsOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkConsNegative + "-15-" + ReviewSystemsGCCValue + "-1");
    basicInfoArray.push(chkConsWtLoss + "-16-" + ReviewSystemsGCCValue + "-1");
    basicInfoArray.push(chkConsFever + "-17-" + ReviewSystemsGCCValue + "-1");
    basicInfoArray.push(chkConsFatigue + "-18-" + ReviewSystemsGCCValue + "-1");
    basicInfoArray.push(chkConsOther + "-19-" + ReviewSystemsGCCValue + "-1");
    //Constitutional end

    //Eyes start
    var chkEyesNegative = $("#chkEyesNegative")[0].checked == true ? "1" : "0";
    var chkEyesVisionChange = $("#chkEyesVisionChange")[0].checked == true ? "1" : "0";
    var chkEyesGlassesContacts = $("#chkEyesGlassesContacts")[0].checked == true ? "1" : "0";
    var chkEyesVisibleIrritation = $("#chkEyesVisibleIrritation")[0].checked == true ? "1" : "0";
    var chkEyesOther = $("#chkEyesOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkEyesNegative + "-20-" + ReviewSystemsGCCValue + "-2");
    basicInfoArray.push(chkEyesVisionChange + "-21-" + ReviewSystemsGCCValue + "-2");
    basicInfoArray.push(chkEyesGlassesContacts + "-22-" + ReviewSystemsGCCValue + "-2");
    basicInfoArray.push(chkEyesVisibleIrritation + "-23-" + ReviewSystemsGCCValue + "-2");
    basicInfoArray.push(chkEyesOther + "-24-" + ReviewSystemsGCCValue + "-2");
    //Eyes end

    //ENT/Mouth start
    var chkENTNegative = $("#chkENTNegative")[0].checked == true ? "1" : "0";
    var chkENTUlcers = $("#chkENTUlcers")[0].checked == true ? "1" : "0";
    var chkENTSinusitis = $("#chkENTSinusitis")[0].checked == true ? "1" : "0";
    var chkENTTinnitus = $("#chkENTTinnitus")[0].checked == true ? "1" : "0";
    var chkENTHeadache = $("#chkENTHeadache")[0].checked == true ? "1" : "0";
    var chkENTOther = $("#chkENTOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkENTNegative + "-25-" + ReviewSystemsGCCValue + "-3");
    basicInfoArray.push(chkENTUlcers + "-26-" + ReviewSystemsGCCValue + "-3");
    basicInfoArray.push(chkENTSinusitis + "-27-" + ReviewSystemsGCCValue + "-3");
    basicInfoArray.push(chkENTTinnitus + "-28-" + ReviewSystemsGCCValue + "-3");
    basicInfoArray.push(chkENTHeadache + "-29-" + ReviewSystemsGCCValue + "-3");
    basicInfoArray.push(chkENTOther + "-30-" + ReviewSystemsGCCValue + "-3");
    //ENT/Mouth end

    //Cardiovascular start
    var chkCardioNegative = $("#chkCardioNegative")[0].checked == true ? "1" : "0";
    var chkCardioOrthopneae = $("#chkCardioOrthopneae")[0].checked == true ? "1" : "0";
    var chkCardioChestPain = $("#chkCardioChestPain")[0].checked == true ? "1" : "0";
    var chkCardioDoe = $("#chkCardioDoe")[0].checked == true ? "1" : "0";
    var chkCardioEdema = $("#chkCardioEdema")[0].checked == true ? "1" : "0";
    var chkCardioPalpitation = $("#chkCardioPalpitation")[0].checked == true ? "1" : "0";
    var chkCardioOther = $("#chkCardioOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkCardioNegative + "-31-" + ReviewSystemsGCCValue + "-4");
    basicInfoArray.push(chkCardioOrthopneae + "-32-" + ReviewSystemsGCCValue + "-4");
    basicInfoArray.push(chkCardioChestPain + "-33-" + ReviewSystemsGCCValue + "-4");
    basicInfoArray.push(chkCardioDoe + "-34-" + ReviewSystemsGCCValue + "-4");
    basicInfoArray.push(chkCardioEdema + "-35-" + ReviewSystemsGCCValue + "-4");
    basicInfoArray.push(chkCardioPalpitation + "-36-" + ReviewSystemsGCCValue + "-4");
    basicInfoArray.push(chkCardioOther + "-37-" + ReviewSystemsGCCValue + "-4");
    //Cardiovascular end

    //Respiratory start
    var chkRespNegative = $("#chkRespNegative")[0].checked == true ? "1" : "0";
    var chkRespWheezing = $("#chkRespWheezing")[0].checked == true ? "1" : "0";
    var chkRespHemoptysis = $("#chkRespHemoptysis")[0].checked == true ? "1" : "0";
    var chkRespSOB = $("#chkRespSOB")[0].checked == true ? "1" : "0";
    var chkRespCough = $("#chkRespCough")[0].checked == true ? "1" : "0";
    var chkRespOther = $("#chkRespOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkRespNegative + "-38-" + ReviewSystemsGCCValue + "-5");
    basicInfoArray.push(chkRespWheezing + "-39-" + ReviewSystemsGCCValue + "-5");
    basicInfoArray.push(chkRespHemoptysis + "-40-" + ReviewSystemsGCCValue + "-5");
    basicInfoArray.push(chkRespSOB + "-41-" + ReviewSystemsGCCValue + "-5");
    basicInfoArray.push(chkRespCough + "-42-" + ReviewSystemsGCCValue + "-5");
    basicInfoArray.push(chkRespOther + "-43-" + ReviewSystemsGCCValue + "-5");
    //Respiratory end

    //Gastrointestinal start
    var chkGasNegative = $("#chkGasNegative")[0].checked == true ? "1" : "0";
    var chkGasDiarrhea = $("#chkGasDiarrhea")[0].checked == true ? "1" : "0";
    var chkGasBldyStool = $("#chkGasBldyStool")[0].checked == true ? "1" : "0";
    var chkGasNV = $("#chkGasNV")[0].checked == true ? "1" : "0";
    var chkGasConstipation = $("#chkGasConstipation")[0].checked == true ? "1" : "0";
    var chkGasFlatulence = $("#chkGasFlatulence")[0].checked == true ? "1" : "0";
    var chkGasPain = $("#chkGasPain")[0].checked == true ? "1" : "0";
    var chkGasOther = $("#chkGasOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkGasNegative + "-44-" + ReviewSystemsGCCValue + "-6");
    basicInfoArray.push(chkGasDiarrhea + "-45-" + ReviewSystemsGCCValue + "-6");
    basicInfoArray.push(chkGasBldyStool + "-46-" + ReviewSystemsGCCValue + "-6");
    basicInfoArray.push(chkGasNV + "-47-" + ReviewSystemsGCCValue + "-6");
    basicInfoArray.push(chkGasConstipation + "-48-" + ReviewSystemsGCCValue + "-6");
    basicInfoArray.push(chkGasFlatulence + "-49-" + ReviewSystemsGCCValue + "-6");
    basicInfoArray.push(chkGasPain + "-50-" + ReviewSystemsGCCValue + "-6");
    basicInfoArray.push(chkGasOther + "-51-" + ReviewSystemsGCCValue + "-6");
    //Gastrointestinal end

    //Genitourinary start
    var chkGenNegative = $("#chkGenNegative")[0].checked == true ? "1" : "0";
    var chkGenHematuria = $("#chkGenHematuria")[0].checked == true ? "1" : "0";
    var chkGenDysuria = $("#chkGenDysuria")[0].checked == true ? "1" : "0";
    var chkGenUrgncy = $("#chkGenUrgncy")[0].checked == true ? "1" : "0";
    var chkGenFrqncy = $("#chkGenFrqncy")[0].checked == true ? "1" : "0";
    var chkGenEmptying = $("#chkGenEmptying")[0].checked == true ? "1" : "0";
    var chkGenIncontinent = $("#chkGenIncontinent")[0].checked == true ? "1" : "0";
    var chkGenAbnormalBleeding = $("#chkGenAbnormalBleeding")[0].checked == true ? "1" : "0";
    var chkGenDyspareunia = $("#chkGenDyspareunia")[0].checked == true ? "1" : "0";
    var chkGenOthera = $("#chkGenOthera")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkGenNegative + "-52-" + ReviewSystemsGCCValue + "-7");
    basicInfoArray.push(chkGenHematuria + "-53-" + ReviewSystemsGCCValue + "-7");
    basicInfoArray.push(chkGenDysuria + "-54-" + ReviewSystemsGCCValue + "-7");
    basicInfoArray.push(chkGenUrgncy + "-55-" + ReviewSystemsGCCValue + "-7");
    basicInfoArray.push(chkGenFrqncy + "-56-" + ReviewSystemsGCCValue + "-7");
    basicInfoArray.push(chkGenEmptying + "-57-" + ReviewSystemsGCCValue + "-7");
    basicInfoArray.push(chkGenIncontinent + "-58-" + ReviewSystemsGCCValue + "-7");
    basicInfoArray.push(chkGenAbnormalBleeding + "-59-" + ReviewSystemsGCCValue + "-7");
    basicInfoArray.push(chkGenDyspareunia + "-61-" + ReviewSystemsGCCValue + "-7");
    basicInfoArray.push(chkGenOthera + "-62-" + ReviewSystemsGCCValue + "-7");
    //Genitourinary end

    //Musculoskeletal start
    var chkMusculNegative = $("#chkMusculNegative")[0].checked == true ? "1" : "0";
    var chkMusculWeakness = $("#chkMusculWeakness")[0].checked == true ? "1" : "0";
    var chkMusculPain = $("#chkMusculPain")[0].checked == true ? "1" : "0";
    var chkMusculVisibleKnot = $("#chkMusculVisibleKnot")[0].checked == true ? "1" : "0";
    var chkMusculOther = $("#chkMusculOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkMusculNegative + "-63-" + ReviewSystemsGCCValue + "-8");
    basicInfoArray.push(chkMusculWeakness + "-64-" + ReviewSystemsGCCValue + "-8");
    basicInfoArray.push(chkMusculPain + "-65-" + ReviewSystemsGCCValue + "-8");
    basicInfoArray.push(chkMusculVisibleKnot + "-66-" + ReviewSystemsGCCValue + "-8");
    basicInfoArray.push(chkMusculOther + "-67-" + ReviewSystemsGCCValue + "-8");
    //Musculoskeletal end

    //Skin/breast start
    var chkSkinNegative = $("#chkSkinNegative")[0].checked == true ? "1" : "0";
    var chkSkinMastalgia = $("#chkSkinMastalgia")[0].checked == true ? "1" : "0";
    var chkSkinDicharge = $("#chkSkinDicharge")[0].checked == true ? "1" : "0";
    var chkSkinMasses = $("#chkSkinMasses")[0].checked == true ? "1" : "0";
    var chkSkinRash = $("#chkSkinRash")[0].checked == true ? "1" : "0";
    var chkSkinUlcer = $("#chkSkinUlcer")[0].checked == true ? "1" : "0";
    var chkSkinOther = $("#chkSkinOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkSkinNegative + "-68-" + ReviewSystemsGCCValue + "-9");
    basicInfoArray.push(chkSkinMastalgia + "-69-" + ReviewSystemsGCCValue + "-9");
    basicInfoArray.push(chkSkinDicharge + "-70-" + ReviewSystemsGCCValue + "-9");
    basicInfoArray.push(chkSkinMasses + "-71-" + ReviewSystemsGCCValue + "-9");
    basicInfoArray.push(chkSkinRash + "-72-" + ReviewSystemsGCCValue + "-9");
    basicInfoArray.push(chkSkinUlcer + "-73-" + ReviewSystemsGCCValue + "-9");
    basicInfoArray.push(chkSkinOther + "-74-" + ReviewSystemsGCCValue + "-9");
    //Skin/breast end

    //Neurological start
    var chkNeuroNegative = $("#chkNeuroNegative")[0].checked == true ? "1" : "0";
    var chkNeuroSyncope = $("#chkNeuroSyncope")[0].checked == true ? "1" : "0";
    var chkNeuroSeizures = $("#chkNeuroSeizures")[0].checked == true ? "1" : "0";
    var chkNeuroNumbness = $("#chkNeuroNumbness")[0].checked == true ? "1" : "0";
    var chkNeuroWalking = $("#chkNeuroWalking")[0].checked == true ? "1" : "0";
    var chkNeuroOther = $("#chkNeuroOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkNeuroNegative + "-75-" + ReviewSystemsGCCValue + "-10");
    basicInfoArray.push(chkNeuroSyncope + "-76-" + ReviewSystemsGCCValue + "-10");
    basicInfoArray.push(chkNeuroSeizures + "-77-" + ReviewSystemsGCCValue + "-10");
    basicInfoArray.push(chkNeuroNumbness + "-78-" + ReviewSystemsGCCValue + "-10");
    basicInfoArray.push(chkNeuroWalking + "-79-" + ReviewSystemsGCCValue + "-10");
    basicInfoArray.push(chkNeuroOther + "-80-" + ReviewSystemsGCCValue + "-10");
    //Neurological end

    //Psychiatric start
    var chkPsyNegative = $("#chkPsyNegative")[0].checked == true ? "1" : "0";
    var chkPsyDepression = $("#chkPsyDepression")[0].checked == true ? "1" : "0";
    var chkPsyCrying = $("#chkPsyCrying")[0].checked == true ? "1" : "0";
    var chkPsyOther = $("#chkPsyOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkPsyNegative + "-81-" + ReviewSystemsGCCValue + "-11");
    basicInfoArray.push(chkPsyDepression + "-82-" + ReviewSystemsGCCValue + "-11");
    basicInfoArray.push(chkPsyCrying + "-83-" + ReviewSystemsGCCValue + "-11");
    basicInfoArray.push(chkPsyOther + "-84-" + ReviewSystemsGCCValue + "-11");
    //Psychiatric end

    //Endocrine start
    var chkEndoNegative = $("#chkEndoNegative")[0].checked == true ? "1" : "0";
    var chkEndoDiabetes = $("#chkEndoDiabetes")[0].checked == true ? "1" : "0";
    var chkEndoHypothyroid = $("#chkEndoHypothyroid")[0].checked == true ? "1" : "0";
    var chkEndoHperthyroid = $("#chkEndoHperthyroid")[0].checked == true ? "1" : "0";
    var chkEndoHotflashes = $("#chkEndoHotflashes")[0].checked == true ? "1" : "0";
    var chkEndoOther = $("#chkEndoOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkEndoNegative + "-85-" + ReviewSystemsGCCValue + "-12");
    basicInfoArray.push(chkEndoDiabetes + "-86-" + ReviewSystemsGCCValue + "-12");
    basicInfoArray.push(chkEndoHypothyroid + "-87-" + ReviewSystemsGCCValue + "-12");
    basicInfoArray.push(chkEndoHperthyroid + "-88-" + ReviewSystemsGCCValue + "-12");
    basicInfoArray.push(chkEndoHotflashes + "-89-" + ReviewSystemsGCCValue + "-12");
    basicInfoArray.push(chkEndoOther + "-90-" + ReviewSystemsGCCValue + "-12");
    //Endocrine end

    //Hemat/Lymph start
    var chkHematNegative = $("#chkHematNegative")[0].checked == true ? "1" : "0";
    var chkHematBruises = $("#chkHematBruises")[0].checked == true ? "1" : "0";
    var chkHematBleeding = $("#chkHematBleeding")[0].checked == true ? "1" : "0";
    var chkHematAdenopathy = $("#chkHematAdenopathy")[0].checked == true ? "1" : "0";
    var chkHematOther = $("#chkHematOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkHematNegative + "-91-" + ReviewSystemsGCCValue + "-13");
    basicInfoArray.push(chkHematBruises + "-92-" + ReviewSystemsGCCValue + "-13");
    basicInfoArray.push(chkHematBleeding + "-93-" + ReviewSystemsGCCValue + "-13");
    basicInfoArray.push(chkHematAdenopathy + "-94-" + ReviewSystemsGCCValue + "-13");
    basicInfoArray.push(chkHematOther + "-95-" + ReviewSystemsGCCValue + "-13");
    //Hemat/Lymph end

    //Allergic/Immuno start
    var chkAllergicNegative = $("#chkAllergicNegative")[0].checked == true ? "1" : "0";
    var chkAllergicOther = $("#chkAllergicOther")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkAllergicNegative + "-96-" + ReviewSystemsGCCValue + "-14");
    basicInfoArray.push(chkAllergicOther + "-97-" + ReviewSystemsGCCValue + "-14");
    //Allergic/Immuno end

    //Organ Systems start
    var txtOrganEyes = $("#txtOrganEyes").val();
    var txtOrganEars = $("#txtOrganEars").val();
    var txtOrganCardio = $("#txtOrganCardio").val();
    var txtOrganResp = $("#txtOrganResp").val();
    var txtOrganGas = $("#txtOrganGas").val();
    var txtOrganSkin = $("#txtOrganSkin").val();
    var txtOrganMuscul = $("#txtOrganMuscul").val();
    var txtOrganNeurological = $("#txtOrganNeurological").val();
    var txtOrganLymphatic = $("#txtOrganLymphatic").val();
    var txtOrganPsychiatric = $("#txtOrganPsychiatric").val();
    basicInfoArray.push(txtOrganEyes + "-1-" + OrganSystemsGCCValue + "-0");
    basicInfoArray.push(txtOrganEars + "-2-" + OrganSystemsGCCValue + "-0");
    basicInfoArray.push(txtOrganCardio + "-3-" + OrganSystemsGCCValue + "-0");
    basicInfoArray.push(txtOrganResp + "-4-" + OrganSystemsGCCValue + "-0");
    basicInfoArray.push(txtOrganGas + "-5-" + OrganSystemsGCCValue + "-0");
    basicInfoArray.push(txtOrganSkin + "-6-" + OrganSystemsGCCValue + "-0");
    basicInfoArray.push(txtOrganMuscul + "-7-" + OrganSystemsGCCValue + "-0");
    basicInfoArray.push(txtOrganNeurological + "-8-" + OrganSystemsGCCValue + "-0");
    basicInfoArray.push(txtOrganLymphatic + "-9-" + OrganSystemsGCCValue + "-0");
    basicInfoArray.push(txtOrganPsychiatric + "-10-" + OrganSystemsGCCValue + "-0");
    //Organ Systems end

    //BODY AREAS start
    var txtHeadFace = $("#txtHeadFace").val();
    var txtNeck = $("#txtNeck").val();
    var txtChest = $("#txtChest").val();
    var txtAbdomen = $("#txtAbdomen").val();
    var txtGenitalia = $("#txtGenitalia").val();
    var txtBackSpine = $("#txtBackSpine").val();
    basicInfoArray.push(txtHeadFace + "-1-" + BodyAreasGCCValue + "-0");
    basicInfoArray.push(txtNeck + "-2-" + BodyAreasGCCValue + "-0");
    basicInfoArray.push(txtChest + "-3-" + BodyAreasGCCValue + "-0");
    basicInfoArray.push(txtAbdomen + "-4-" + BodyAreasGCCValue + "-0");
    basicInfoArray.push(txtGenitalia + "-5-" + BodyAreasGCCValue + "-0");
    basicInfoArray.push(txtBackSpine + "-6-" + BodyAreasGCCValue + "-0");
    //BODY AREAS end

    //AMOUNT AND COMPLEXITY OF DATA REVIEWED start
    var txtAmtComplexLaboratory = $("#txtAmtComplexLaboratory").val();
    var txtAmtComplexRadiology = $("#txtAmtComplexRadiology").val();
    var txtAmtComplexPreviousTest = $("#txtAmtComplexPreviousTest").val();
    var txtAmtComplexDiscussionTest = $("#txtAmtComplexDiscussionTest").val();
    var txtAmtComplexOldrecords = $("#txtAmtComplexOldrecords").val();
    var txtAmtComplexHistoryObtained = $("#txtAmtComplexHistoryObtained").val();
    basicInfoArray.push(txtAmtComplexLaboratory + "-1-" + AmountReviewedGCCValue + "-0");
    basicInfoArray.push(txtAmtComplexRadiology + "-2-" + AmountReviewedGCCValue + "-0");
    basicInfoArray.push(txtAmtComplexPreviousTest + "-3-" + AmountReviewedGCCValue + "-0");
    basicInfoArray.push(txtAmtComplexDiscussionTest + "-4-" + AmountReviewedGCCValue + "-0");
    basicInfoArray.push(txtAmtComplexOldrecords + "-5-" + AmountReviewedGCCValue + "-0");
    basicInfoArray.push(txtAmtComplexHistoryObtained + "-6-" + AmountReviewedGCCValue + "-0");
    //AMOUNT AND COMPLEXITY OF DATA REVIEWED end

    //DIAGNOSIS/MANAGEMENT OPTIONS start
    var chkDIAGNOSISEstProb = $("#chkDIAGNOSISEstProb")[0].checked == true ? "1" : "0";
    var chkDIAGNOSISNewProb = $("#chkDIAGNOSISNewProb")[0].checked == true ? "1" : "0";
    var chkDIAGNOSISMinProb = $("#chkDIAGNOSISMinProb")[0].checked == true ? "1" : "0";
    var chkDIAGNOSISLimitedProb = $("#chkDIAGNOSISLimitedProb")[0].checked == true ? "1" : "0";
    var chkDIAGNOSISExtNewProb = $("#chkDIAGNOSISExtNewProb")[0].checked == true ? "1" : "0";
    var chkDIAGNOSISMultipleProb = $("#chkDIAGNOSISMultipleProb")[0].checked == true ? "1" : "0";
    var txtAssessmentPlan = $("#txtAssessmentPlan").val();
    basicInfoArray.push(chkDIAGNOSISEstProb + "-1-" + DiagnosisGCCValueGCCValue + "-0");
    basicInfoArray.push(chkDIAGNOSISNewProb + "-2-" + DiagnosisGCCValueGCCValue + "-0");
    basicInfoArray.push(chkDIAGNOSISMinProb + "-3-" + DiagnosisGCCValueGCCValue + "-0");
    basicInfoArray.push(chkDIAGNOSISLimitedProb + "-4-" + DiagnosisGCCValueGCCValue + "-0");
    basicInfoArray.push(chkDIAGNOSISExtNewProb + "-6-" + DiagnosisGCCValueGCCValue + "-0");
    basicInfoArray.push(chkDIAGNOSISMultipleProb + "-5-" + DiagnosisGCCValueGCCValue + "-0");
    basicInfoArray.push(txtAssessmentPlan + "-7-" + DiagnosisGCCValueGCCValue + "-0");
    //DIAGNOSIS/MANAGEMENT OPTIONS end

    //RISK OF COMPLICATIONS start
    var chkComplicationMin = $("#chkComplicationMin")[0].checked == true ? "1" : "0";
    var chkComplicationLow = $("#chkComplicationLow")[0].checked == true ? "1" : "0";
    var chkComplicationModerate = $("#chkComplicationModerate")[0].checked == true ? "1" : "0";
    var chkComplicationHigh = $("#chkComplicationHigh")[0].checked == true ? "1" : "0";
    basicInfoArray.push(chkComplicationMin + "-1-" + RiskComplicationsGCCValue + "-0");
    basicInfoArray.push(chkComplicationLow + "-2-" + RiskComplicationsGCCValue + "-0");
    basicInfoArray.push(chkComplicationModerate + "-3-" + RiskComplicationsGCCValue + "-0");
    basicInfoArray.push(chkComplicationHigh + "-4-" + RiskComplicationsGCCValue + "-0");
    //RISK OF COMPLICATIONS end

    //PATIENT COUNSELED RE start
    var txtPatientCounseledRe = $("#txtPatientCounseledRe").val();
    basicInfoArray.push(txtPatientCounseledRe + "-1-" + PatientCounseledReGCCValue + "-0");
    //PATIENT COUNSELED RE end


    //Electronic Signature start
    var txtElectronicSignature = $("#txtElectronicSignature").length > 0 ? $("#txtElectronicSignature").val() : "";
    basicInfoArray.push(txtElectronicSignature + "-1-" + ElectronicSignatureGCCValue + "-0");
    //Electronic Signature end

    /*Review of Systems end*/
    var jsonData = JSON.stringify({
        basicInfo: basicInfoArray,
        patientId: patientId,
        encounterId: encounterId,
        estatus: status,
        rowExists: $("#hfRowExists").val(),
        setId: setId,
        imagePath: $("#imagePath").val()
    });
    $.ajax({
        type: "POST",
        url: '/Evaluation/SaveEvaluationManagement',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data > 0) {
                BindEvaluationManagement();
                ShowMessage("E&M Form submitted successfully", "Success", "success", true);
            } else if (data == "-1") {
                ShowMessage("Encounter is not started yet", "Oops!!", "warning", true);
            }
            $('#divEvaluation').hide();
            $.validationEngine.closePrompt('.formError', true);
        },
        error: function (msg) {
            ShowMessage("Failed to submit", "Error", "error", true);
        }
    });
}



function ViewENMFrom(id) {
    var setId = id;
    var patientId = $("#hdPatientId").val();
    var jsonData = JSON.stringify({
        pId: patientId,
        setId: setId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Evaluation/EvaluationData",
        data: jsonData,
        dataType: "html",
        success: function (data) {
            BindList("#EvaluationDiv", data);
            //$("#imagesource").attr('src', data.imageSource);
            InitializeDateTimePicker();
            BindSignatureData(id);

            $('#divEvaluation').show();
        },
        error: function (msg) {

        }
    });
}

function BindEvaluationManagement() {

    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        ecounterId: encounterId,
        patinetId: patientId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetPreENcounterList",
        data: jsonData,
        dataType: "html",
        success: function (data) {
            BindList("#PreEvaluationList", data);
            InitializeDateTimePicker();
        },
        error: function (msg) {

        }
    });
}

function BindSignatureData(id) {
    var setId = id;
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        ecounterId: encounterId,
        patinetId: patientId,
        setId: setId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Evaluation/GetSignatureData",
        data: jsonData,
        dataType: "json",
        success: function (data) {
            $("#imagesource").attr('src', data);
        },
        error: function (msg) {

        }
    });
}


