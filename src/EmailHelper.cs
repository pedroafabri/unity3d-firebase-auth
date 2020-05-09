using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailHelper
{
    /// <summary>
    /// Tests an e-mail and returns true if is valid or false if not.
    /// </summary>
    /// <param name="email">The e-mail to be tested.</param>
    /// <returns></returns>
    public static bool isEmailValid(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
