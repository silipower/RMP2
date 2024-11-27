using System.Collections.ObjectModel;

namespace NotebookApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Note> Notes { get; set; }
        private ObservableCollection<Note> filteredNotes;

        public MainPage()
        {
            InitializeComponent();
            Notes = new ObservableCollection<Note>();
            filteredNotes = new ObservableCollection<Note>(Notes);
            notesListView.ItemsSource = filteredNotes;
            LoadNotes();
        }

        public ListView NotesListView => notesListView;

        private async void LoadNotes()
        {
            var notesFromDb = await App.Database.GetNotesAsync();
            Notes.Clear();
            foreach (var note in notesFromDb)
            {
                Notes.Add(note);
            }
            filteredNotes = new ObservableCollection<Note>(Notes);
            notesListView.ItemsSource = filteredNotes;
        }

        public async void AddNote(Note note)
        {
            await App.Database.SaveNoteAsync(note);
            Notes.Add(note);
            filteredNotes.Add(note);
        }

        public async void UpdateNote(Note note, int index)
        {
            await App.Database.SaveNoteAsync(note);
            Notes[index] = note;
            filteredNotes[index] = note;
        }

        public async void DeleteNote(Note note)
        {
            await App.Database.DeleteNoteAsync(note);
            Notes.Remove(note);
            filteredNotes.Remove(note);
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
                NotesListView.SelectedItem = null;
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue.ToLower();
            filteredNotes.Clear();
            var filtered = Notes.Where(note => note.Title.ToLower().Contains(searchText)).ToList();
            foreach (var note in filtered)
            {
                filteredNotes.Add(note);
            }
        }
    }
}
