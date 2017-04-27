using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using IndianCalendarApp.Resources;
using System.Windows.Resources;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace IndianCalendarApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        DateTime calDate;
        List<EventItem> events;
        int selectedYear;

        public MainPage()
        {
            InitializeComponent();
              
            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            calDate = DateTime.Today;
            events = new List<EventItem>();
            FillEvents();
            selectedYear = calDate.Year;
            LoadCalendar(calDate);
            //ShowEventsForMonth(calDate);//ShowDateEvents(DateTime.Now, string.Format(HinduCalendar.Resources.AppResources.todayMessage, DateTime.Now.ToString("d MMMM yyyy")));
            adControl.ErrorOccurred += new EventHandler<Microsoft.Advertising.AdErrorEventArgs>(listenError);
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        void listenError(object o, Microsoft.Advertising.AdErrorEventArgs args)
        {

        }

        void FillEvents()
        {
            try
            {
                if (selectedYear == calDate.Year) return;
                selectedYear = calDate.Year;

                events = new List<EventItem>();
                string path = string.Format("IndianCalendarApp;component/{0}.xml", calDate.Year);
                StreamResourceInfo xml =
                    Application.GetResourceStream(new Uri(path, UriKind.Relative));
                if (xml != null)
                {
                    var appDataXml = XDocument.Load(xml.Stream);
                    XDocument xdoc = appDataXml;
                    var rootCategory = xdoc.Root.Element("Events");
                    List<string> list = new List<string>();

                    foreach (XElement item in appDataXml.Descendants("item"))
                    {
                        int day = (int)item.Attribute("day");
                        int month = (int)item.Attribute("month");
                        string title = (string)item;
                        string details = Convert.ToString(item.Attribute("description") != null ? item.Attribute("description").Value : string.Empty);
                        events.Add(new EventItem { Day = day, Month = month, Title = title, Details = details });
                    }


                }
            }
            catch (Exception)
            {


            }

        }
        void LoadCalendar(DateTime date)
        {
            FillEvents();
            ShowEventsForMonth(date);
            //txtselectedDate.Text = string.Format(HinduCalendar.Resources.AppResources.todayMessage, DateTime.Now.ToString("d MMMM yyyy"));
            //lstEvents.DataContext = new List<EventItem>();
            //CalendarHeader.Text = date.ToString("MMMM yyyy");

            
            date = new DateTime(date.Year, date.Month, 1);
            int dayOfWeek = (int)date.DayOfWeek + 1;
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            int i = 1;
            
            foreach (var weekPanel in CalendarContainer.Children)
            {
                foreach (var datePanel in (weekPanel as StackPanel).Children)
                {
                    var dateLabel = (datePanel as Grid).Children[0] as TextBlock;
                    var isCurrentDate = false;
                    if (i >= dayOfWeek && i < (daysInMonth + dayOfWeek))
                    {
                        isCurrentDate = false;
                        dateLabel.Text = (i - dayOfWeek + 1).ToString();
                        if (DateTime.Today.Day == (i - dayOfWeek + 1) && DateTime.Today.Month == date.Month && DateTime.Today.Year == date.Year)
                        {
                            isCurrentDate = true;
                            //(SolidColorBrush)Application.Current.Resources["PhoneForegroundBrush"]; 
                            //(datePanel as Grid).Style = (Style)Application.Current.Resources["LayoutPunam"];
                            //(datePanel as Grid).Background = new SolidColorBrush(Colors.LightGray);
                            //(o2 as Grid).Background = new ImageBrush() { AlignmentX = AlignmentX.Left, Stretch=Stretch.None, AlignmentY = AlignmentY.Top, ImageSource = new BitmapImage(new Uri("/HinduCalendar;component/Assets/pixel.gif", UriKind.Relative)) };
                            //dateLabel.Foreground = new SolidColorBrush(Colors.Brown);
                        }
                        else if (events.Any(x => x.Month == date.Month && x.Day == (i - dayOfWeek + 1)))
                        {
                            dateLabel.Foreground = new SolidColorBrush(Colors.Orange);
                            if (!isCurrentDate)
                            {
                                //(datePanel as Grid).Background = new SolidColorBrush(Colors.Transparent);
                            }
                            //dateLabel.Tag = new DateTime(date.Year, date.Month, (i - dayOfWeek + 1));
                        }
                        else
                        {

                            //(datePanel as Grid).Background = new SolidColorBrush(Colors.Transparent);
                            dateLabel.Foreground = (SolidColorBrush)Application.Current.Resources["PhoneForegroundBrush"];
                        }

                    }
                    else
                        dateLabel.Text = string.Empty;
                    i++;
                }
            }
        }

        private void GoToPreviousMonth(object sender, RoutedEventArgs e)
        {
            calDate = calDate.AddMonths(-1);
            LoadCalendar(calDate);
        }

        private void GoToNextMonth(object sender, RoutedEventArgs e)
        {
            calDate = calDate.AddMonths(1);
            LoadCalendar(calDate);
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            return;
            //lstEvents.Items.Clear();
            var qualifier = ((TextBlock)sender);

            if (qualifier.Tag != null)
            {
                var selectedDate = Convert.ToDateTime(qualifier.Tag);
                ShowDateEvents(selectedDate);
                //txtMsg.Text = HinduCalendar.Resources.AppResources.selectedDateTapMessage;
            }
            else
            {
                resetSelectedDate();
            }

        }

        private void ShowDateEvents(DateTime selectedDate, string dateLabel = null)
        {


            //txtselectedDate.Text = dateLabel ?? selectedDate.ToString("d MMMM yyyy");

            var list = new ObservableCollection<EventItem>();
            foreach (var item in events.Where(eve => eve.Day == selectedDate.Day && eve.Month == selectedDate.Month))
            {
                list.Add(item);
            }
            lstEvents.DataContext = list;
        }
        private void lstEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedItem = (EventItem)e.AddedItems[0];
                MessageBox.Show(selectedItem.Title + Environment.NewLine + Environment.NewLine + selectedItem.Details.Replace("|", Environment.NewLine));
            }

        }

        private void resetSelectedDate()
        {
            //txtselectedDate.Text = string.Format(HinduCalendar.Resources.AppResources.todayMessage, DateTime.Now.ToString("d MMMM yyyy"));
            lstEvents.DataContext = new List<EventItem>();
            //txtMsg.Text = string.Empty;

        }

        private void ShowEventsForMonth(DateTime date)
        {
            //txtselectedDate.Text = "Festivals in " + date.ToString("MMMM yyyy");
            lstEvents.DataContext = events.Where(eve => eve.Month == date.Month).OrderBy(eve => eve.Day).ToList();
        }


        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
          
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void PivotItem_LayoutUpdated(object sender, EventArgs e)
        {

        }

        private void PivotItem_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void PivotItem_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {

        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = e.AddedItems[0] as PivotItem;
            
        }

        private void datePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (birthDate.Value.HasValue)
            {
                var Years = (DateTime.Now.Year - birthDate.Value.Value.Year);
                var Months =  (((DateTime.Now.Year + 1) - (birthDate.Value.Value.Year - 1)) * 12) + DateTime.Now.Month + birthDate.Value.Value.Month;
                var Weeks = Convert.ToInt32((DateTime.Now - birthDate.Value.Value).TotalDays / 7);
                var Days = Convert.ToInt32((DateTime.Now - birthDate.Value.Value).TotalDays);

                txtBirthDateMsg.Text = "You have completed ...";
                txtbirthYears.Text = Years.ToString();
                txtbirthMonths.Text = Months.ToString();
                txtbirthWeeks.Text = Weeks.ToString();
                txtbirthDays.Text = Days.ToString();
            }
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }

    public class EventItem
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }

    }
}