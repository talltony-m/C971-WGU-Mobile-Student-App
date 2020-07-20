using SQLite;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace C971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentPage : ContentPage
    {
        private Assessment _assessment;
        private SQLiteAsyncConnection _connection;
        private Course currentCourse;

        public AssessmentPage(Assessment assessment)
        {
            _assessment = assessment;

            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        public AssessmentPage(Course currentCourse)
        {
            this.currentCourse = currentCourse;
        }

        protected override void OnAppearing()
        {
            Title = _assessment.Type;
            AssessmentName.Text = _assessment.Title;
            StartDate.Text = _assessment.StartDate.ToString("MM/dd/yyyy");
            EndDate.Text = _assessment.EndDate.ToString("MM/dd/yyyy");
            AssessmentNotification.Text = _assessment.NotificationEnabled == 1 ? "Yes" : "No";

            base.OnAppearing();
        }
        private async void Edit_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditAssessment(_assessment));
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Caution", "Do you want to delete this Assessment?", "Yes", "No");
            if (answer)
            {
                await _connection.DeleteAsync(_assessment);
                await Navigation.PopAsync();
            }
        }
    }
}