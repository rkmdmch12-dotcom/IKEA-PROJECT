namespace IKEA_PROJECT;

public partial class NewPage1 : ContentPage
{
    public NewPage1()
    {
        InitializeComponent();
    }

    private async void OnCityClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string selectedCity)
        {
            await Navigation.PushAsync(new MainPage(selectedCity));
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync("התחבר", "כאן ייפתח מסך ההתחברות שלך", "אישור");
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync("הירשם", "כאן ייפתח מסך ההרשמה שלך", "אישור");
    }
}