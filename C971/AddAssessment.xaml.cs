using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssessment : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Course _course;

        public AddAssessment(Course course)
        {
            InitializeComponent();
            _course = course;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Assessment>();
            var assessmentList = await _connection.QueryAsync<Assessment>($"SELECT Type FROM Assessments WHERE Course = '{_course.Id}'");
            foreach (Assessment assessment in assessmentList)
            {
                if (String.IsNullOrEmpty(assessment.Type))
                {
                    AssessmentType.Items.Add("Objective");
                    AssessmentType.Items.Add("Performance");
                }
                //else if (assessment.Type == "Objective")
                 //   AssessmentType.Items.Add("Performance");
               // else if (assessment.Type == "Performance")
               //     AssessmentType.Items.Add("Objective");
                else
                    return;
            }

            base.OnAppearing();
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            Assessment newAssessment = new Assessment();
            newAssessment.Title = AssessmentName.Text;
            newAssessment.StartDate = StartDate.Date;
            newAssessment.EndDate = EndDate.Date;
            newAssessment.Course = _course.Id;
            newAssessment.Type = (string)AssessmentType.SelectedItem;

            await _connection.InsertAsync(newAssessment);

            await Navigation.PopModalAsync();
        }
    }
}