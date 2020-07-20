
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewTerm : ContentPage
    {
        public MainPage _mainPage;
        private SQLiteAsyncConnection _connection;

        public AddNewTerm(MainPage mainPage)
        {
            InitializeComponent();

            _mainPage = mainPage;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            var newTerm = new Term();
            newTerm.Title = TermTitle.Text;
            newTerm.StartDate = startDate.Date;
            newTerm.EndDate = endDate.Date;

            if (newTerm.StartDate < newTerm.EndDate)
            {
                if (FieldValidation.NullCheck(newTerm.Title))
                {
                    await _connection.InsertAsync(newTerm);

                    _mainPage._termList.Add(newTerm);
                    await Navigation.PopModalAsync();
                }
                else
                    await DisplayAlert("Warining", "Please make sure the Term Title is not blank", "Ok");
            }
            else
                await DisplayAlert("Warining", "Please make sure the start date is before the end date", "Ok");
        }
    }
}