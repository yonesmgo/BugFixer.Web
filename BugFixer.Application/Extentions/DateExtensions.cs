using System.Globalization;

namespace BugFixer.Application.Extentions
{
    public static class DateExtensions
    {
        public static string ToShamsi(this DateTime d)
        {
            var persianCalander = new PersianCalendar();
            return $"{persianCalander.GetYear(d)}/{persianCalander.GetMonth(d).ToString("00")}/{persianCalander.GetDayOfMonth(d).ToString("00")}";
        }

        public static DateTime ToMiladi(this string date)
        {
            var spDate = date.Split('/');
            var year = Convert.ToInt32(spDate[0]);
            var month = Convert.ToInt32(spDate[1]);
            var day = Convert.ToInt32(spDate[2]);
            return new DateTime(year, month, day, new PersianCalendar());
        }

        public static string AsTimeAgo(this DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);

            return timeSpan.TotalSeconds switch
            {
                <= 60 => $"{timeSpan.Seconds} ثانیه پیش",

                _ => timeSpan.TotalMinutes switch
                {
                    <= 1 => "یک دقیقه پیش",
                    < 60 => $"حدود {timeSpan.Minutes} دقیقه پیش",
                    _ => timeSpan.TotalHours switch
                    {
                        <= 1 => "یک ساعت پیش",
                        < 24 => $"حدود {timeSpan.Hours} ساعت پیش",
                        _ => timeSpan.TotalDays switch
                        {
                            <= 1 => "دیروز",
                            <= 30 => $"حدود {timeSpan.Days} روز پیش",

                            <= 60 => "یک ماه قبل ",
                            < 365 => $"حدود {timeSpan.Days / 30} ماه پیش",

                            <= 365 * 2 => "حدود یکسال قبل",
                            _ => $"حدود {timeSpan.Days / 365} سال پیش"
                        }
                    }
                }
            };
        }
    }
}
