using Java.Sql;
using StyleAndValidation.Models;
using StyleAndValidation.Services;
using StyleAndValidation.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StyleAndValidation.ViewModels
{
    public class RegisterPageViewModel:ViewModelBase
    {
        #region Service
       readonly AppServices appServices;
        #endregion

        #region Fields
        
        string username;
        string password;
        string fullName;
        string email;
        DateTime birthDate;
        #region validation messages
        bool showUserNameError;
        string userNameErrorMessage;
        bool showPasswordError;
        string passwordErrorMessage;
        #endregion
        #endregion

        #region Properties
        public string Username
        { get => username;
            set { if (username != value)
                    { 
                    username = value;
                    ((Command)RegisterCommand).ChangeCanExecute();
                    OnPropertyChanged();
                   
                }
            }
        }


        public string Password { get=>password; set { password = value;((Command)RegisterCommand).ChangeCanExecute(); OnPropertyChanged(); } }
        public string FullName { get=>fullName; set { fullName = value; OnPropertyChanged(); } }
        public string Email { get=>email; set { email = value; OnPropertyChanged(); } }

        public DateTime BirthDate { get => birthDate; set { birthDate = value; OnPropertyChanged(); ((Command)RegisterCommand).ChangeCanExecute(); } }

        #region Validation Properties
        public bool ShowUserNameError { get=>showUserNameError;  set { showUserNameError = value; OnPropertyChanged(); } }
        public string UserNameErrorMessage { get => userNameErrorMessage; set { userNameErrorMessage = value; OnPropertyChanged(); } }
        public bool ShowPasswordError { get => showPasswordError; set { showPasswordError = value; OnPropertyChanged(); } }
        
        public string PasswordErrorMessage { get => passwordErrorMessage; set { passwordErrorMessage = value; OnPropertyChanged(); } }
        #endregion

        #endregion


        #region Commands
        public ICommand RegisterCommand { get; protected set; }
        
        #endregion
        public RegisterPageViewModel(AppServices service)
        {
            appServices = service;
            RegisterCommand = new Command(async () => await RegisterUser(),()=>ValidateAll()) ;
            Username = string.Empty;
            ShowPasswordError = false;
            ShowUserNameError = false;
            BirthDate = DateTime.Parse("1/1/2000");
        }


        private async Task RegisterUser()
        {
           User registered=new () { BirthDate=BirthDate, Email=Email, FullName=FullName, Password=Password, Username=Username};
            #region מסך טעינה
            await AppShell.Current.GoToAsync("Loading");
           
            /*אם נרצה לעדכן את ההודעות שמוצגות במסך הפופאפ
             * int index=AppShell.Current.CurrentPage.Navigation.ModalStack.Count-1;
           
            var loading=AppShell.Current.CurrentPage.Navigation.ModalStack[index].BindingContext as LoadingPageViewModel;
            */
            #endregion
            bool ok = await appServices.RegisterUserAsync(registered);

            #region סגירת מסך טעינה
            await AppShell.Current.Navigation.PopModalAsync();
            //   await loading.Close();
            #endregion
            if (ok)
            {
                await AppShell.Current.DisplayAlert("הצלחה", "הנך מועבר.ת למסך הכניסה", "Ok");
                await AppShell.Current.GoToAsync("Login");
            }
           else
            {
              
                await AppShell.Current.DisplayAlert("או ויי", "משהו לא טוב קרה", "Ok");
            }
          
        }

        #region Validation
        private bool ValidateUserName()
        {
            #region שימוש בREGEX
            string pattern = @"^[a-zA-Z](?=.*[0-9])(?=.*[a-z])[a-zA-Z0-9]{3,7}$";

            bool ok =!string.IsNullOrEmpty(Username)&& Regex.IsMatch(Username, pattern);
            #endregion
            switch (ok)
            {
                case false:
                //הצג הודעת שגיאה
                ShowUserNameError = true;
                UserNameErrorMessage = "שם משתמש וסיסמה לא תקינים";
                    break;
                case true:
                    //בטל הודעת שגיאה
                ShowUserNameError = false;
                UserNameErrorMessage = string.Empty;
                    break;

            }
            //בדיקה האם הכפתור צריך להיות מנוטרל או פעיל
            return ok;
        }
        private bool ValidatePassword()
        {
            string pattern = @"^(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[!@#])[A-Za-z0-9!@#]{4,16}$";
            
            bool ok =!string.IsNullOrEmpty(Password)&&Regex.IsMatch(Password, pattern);
            switch (ok)
            {
                case false:
                    //הצג הודעת שגיאה
                    ShowPasswordError = true;
                    PasswordErrorMessage = "שם משתמש וסיסמה לא תקינים";
                    break;
                case true:
                    //בטל הודעת שגיאה
                    ShowPasswordError = false;
                    PasswordErrorMessage = string.Empty;
                    break;
            }
            //בדיקה האם הכפתור צריך להיות מנוטרל או פעיל
            return ok;
        }

        private bool ValidateAge()
        {
            return DateTime.Today.Year-BirthDate.Year>13;
        }

        private bool ValidateAll()
        {
            ValidatePassword();
            ValidateUserName();
            return !ShowPasswordError&&!ShowUserNameError&&ValidateAge();
        }

        #endregion
    }
}
