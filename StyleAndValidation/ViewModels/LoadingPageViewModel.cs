using StyleAndValidation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StyleAndValidation.ViewModels
{
    public class LoadingPageViewModel:ViewModelBase
    {

        private bool isRunning;
        public bool IsRunning
        {
            get => isRunning; set { isRunning=value; OnPropertyChanged(); }}
        
        public LoadingPageViewModel()
        {
            IsRunning = true;
            
        }
        public async Task Close()
        {
            IsRunning = false;
            await AppShell.Current.Navigation.PopModalAsync();  
            
       //     await AppShell.Current.GoToAsync("..");
        }
    }
}
