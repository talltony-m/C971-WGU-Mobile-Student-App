using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCourse : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Term _currentTerm;

        public AddCourse(Term currentTerm)
        {
            InitializeComponent();
            _currentTerm = currentTerm;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Course>();

            base.OnAppearing();
        }
      
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            var newCourse = new Course();
            newCourse.CourseName = CourseName.Text;
            newCourse.StartDate = startDate.Date;
            newCourse.EndDate = endDate.Date;
            newCourse.Status = (string)CourseStatus.SelectedItem;
            newCourse.InstructorName = InstructorName.Text;
            newCourse.InstructorPhone = InstructorPhone.Text;
            newCourse.InstructorEmail = InstructorEmail.Text;
            newCourse.Notes = Notes.Text;
            newCourse.Term = _currentTerm.Id;


            if (FieldValidation.NullCheck(InstructorName.Text) &&
               FieldValidation.NullCheck(InstructorPhone.Text) &&
               FieldValidation.NullCheck(CourseName.Text))
            {
                if (FieldValidation.EmailCheck(InstructorEmail.Text))
                {
                    if (newCourse.StartDate < newCourse.EndDate)
                    {
                        await _connection.InsertAsync(newCourse);

                        await Navigation.PopModalAsync();
                    }
                    else
                        await DisplayAlert("Warning", "Please make sure the start date is before the end date", "Ok");
                }
                else
                    await DisplayAlert("Warning", "Please make sure your email is valid before submitting", "Ok");
            }
            else
                await DisplayAlert("Warning", "Please make sure all fields are filled in before submitting", "Ok");

        }
    }
}