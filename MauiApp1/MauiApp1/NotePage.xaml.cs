using System;

namespace NotebookApp
{
    public partial class NotePage : ContentPage
    {
        private Note note;
        private MainPage mainPage;
        private DateTime selectedDate;

        public NotePage(Note note, DateTime selectedDate, MainPage mainPage)
        {
            InitializeComponent();
            this.note = note;
            this.mainPage = mainPage;
            this.selectedDate = selectedDate;

            if (note != null)
            {
                titleEntry.Text = note.Title;
                contentEditor.Text = note.Content;
                datePicker.Date = note.DateTime.Date;
                timePicker.Time = note.DateTime.TimeOfDay;

                titleEntry.IsEnabled = false;
                contentEditor.IsEnabled = false;
                datePicker.IsEnabled = false;
                timePicker.IsEnabled = false;

                editButton.IsVisible = true;
                deleteButton.IsVisible = true;
                saveButton.IsVisible = false;
            }
            else
            {
                datePicker.Date = selectedDate;  // Устанавливаем дату по умолчанию

                titleEntry.IsEnabled = true;
                contentEditor.IsEnabled = true;
                datePicker.IsEnabled = true;
                timePicker.IsEnabled = false; // Скрыт до выбора даты

                editButton.IsVisible = false;
                deleteButton.IsVisible = false;
                saveButton.IsVisible = true;
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (e.NewDate != DateTime.Today)
            {
                timePicker.IsVisible = true;
                timePicker.IsEnabled = true;
            }
            else
            {
                timePicker.Time = DateTime.Now.TimeOfDay;
                timePicker.IsVisible = false;
                timePicker.IsEnabled = false;
            }

        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            titleEntry.IsEnabled = true;
            contentEditor.IsEnabled = true;
            datePicker.IsEnabled = true;

            if (datePicker.Date != DateTime.Today)
            {
                timePicker.IsVisible = true;
                timePicker.IsEnabled = true;
            }

            editButton.IsVisible = false;
            saveButton.IsVisible = true;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleEntry.Text))
            {
                await DisplayAlert("Ошибка", "Название заметки не может быть пустым.", "ОК");
                return;
            }

            if (note == null)
            {
                note = new Note
                {
                    CreatedDate = DateTime.Now
                };
            }

            note.Title = titleEntry.Text;
            note.Content = contentEditor.Text;

            DateTime selectedDate = datePicker.Date;
            TimeSpan selectedTime = timePicker.IsVisible ? timePicker.Time : DateTime.Now.TimeOfDay;
            note.DateTime = selectedDate.Add(selectedTime);
            note.ModifiedDate = DateTime.Now;

            if (mainPage != null)
            {
                if (note.Id == 0)
                {
                    await App.Database.SaveNoteAsync(note);
                    mainPage.AddNote(note);
                }
                else
                {
                    await App.Database.SaveNoteAsync(note);
                    mainPage.UpdateNote(note);
                }
            }

            await Navigation.PopAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (note != null)
            {
                await App.Database.DeleteNoteAsync(note);
                mainPage?.DeleteNote(note);
            }

            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
