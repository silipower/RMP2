namespace NotebookApp
{
    public partial class NotePage : ContentPage
    {
        private Note note;
        private MainPage mainPage;

        public NotePage(Note note, MainPage mainPage)
        {
            InitializeComponent();
            this.note = note;
            this.mainPage = mainPage;

            if (note != null)
            {
                titleEntry.Text = note.Title;
                contentEditor.Text = note.Content;
                datePicker.Date = note.DateTime;

                titleEntry.IsEnabled = false;
                contentEditor.IsEnabled = false;
                datePicker.IsEnabled = false;

                editButton.IsVisible = true;
                deleteButton.IsVisible = true;
                saveButton.IsVisible = false;
            }
            else
            {
                titleEntry.IsEnabled = true;
                contentEditor.IsEnabled = true;
                datePicker.IsEnabled = true;

                editButton.IsVisible = false;
                deleteButton.IsVisible = false;
                saveButton.IsVisible = true;
            }
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            titleEntry.IsEnabled = true;
            contentEditor.IsEnabled = true;
            datePicker.IsEnabled = true;

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
                    CreatedDate = DateTime.Now,
                };
            }

            note.Title = titleEntry.Text;
            note.Content = contentEditor.Text;
            note.DateTime = datePicker.Date;
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
                    mainPage.UpdateNote(note, mainPage.Notes.IndexOf(note));
                }
            }

            await Navigation.PopAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (note != null)
            {
                await App.Database.DeleteNoteAsync(note);

                if (mainPage != null)
                {
                    mainPage.DeleteNote(note);
                }
            }

            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            mainPage.NotesListView.SelectedItem = null;
        }
    }
}
