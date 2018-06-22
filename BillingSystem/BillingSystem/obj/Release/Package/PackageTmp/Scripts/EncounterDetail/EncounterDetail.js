function CalculateAge(birthday) {
    var date1 = birthday.value.split('/');
    /*
    Changes by Amit Jain as Date was not picked correctly, now working correctly
    On: 13102014
    */
    //var newdate = date1[1] + "/" + date1[0] + "/" + date1[2];
    var newdate = date1[0] + "/" + date1[1] + "/" + date1[2];
    if (birthday.value != '') {
        birthdayDate = new Date(newdate);
        dateNow = new Date();
        var years = dateNow.getFullYear() - birthdayDate.getFullYear();
        var months = dateNow.getMonth() - birthdayDate.getMonth();
        var days = dateNow.getDate() - birthdayDate.getDate();
        if (isNaN(years)) {
            return "";
        }
        else {
            if (months < 0 || (months == 0 && days < 0)) {
                years = parseInt(years) - 1;
                if (years < 0)
                    years = 0;
                return years;
            }
            else {
                return years;
            }
        }
    }
    return "";
}
