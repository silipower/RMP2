using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace NotebookApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();
        private ObservableCollection<Note> filteredNotes = new ObservableCollection<Note>();

        public MainPage()
        {
            InitializeComponent();
            notesListView.ItemsSource = filteredNotes;
            LoadNotes();

            int selectedStyleIndex = Preferences.Get("SelectedStyle", 1);
            string selectedFont = Preferences.Get("SelectedFont", "Arial");
            int selectedFontSize = Preferences.Get("SelectedFontSize", 16);

            ApplySettings(selectedStyleIndex, selectedFont, selectedFontSize);

            stylePicker.SelectedIndex = selectedStyleIndex;
            fontPicker.SelectedItem = selectedFont;
            fontSizePicker.SelectedItem = selectedFontSize.ToString();
        }

        private async void LoadNotes()
        {
            var notesFromDb = await App.Database.GetNotesAsync();
            Notes.Clear();
            foreach (var note in notesFromDb)
            {
                if (note.ModifiedDate == DateTime.MinValue)
                {
                    note.ModifiedDate = note.CreatedDate;
                }
                Notes.Add(note);
            }
            UpdateFilteredNotes();
        }

        private void UpdateFilteredNotes(string filter = "")
        {
            filteredNotes.Clear();
            var filtered = string.IsNullOrWhiteSpace(filter)
                ? Notes
                : Notes.Where(note => note.Title.ToLower().Contains(filter.ToLower()));

            foreach (var note in filtered)
            {
                filteredNotes.Add(note);
            }
        }

        public async void AddNote(Note note)
        {
            note.CreatedDate = DateTime.Now;
            note.ModifiedDate = note.CreatedDate;

            await App.Database.SaveNoteAsync(note);
            Notes.Add(note);
            UpdateFilteredNotes();
        }

        public async void UpdateNote(Note note)
        {
            note.ModifiedDate = DateTime.Now;
            await App.Database.SaveNoteAsync(note);
            var existingNote = Notes.FirstOrDefault(n => n.Id == note.Id);
            if (existingNote != null)
            {
                int index = Notes.IndexOf(existingNote);
                Notes[index] = note;
            }
            UpdateFilteredNotes();
        }

        public async void DeleteNote(Note note)
        {
            await App.Database.DeleteNoteAsync(note);
            Notes.Remove(note);
            UpdateFilteredNotes();
        }

        private void OnCreateNoteClicked(object sender, EventArgs e)
        {
            DateTime selectedDate = Preferences.Get("SelectedDate", DateTime.Today);
            Navigation.PushAsync(new NotePage(null, selectedDate, this));
        }

        private async void OnNoteTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var note = e.Item as Note;
                DateTime selectedDate = Preferences.Get("SelectedDate", DateTime.Today);
                await Navigation.PushAsync(new NotePage(note, selectedDate, this));
                notesListView.SelectedItem = null;
            }
        }

        private void OnSettingsClicked(object sender, EventArgs e)
        {
            settingsPopup.IsVisible = true;
            SetMainPageInteraction(false);

            datePicker.Date = Preferences.Get("SelectedDate", DateTime.Today);
            int selectedStyleIndex = Preferences.Get("SelectedStyle", 1);
            stylePicker.SelectedIndex = selectedStyleIndex;

            string selectedFont = Preferences.Get("SelectedFont", "Arial");
            fontPicker.SelectedItem = selectedFont;
            int selectedFontSize = Preferences.Get("SelectedFontSize", 16);
            fontSizePicker.SelectedItem = selectedFontSize.ToString();
        }

        private void OnSaveSettingsClicked(object sender, EventArgs e)
        {
            DateTime selectedDate = datePicker.Date;
            int selectedStyleIndex = stylePicker.SelectedIndex;
            string selectedFont = fontPicker.SelectedItem as string;
            int selectedFontSize = int.Parse(fontSizePicker.SelectedItem.ToString());

            Preferences.Set("SelectedDate", selectedDate);
            Preferences.Set("SelectedStyle", selectedStyleIndex);
            Preferences.Set("SelectedFont", selectedFont);
            Preferences.Set("SelectedFontSize", selectedFontSize);

            ApplySettings(selectedStyleIndex, selectedFont, selectedFontSize);
            settingsPopup.IsVisible = false;
            SetMainPageInteraction(true);
        }

        private void OnCloseSettingsClicked(object sender, EventArgs e)
        {
            settingsPopup.IsVisible = false;
            SetMainPageInteraction(true);
        }

        private void OnRestoreDefaultStyleClicked(object sender, EventArgs e)
        {
            Preferences.Set("SelectedStyle", 1);
            Preferences.Set("SelectedFont", "Arial");
            Preferences.Set("SelectedFontSize", 16);
            Preferences.Set("SelectedDate", DateTime.Today);

            ApplySettings(1, "Arial", 16);

            stylePicker.SelectedIndex = 1;
            fontPicker.SelectedItem = "OpenSans";
            fontSizePicker.SelectedItem = 16.ToString();
        }

        public void ApplySettings(int styleIndex, string fontFamily, int fontSize)
        {
            switch (styleIndex)
            {
                case 0:
                    Application.Current.Resources["PrimaryColor"] = Colors.Black;
                    Application.Current.Resources["TextColor"] = Colors.White;
                    Application.Current.Resources["PageBackgroundColor"] = Colors.Black;
                    Application.Current.Resources["AccentColor"] = Colors.Cyan;
                    break;
                case 1:
                    Application.Current.Resources["PrimaryColor"] = Colors.White;
                    Application.Current.Resources["TextColor"] = Colors.Black;
                    Application.Current.Resources["PageBackgroundColor"] = Colors.White;
                    Application.Current.Resources["AccentColor"] = Colors.Blue;
                    break;
                case 2:
                    Application.Current.Resources["PrimaryColor"] = Colors.Grey;
                    Application.Current.Resources["TextColor"] = Colors.Yellow;
                    Application.Current.Resources["PageBackgroundColor"] = Colors.Grey;
                    Application.Current.Resources["AccentColor"] = Colors.Red;
                    break;
            }

            Application.Current.Resources["FontFamily"] = fontFamily;
            Application.Current.Resources["FontSize"] = fontSize;
        }

        private void OnFontChanged(object sender, EventArgs e)
        {
            string selectedFont = fontPicker.SelectedItem.ToString();
            Preferences.Set("SelectedFont", selectedFont);
            ApplySettings(Preferences.Get("SelectedStyle", 1), selectedFont, Preferences.Get("SelectedFontSize", 16));
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            int selectedFontSize = int.Parse(fontSizePicker.SelectedItem.ToString());
            Preferences.Set("SelectedFontSize", selectedFontSize);
            ApplySettings(Preferences.Get("SelectedStyle", 1), Preferences.Get("SelectedFont", "Arial"), selectedFontSize);
        }

        private void SetMainPageInteraction(bool isVisibled)
        {
            notesListView.IsEnabled = isVisibled;
            addButton.IsVisible = isVisibled;
            settingsButton.IsVisible = isVisibled;
            searchBar.IsVisible = isVisibled;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilteredNotes(e.NewTextValue);
        }
    }
}