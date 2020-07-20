using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCourse : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Course _currentCourse;

        public EditCourse(Course currentCourse)
        {
            InitializeComponent();

            _currentCourse = currentCourse;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Course>();

            CourseName.Text = _currentCourse.CourseName;
            CourseStatus.SelectedItem = _currentCourse.Status;
            StartDate.Date = _currentCourse.StartDate;
            EndDate.Date = _currentCourse.EndDate;
            InstructorName.Text = _currentCourse.InstructorName;
            InstructorPhone.Text = _currentCourse.InstructorPhone;
            InstructorEmail.Text = _currentCourse.InstructorEmail;
            Notes.Text = _currentCourse.Notes;


            base.OnAppearing();
        }

        private async void Edit_Clicked(object sender, EventArgs e)
        {
            _currentCourse.CourseName = CourseName.Text;
            _currentCourse.StartDate = StartDate.Date;
            _currentCourse.EndDate = EndDate.Date;
            _currentCourse.Status = (string)CourseStatus.SelectedItem;
            _currentCourse.InstructorName = InstructorName.Text;
            _currentCourse.InstructorPhone = InstructorPhone.Text;
            _currentCourse.InstructorEmail = InstructorEmail.Text;
            _currentCourse.Notes = Notes.Text;
            _currentCourse.NotificationEnabled = EnableNotifications.On == true ? 1 : 0;


            if (FieldValidation.NullCheck(InstructorName.Text) &&
               FieldValidation.NullCheck(InstructorPhone.Text) &&
               FieldValidation.NullCheck(CourseName.Text))
            {
                if (FieldValidation.EmailCheck(InstructorEmail.Text))
                {
                    if (_currentCourse.StartDate < _currentCourse.EndDate)
                    {
                        await _connection.UpdateAsync(_currentCourse);

                        await Navigation.PopModalAsync();
                    }
                    else
                        await DisplayAlert("Warning", "Please make sure the start date is before the end date", "Ok");
                }
                else
                    await DisplayAlert("Warning", "Please make sure your email is valid before submitting", "Ok");
            }
            else
                await DisplayAlert("Warning", "Please make sure no fields are blank before submitting", "Ok");
        }
    }
}