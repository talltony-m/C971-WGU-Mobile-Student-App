using SQLite;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentsPage : ContentPage
    {
        private Course _currentCourse;
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Assessment> _assessmentList;

        public AssessmentsPage(Course currentCourse)
        {
            InitializeComponent();
            Title = currentCourse.CourseName;
            _currentCourse = currentCourse;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            assessmentsListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(Assessment_Clicked);
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Assessment>();
            var assessmentList = await _connection.QueryAsync<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_currentCourse.Id}'");
            _assessmentList = new ObservableCollection<Assessment>(assessmentList);
            assessmentsListView.ItemsSource = _assessmentList;

            base.OnAppearing();
        }

        async private void Assessment_Clicked(object sender, ItemTappedEventArgs e)
        {
            Assessment assessment = (Assessment)e.Item;
            await Navigation.PushAsync(new AssessmentPage(assessment));
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            int assessmentCount = 0;
            bool assessmentCheck = true;
            foreach (Assessment assessment in _assessmentList)
            {
                if (assessment.Type == "Objective" || assessment.Type == "Performance")
                    assessmentCount++;
            }
            if (assessmentCount == 2)
            {
                await DisplayAlert("Caution", "You already have the max amount of assessments", "Ok");
                assessmentCheck = false;
            }

            if (assessmentCheck)
                await Navigation.PushModalAsync(new AddAssessment(_currentCourse));
        }
    }
}