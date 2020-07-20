using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTerm : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Term _currentTerm;

        public EditTerm(Term currentTerm)
        {
            InitializeComponent();
            _currentTerm = currentTerm;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Term>();

            TermTitle.Text = _currentTerm.Title;
            startDate.Date = _currentTerm.StartDate;
            endDate.Date = _currentTerm.EndDate;
            base.OnAppearing();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            _currentTerm.Title = TermTitle.Text;
            _currentTerm.StartDate = startDate.Date;
            _currentTerm.EndDate = endDate.Date;

            if (_currentTerm.StartDate < _currentTerm.EndDate)
            {
                if (FieldValidation.NullCheck(_currentTerm.Title))
                {
                    await _connection.UpdateAsync(_currentTerm);

                    await Navigation.PopModalAsync();
                }
                else
                    await DisplayAlert("Warning", "Please make sure the Term Title is not blank", "Ok");
            }
            else
                await DisplayAlert("Warning", "Please make sure the Start Date is before the End Date", "Ok");
        }
    }
}