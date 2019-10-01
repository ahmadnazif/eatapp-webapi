using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Services
{
    public class TimezoneHelper : ITimezoneHelper
    {
        public string ServerTimezoneId { get; } = TimeZoneInfo.Local.Id;
        private string ClientTimezoneId { get; } = "Singapore Standard Time"; // later change with database value

        public TimezoneHelper()
        {

        }

        public string GetClientTimezoneId() => this.ClientTimezoneId;

        public string GetServerTimezoneOffset()
        {
            var offset = TimeZoneInfo.GetSystemTimeZones().Where(d => d.Id == this.ServerTimezoneId).Select(d => d.BaseUtcOffset).SingleOrDefault();
            var offsetStr = offset.TotalHours > 0 ? "+" + offset.TotalHours : offset.TotalHours.ToString();
            //return offsetStr + " GMT";
            return "UTC" + offsetStr;
        }

        public string GetClientTimezoneOffset()
        {
            var offset = TimeZoneInfo.GetSystemTimeZones().Where(d => d.Id == this.ClientTimezoneId).Select(d => d.BaseUtcOffset).SingleOrDefault();
            var offsetStr = offset.TotalHours > 0 ? "+" + offset.TotalHours : offset.TotalHours.ToString();
            //return offsetStr + " GMT";
            return "UTC" + offsetStr;
        }

        public List<string> ListAllTimezoneId(bool sortById = true)
        {
            var ids = TimeZoneInfo.GetSystemTimeZones();
            if (sortById)
            {
                return ids.OrderBy(d => d.Id).Select(d => d.Id).ToList();
            }
            else
            {
                return ids.Select(d => d.Id).ToList();
            }
        }

        public IEnumerable<TimeZoneInfo> ListAllTimezone()
        {
            return TimeZoneInfo.GetSystemTimeZones();
        }

        public DateTime ConvertToClientTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, ServerTimezoneId, ClientTimezoneId);
        }

        public DateTime ConvertToServerTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, ClientTimezoneId, ServerTimezoneId);
        }

    }
}
