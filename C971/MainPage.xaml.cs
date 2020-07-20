using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace C971
{
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int Term { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public string Notes { get; set; }
        public int NotificationEnabled { get; set; }
    }

    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public int Course { get; set; }
        public int NotificationEnabled { get; set; }
    }


    public partial class MainPage : ContentPage
    {

        private SQLiteAsyncConnection _connection;
        public ObservableCollection<Term> _termList;
        private bool _firstAppearnce = true;

        public MainPage()
        {
            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            termListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(Term_Clicked);
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Term>();
            await _connection.CreateTableAsync<Course>();
            await _connection.CreateTableAsync<Assessment>();

            var termList = await _connection.Table<Term>().ToListAsync();

            // Seed data if there are no Terms
            if (!termList.Any())
            {
                // Seed Data
                var newTerm = new Term();
                newTerm.Title = "Term 1";
                newTerm.StartDate = new DateTime(2020, 09, 01);
                newTerm.EndDate = new DateTime(2020, 12, 18);
                await _connection.InsertAsync(newTerm);
                termList.Add(newTerm);

                var newCourse = new Course();
                newCourse.CourseName = "Mobile Application";
                newCourse.StartDate = new DateTime(2020, 09, 01);
                newCourse.EndDate = new DateTime(2020, 12, 18);
                newCourse.Status = "Completed";
                newCourse.InstructorName = "Tony Martin";
                newCourse.InstructorPhone = "509-423-6233";
                newCourse.InstructorEmail = "amar446@wgu.edu";
                newCourse.NotificationEnabled = 1;
                newCourse.Notes = "Notes about the course";
                newCourse.Term = newTerm.Id;
                await _connection.InsertAsync(newCourse);

                Assessment newObjectiveAssessment = new Assessment();
                newObjectiveAssessment.Title = "MOP-1";
                newObjectiveAssessment.StartDate = new DateTime(2020, 12, 15);
                newObjectiveAssessment.EndDate = new DateTime(2020, 12, 18);
                newObjectiveAssessment.Course = newCourse.Id;
                newObjectiveAssessment.Type = "Objective";
                newObjectiveAssessment.NotificationEnabled = 1;
                await _connection.InsertAsync(newObjectiveAssessment);

                Assessment newPerformanceAssessment = new Assessment();
                newPerformanceAssessment.Title = "MAP-1";
                newPerformanceAssessment.StartDate = new DateTime(2020, 12, 10);
                newPerformanceAssessment.EndDate = new DateTime(2020, 12, 14);
                newPerformanceAssessment.Course = newCourse.Id;
                newPerformanceAssessment.Type = "Performance";
                newPerformanceAssessment.NotificationEnabled = 1;
                await _connection.InsertAsync(newPerformanceAssessment);
            }


            var courseList = await _connection.Table<Course>().ToListAsync();
            var assessmentList = await _connection.Table<Assessment>().ToListAsync();

            // Sets Notifications for Course and Assessment start/end times
            if (_firstAppearnce)
            {
                _firstAppearnce = false;

                int courseId = 0;
                foreach (Course course in courseList)
                {
                    courseId++;
                    if (course.NotificationEnabled == 1)
                    {
                        if (course.StartDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} starts today!", courseId);
                        if (course.EndDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} ends today!", courseId);
                    }
                }

                int assessmentId = courseId;
                foreach (Assessment assessment in assessmentList)
                {
                    assessmentId++;
                    if (assessment.NotificationEnabled == 1)
                    {
                        if (assessment.StartDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{assessment.Title} starts today!", assessmentId);
                        if (assessment.EndDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{assessment.Title} ends today!", assessmentId);
                    }
                }
            }


            _termList = new ObservableCollection<Term>(termList);
            termListView.ItemsSource = _termList;

            base.OnAppearing();
        }

        async private void Term_Clicked(object sender, ItemTappedEventArgs e)
        {
            Term term = (Term)e.Item;
            await Navigation.PushAsync(new TermPage(term));
        }

        private async void Add_Term(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddNewTerm(this));

        }


    }
}