using SQLite;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace C971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoursePage : ContentPage
    {

        private Course _currentCourse;
        private SQLiteAsyncConnection _connection;

        public CoursePage(Course course)
        {
            _currentCourse = course;

            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

        }

        protected override void OnAppearing()
        {
            Title = _currentCourse.CourseName;
            Status.Text = _currentCourse.Status;
            StartDate.Text = _currentCourse.StartDate.ToString("MM/dd/yyyy");
            EndDate.Text = _currentCourse.EndDate.ToString("MM/dd/yyyy");
            InstructorName.Text = _currentCourse.InstructorName;
            InstructorPhone.Text = _currentCourse.InstructorPhone;
            InstructorEmail.Text = _currentCourse.InstructorEmail;
            Notes.Text = _currentCourse.Notes;
            NotificationsEnabled.Text = _currentCourse.NotificationEnabled == 1 ? "Yes" : "No";

            base.OnAppearing();
        }

        async void Assements_Button(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AssessmentsPage(_currentCourse));
        }

        private async void Edit_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditCourse(_currentCourse));
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Caution", "Do you want to drop this course?", "Yes", "No");
            if (answer)
            {
                await _connection.DeleteAsync(_currentCourse);
                await Navigation.PopAsync();
            }

        }

        private async void ShareButton_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = Notes.Text,
                Title = "Share your notes on the course"
            });
        }
    }
}
