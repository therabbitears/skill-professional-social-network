using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Wrly.infrastuctures.Utils;
using Wrly.Infrastuctures.Utils;
using Wrly.Models;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class BaseProcessor
    {
        public string IpAddress
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
                return "-1.-1.-1.-1";
            }
        }

        public HttpRequest Request { get { return HttpContext.Current.Request; } }

        public UserHash UserHashObject { get { return SessionInfo.UserHash; } }

        public string User
        {
            get
            {
                if (Request.IsAuthenticated)
                {
                    return HttpContext.Current.User.Identity.Name;
                }
                return IpAddress;
            }
        }

        public string UserHash
        {
            get
            {
                if (SessionInfo.UserHash != null)
                {
                    var userHash = SessionInfo.UserHash;
                    var table = new Hashtable();
                    table.Add("PersonID", userHash.PersonID);
                    table.Add("EntityID", userHash.EntityID);
                    table.Add("UserID", userHash.UserID);
                    return QueryStringHelper.Encrypt(table);
                }
                return string.Empty;
            }
        }

        Hashtable _EntityHash;
        public Hashtable EntityHash
        {
            get
            {
                if (_EntityHash == null)
                {
                    _EntityHash = new Hashtable();
                    if (SessionInfo.UserHash != null)
                    {
                        var userHash = SessionInfo.UserHash;
                        _EntityHash.Add("PersonID", userHash.PersonID);
                        _EntityHash.Add("EntityID", userHash.EntityID);
                        _EntityHash.Add("UserID", userHash.UserID);
                        _EntityHash.Add("CreatedBy", User);
                        _EntityHash.Add("EditedBy", User);
                    }
                    _EntityHash.Add("CreatedOn", Now);
                    _EntityHash.Add("EditedOn", Now);
                }
                return _EntityHash;
            }
        }

        internal bool? GetPottentialCurrent(DateTime? endDate)
        {
            if (endDate == null)
                return true;
            return false;
        }

        internal DateTime? GetPottentialStartDateForFormData(int? year, int? month, int? day)
        {
            if (year == null || year < 0)
                return null;

            if (month == null)
                month = 1;
            if (day == null)
                day = DateTime.DaysInMonth((int)year, (int)month);

            return new DateTime((int)year, (int)month, (int)day);
        }

        internal DateTime? GetPottentialEndDateForFormData(int? year, int? month, int? day)
        {
            if (year == null || year < 0)
                return null;

            if (month == null)
                month = 12;
            if (day == null)
                day = 1;

            return new DateTime((int)year, (int)month, (int)day);
        }

        public DateTime Now { get { return DateTime.UtcNow; } }
        public long Tickes { get { return Now.Ticks; } }
    }
}
