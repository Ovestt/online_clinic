using System.Windows;


public static class ExitButton
{
    public static void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }
}

