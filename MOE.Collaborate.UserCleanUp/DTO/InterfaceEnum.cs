using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOE.Collaborate.UserCleanUp.DTO
{
    public static class InterfaceEnum
    {

        #region IAMS
        public enum IAMSField
        {
            nric = 0,
            full_name = 1,
            account_id = 2,
            location_code = 3,
            moe_applications = 4,
            designation = 5,
            email = 6,
            division_name = 7,
            branch_name = 8,
            school_name = 9,
            last_changed_nric = 10,
            nric_date_changed = 11
        }
        #endregion
    }
}
