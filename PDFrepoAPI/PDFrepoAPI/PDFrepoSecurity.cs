using PDFrepoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFrepoAPI
{
    public class PDFrepoSecurity
    {
        public static bool login(string username, string password)
        {
            using (PDFsystemEntities1 entities = new PDFsystemEntities1())
            {
                //if username and pass match return true otherwize will return false 
                return entities.users_TBL.Any(u => u.username.Equals(username, StringComparison.OrdinalIgnoreCase)
                  && (u.password == password));




            }

        }
    }
}


 