using System.Collections.ObjectModel;
using System.Linq;

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
        }

        private async void LoadNotes()
        {
            var notesFromDb = await App.Database.GetNotesAsync();
            Notes.Clear();
            foreach (var note in notesFromDb)
            {
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

            // Обновляем объект заметки в коллекции
            var existingNote = Notes.FirstOrDefault(n => n.Id == note.Id);
            if (existingNote != null)
            {
                int index = Notes.IndexOf(existingNote);
                Notes[index] = note; // Заменяем старую версию заметки новой
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
            Navigation.PushAsync(new NotePage(null, this));
        }

        private async void OnNoteTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var note = e.Item as Note;
                await Navigation.PushAsync(new NotePage(note, this));
                notesListView.SelectedItem = null;
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue;
            UpdateFilteredNotes(searchText);
        }
    }
}
