using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Functionality
    {
        static void Main(string[] args)
        {
            /********  Duration in Minutes  *********/
            int _Duration = 60;

            /********  Mention Start Date  *********/
            DateTime _StartDate = DateTime.Now;

            /********  Mention your End Date here  *********/
            DateTime _EndDate = DateTime.Now.AddDays(30);

            /********  Mention your Minimum Starting Hour  *********/
            int _StartHour = 9;

            /********  Mention your Minimum Ending Hour  *********/
            int _EndHour = 18;

            /********  Mention your Frequency of Dates It can be either  
             * 
                    Weekly
                    Bi-Weekly
                    Monthly
                    Quarterly
                    Yearly

             * *********/
            string _Frequency = "Monthly";

            List<string> _DaysofWeek = new List<string>()
            {
                "Monday" , "Tuesday", "Wednesday","Thursday","Friday"
            };
            List<DateModel> _dates = GetTimeSlots(_Duration, _StartDate, _EndDate, _StartHour, _EndHour, _Frequency, _DaysofWeek);

            foreach (var item in _dates)
            {
                Console.WriteLine(item.DayName);
                foreach (var _insideITem in item.ActualDates)
                {
                    Console.WriteLine(_insideITem.ActualDate.ToString("dd MMM, yyyy") + " - "
                        + _insideITem.StartTime.ToString() + " to " + _insideITem.EndTime.ToString());
                }
            }
            Console.WriteLine(_dates);
        }

        /// <summary>
        /// Method to Get Time Slots
        /// </summary>
        /// <param name="_Duration"></param>
        /// <param name="_StartDate"></param>
        /// <param name="_EndDate"></param>
        /// <param name="_StartHour"></param>
        /// <param name="_EndHour"></param>
        /// <param name="_Frequency"></param>
        /// <param name="_DaysofWeek"></param>
        /// <returns></returns>
        private static List<DateModel> GetTimeSlots(int _Duration, DateTime _StartDate, DateTime _EndDate, int _StartHour, int _EndHour, string _Frequency, List<string> _DaysofWeek)
        {
            List<DateTime> days_list = new List<DateTime>();
            List<DateTime> filtered_dayslist = new List<DateTime>();


            // Take All Days between Start and End Date
            for (DateTime date = _StartDate; date <= _EndDate; date = date.AddDays(1))
            {
                if (_DaysofWeek.Contains(date.DayOfWeek.ToString())) { days_list.Add(date); }
            }

            // Take All Days between Start and End Date

            Dictionary<string, List<DateTime>> _dict = days_list
                                                            .GroupBy(x => x.DayOfWeek.ToString())
                                                            .OrderBy(p => p.Key.ToString())
                                                            .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var dictItem in _dict)
            {
                List<int> _months = dictItem.Value.Select(a => a.Month).Distinct().ToList();
                List<int> _years = dictItem.Value.Select(a => a.Year).Distinct().ToList();
                List<int> _weekNumber = dictItem.Value.Select(a => a.DayOfYear / 7).Distinct().ToList();

                if (_Frequency == "Weekly")
                {
                    foreach (var item in _weekNumber)
                    {
                        filtered_dayslist.Add(dictItem.Value.Where(a => a.DayOfYear / 7 == item).FirstOrDefault());
                    }
                }
                if (_Frequency == "Bi-Weekly")
                {
                    for (int i = 0; i < _weekNumber.Count; i = i + 2)
                    {
                        filtered_dayslist.Add(dictItem.Value.Where(a => a.DayOfYear / 7 == _weekNumber[i]).FirstOrDefault());
                    }
                }
                if (_Frequency == "Monthly")
                {
                    foreach (var item in _months)
                    {
                        filtered_dayslist.Add(dictItem.Value.Where(a => a.Month == item).FirstOrDefault());
                    }
                }

                if (_Frequency == "Quarterly")
                {
                    for (int i = 0; i < _months.Count; i = i + 3)
                    {
                        filtered_dayslist.Add(dictItem.Value.Where(a => a.Month == _months[i]).FirstOrDefault());
                    }
                }

                if (_Frequency == "Yearly")
                {
                    foreach (var item in _years)
                    {
                        filtered_dayslist.Add(dictItem.Value.Where(a => a.Month == item).FirstOrDefault());
                    }
                }
            }


            filtered_dayslist = filtered_dayslist.OrderBy(a => a.Date).ToList();


            // Take Time for each days
            List<DateModel> _dates = new List<DateModel>();
            var _groupedList = filtered_dayslist.GroupBy(a => a.DayOfWeek).ToList();
            _groupedList.ForEach((_groupedItem) =>
            {
                DateModel _dateModel = new DateModel();
                _dateModel.ActualDates = new List<DateItemModel>();
                _dateModel.DayName = _groupedItem.Key.ToString();
                _groupedItem.ToList().ForEach((item) =>
                {
                    var _dateStartFrom = new DateTime(item.Year, item.Month, item.Day, _StartHour, 0, 0);
                    var _dateEndsOn = new DateTime(item.Year, item.Month, item.Day, _EndHour, 0, 0);
                    for (var i = _dateStartFrom; i < _dateEndsOn; i = i.AddMinutes(_Duration))
                    {
                        DateItemModel _item = new DateItemModel();
                        _item.ActualDate = item.Date;
                        _item.StartTime = i.TimeOfDay;

                        DateTime _newDate = i.AddMinutes(_Duration);
                        _item.EndTime = _newDate.TimeOfDay;
                        _dateModel.ActualDates.Add(_item);
                    }
                });
                _dates.Add(_dateModel);
            });
            return _dates;
        }
    }

    public class DateModel
    {
        public string DayName { get; set; }
        public List<DateItemModel> ActualDates { get; set; }
    }


    public class DateItemModel
    {
        public DateTime ActualDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }



}
